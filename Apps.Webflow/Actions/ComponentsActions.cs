using Apps.Webflow.Constants;
using Apps.Webflow.Helper;
using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Entities;
using Apps.Webflow.Models.Identifiers;
using Apps.Webflow.Models.Request.Components;
using Apps.Webflow.Models.Request.Content;
using Apps.Webflow.Models.Response.Components;
using Apps.Webflow.Models.Response.Pagination;
using Apps.Webflow.Services;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using RestSharp;
using System.Net.Mime;

namespace Apps.Webflow.Actions;

[ActionList("Components")]
public class ComponentsActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : WebflowInvocable(invocationContext)
{
    private readonly ContentServicesFactory _factory = new(invocationContext, fileManagementClient);

    [Action("Search components", Description = "Search all components for a site")]
    public async Task<SearchComponentsResponse> SearchComponents(
        [ActionParameter] SiteIdentifier site,
        [ActionParameter] SearchComponentsRequest input)
    {
        var endpoint = $"sites/{Client.GetSiteId(site.SiteId)}/components";
        var request = new RestRequest(endpoint, Method.Get);

        IEnumerable<ComponentEntity> components = 
            await Client.Paginate<ComponentEntity, ComponentsPaginationResponse>(request, r => r.Components);

        components = FilterHelper.ApplyBooleanFilter(components, input.IncludeReadOnly, c => c.ReadOnly);
        components = FilterHelper.ApplyContainsFilter(components, input.NameContains, c => c.Name);
        components = FilterHelper.ApplyContainsFilter(components, input.GroupContains, c => c.Group);

        return new SearchComponentsResponse(components.ToList());
    }

    [Action("Download component", Description = "Download the component content")]
    public async Task<DownloadComponentResponse> DownloadComponent(
        [ActionParameter] SiteIdentifier site,
        [ActionParameter] DownloadComponentContentRequest input,
        [ActionParameter] LocaleIdentifier locale)
    {
        string fileFormat = input.FileFormat ?? ContentFormats.InteroperableHtml;

        var downloadRequest = new DownloadContentRequest
        {
            Locale = locale.Locale,
            ContentId = input.ComponentId,
            FileFormat = fileFormat,
        };

        var service = _factory.GetContentService(ContentTypes.Component);
        var file = await service.DownloadContent(Client.GetSiteId(site.SiteId), downloadRequest);
        return new(file);
    }

    [Action("Upload component", Description = "Update component content from a file")]
    public async Task UploadComponent(
        [ActionParameter] SiteIdentifier site,
        [ActionParameter] UpdateComponentContentRequest input,
        [ActionParameter] LocaleIdentifier locale)
    {
        await using var source = await fileManagementClient.DownloadAsync(input.File);
        var bytes = await source.GetByteData();
        await using var stream = new MemoryStream(bytes);

        var updateRequest = new UploadContentRequest 
        {
            Locale = locale.Locale,
            ContentId = input.ComponentId,
        }; 

        var service = _factory.GetContentService(ContentTypes.Component);
        await service.UploadContent(stream, Client.GetSiteId(site.SiteId), updateRequest);
    }
}
