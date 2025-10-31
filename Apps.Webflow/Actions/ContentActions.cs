using Apps.Webflow.Constants;
using Apps.Webflow.Extensions;
using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Request.Content;
using Apps.Webflow.Models.Response.Content;
using Apps.Webflow.Services;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Blackbird.Applications.SDK.Blueprints;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Filters.Transformations;
using Blackbird.Filters.Xliff.Xliff2;
using HtmlAgilityPack;
using System.Net.Mime;
using System.Text;

namespace Apps.Webflow.Actions;

[ActionList("Content")]
public class ContentActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) 
    : WebflowInvocable(invocationContext)
{
    private readonly ContentServicesFactory _factory = new ContentServicesFactory(invocationContext);

    [BlueprintActionDefinition(BlueprintAction.SearchContent)]
    [Action("Search content", Description = "Search for any type of content")]
    public async Task<SearchContentResponse> SearchContent(
        [ActionParameter] SiteRequest site,
        [ActionParameter] SearchContentRequest request,
        [ActionParameter] DateFilter dateFilter)
    {
        request.ContentTypes ??= ContentTypes.SupportedContentTypes;
        var contentServices = _factory.GetContentServices(request.ContentTypes);
        return await contentServices.ExecuteMany(Client.GetSiteId(site.SiteId), request, dateFilter);
    }

    [BlueprintActionDefinition(BlueprintAction.DownloadContent)]
    [Action("Download content", Description = "Download content as HTML for a specific content type based on its ID")]
    public async Task<DownloadContentResponse> DownloadContent(
        [ActionParameter] SiteRequest site,
        [ActionParameter] DownloadContentRequest request,
        [ActionParameter] ContentFilter contentFilter)
    {
        var service = _factory.GetContentService(contentFilter.ContentType);
        var stream = await service.DownloadContent(Client.GetSiteId(site.SiteId), request);
        var fileName = $"{contentFilter.ContentType.Replace(' ', '_').ToLower()}_{request.ContentId}.html";
        var fileReference = await fileManagementClient.UploadAsync(stream, MediaTypeNames.Text.Html, fileName);
        return new DownloadContentResponse(fileReference);
    }

    [BlueprintActionDefinition(BlueprintAction.UploadContent)]
    [Action("Upload content", Description = "Update content from an HTML file")]
    public async Task UploadContent(
        [ActionParameter] SiteRequest site,
        [ActionParameter] UploadContentRequest request)
    {
        var file = await fileManagementClient.DownloadAsync(request.Content);

        var html = Encoding.UTF8.GetString(await file.GetByteData());
        if (Xliff2Serializer.IsXliff2(html))
        {
            html = Transformation.Parse(html, request.Content.Name).Target().Serialize();
            if (html == null) throw new PluginMisconfigurationException("XLIFF did not contain files");
        }

        if (string.IsNullOrEmpty(request.ContentType))
            request.ContentType = GetContentType(html);

        using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(html));
        var service = _factory.GetContentService(request.ContentType);
        await service.UploadContent(memoryStream, site.SiteId, request);
    }

    private static string GetContentType(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var head = doc.DocumentNode.SelectSingleNode("//head") 
            ?? throw new PluginMisconfigurationException(
                "HTML file does not have the 'head' attribute to recognize the content type. Please provide in the input"
            );

        var metaTags = head.SelectNodes("meta") 
            ?? throw new PluginMisconfigurationException(
                "HTML file does not have 'meta' attributes in the 'head' part to recognize the content type. Please provide in the input"
            );

        foreach (var meta in metaTags)
        {
            var nameAttr = meta.GetAttributeValue("name", "").Trim();

            if (nameAttr == "blackbird-page-id") return ContentTypes.Page;
            if (nameAttr == "blackbird-component-id") return ContentTypes.Component;
            if (nameAttr == "blackbird-collection-item-id") return ContentTypes.CollectionItem;
        }

        throw new PluginMisconfigurationException(
            "Unable to recognize the content type. Please provide in the input"
        );
    }
}
