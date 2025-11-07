using Apps.Webflow.Constants;
using Apps.Webflow.Helper;
using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Entities;
using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Request.Components;
using Apps.Webflow.Models.Request.Content;
using Apps.Webflow.Models.Response.Components;
using Apps.Webflow.Models.Response.Pagination;
using Apps.Webflow.Services;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Filters.Transformations;
using Blackbird.Filters.Xliff.Xliff2;
using RestSharp;
using System.Net.Mime;
using System.Text;

namespace Apps.Webflow.Actions;

[ActionList("Components")]
public class ComponentsActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : WebflowInvocable(invocationContext)
{
    private readonly ContentServicesFactory _factory = new ContentServicesFactory(invocationContext);

    [Action("Search components", Description = "Search all components for a site")]
    public async Task<SearchComponentsResponse> SearchComponents(
        [ActionParameter] SiteRequest site,
        [ActionParameter] SearchComponentsRequest input)
    {
        var endpoint = $"sites/{Client.GetSiteId(site.SiteId)}/components";
        var request = new RestRequest(endpoint, Method.Get);

        IEnumerable<ComponentEntity> pages = await Client.Paginate<ComponentEntity, ComponentsPaginationResponse>(request, r => r.Components);

        pages = FilterHelper.ApplyBooleanFilter(pages, input.InludeReadOnly, c => c.ReadOnly);
        pages = FilterHelper.ApplyContainsFilter(pages, input.NameContains, c => c.Name);
        pages = FilterHelper.ApplyContainsFilter(pages, input.GroupContains, c => c.Group);

        return new SearchComponentsResponse(pages.ToList());
    }

    [Action("Download component", Description = "Get the component content in HTML file")]
    public async Task<DownloadComponentResponse> DownloadComponent(
        [ActionParameter] SiteRequest site,
        [ActionParameter] DownloadComponentContentRequest input)
    {
        string fileFormat = input.FileFormat ?? MediaTypeNames.Text.Html;

        var downloadRequest = new DownloadContentRequest
        {
            Locale = input.LocaleId,
            ContentId = input.ComponentId,
            FileFormat = fileFormat,
        };

        var service = _factory.GetContentService(ContentTypes.Component);
        var stream = await service.DownloadContent(Client.GetSiteId(site.SiteId), downloadRequest);

        string fileName = FileHelper.GetDownloadedFileName(fileFormat, input.ComponentId, ContentTypes.Component);
        string contentType = fileFormat == MediaTypeNames.Text.Html ? MediaTypeNames.Text.Html : MediaTypeNames.Application.Json;

        var file = await fileManagementClient.UploadAsync(stream, contentType, fileName);
        return new(file);
    }

    [Action("Upload component", Description = "Update component content using HTML file")]
    public async Task UploadComponent(
        [ActionParameter] SiteRequest site,
        [ActionParameter] UpdateComponentContentRequest input)
    {
        await using var source = await fileManagementClient.DownloadAsync(input.File);
        var html = Encoding.UTF8.GetString(await source.GetByteData());

        if (Xliff2Serializer.IsXliff2(html))
        {
            html = Transformation.Parse(html, input.File.Name).Target().Serialize();
            if (html == null) throw new PluginMisconfigurationException("XLIFF did not contain files");
        }

        await using var ms = new MemoryStream(Encoding.UTF8.GetBytes(html));
        var doc = new HtmlAgilityPack.HtmlDocument();
        doc.Load(ms);
        ms.Position = 0;

        var updateRequest = new UploadContentRequest 
        {
            Locale = input.LocaleId,
            ContentId = input.ComponentId,
        }; 

        var service = _factory.GetContentService(ContentTypes.Component);
        await service.UploadContent(ms, Client.GetSiteId(site.SiteId), updateRequest);
    }
}
