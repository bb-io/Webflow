using Apps.Webflow.Constants;
using Apps.Webflow.Extensions;
using Apps.Webflow.Helper;
using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Request.Content;
using Apps.Webflow.Models.Request.Date;
using Apps.Webflow.Models.Response.Content;
using Apps.Webflow.Services;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Blackbird.Applications.SDK.Blueprints;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using HtmlAgilityPack;
using System.Net.Mime;

namespace Apps.Webflow.Actions;

[ActionList("Content")]
public class ContentActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) 
    : WebflowInvocable(invocationContext)
{
    private readonly ContentServicesFactory _factory = new(invocationContext);

    [BlueprintActionDefinition(BlueprintAction.SearchContent)]
    [Action("Search content", Description = "Search for any type of content")]
    public async Task<SearchContentResponse> SearchContent(
        [ActionParameter] SiteRequest site,
        [ActionParameter] SearchContentRequest request,
        [ActionParameter] ContentDateFilter dateFilter)
    {
        request.ContentTypes ??= ContentTypes.SupportedContentTypes;
        var contentServices = _factory.GetContentServices(request.ContentTypes);
        return await contentServices.ExecuteMany(Client.GetSiteId(site.SiteId), request, dateFilter);
    }

    [BlueprintActionDefinition(BlueprintAction.DownloadContent)]
    [Action("Download content", Description = "Download content to a file")]
    public async Task<DownloadContentResponse> DownloadContent(
        [ActionParameter] SiteRequest site,
        [ActionParameter] DownloadContentRequest request,
        [ActionParameter] ContentFilter contentFilter)
    {
        request.FileFormat = request.FileFormat is null ? MediaTypeNames.Text.Html : request.FileFormat;
        var service = _factory.GetContentService(contentFilter.ContentType);

        var stream = await service.DownloadContent(Client.GetSiteId(site.SiteId), request);

        string fileName = FileHelper.GetDownloadedFileName(request.FileFormat, request.ContentId, contentFilter.ContentType);
        string contentType = request.FileFormat == MediaTypeNames.Text.Html ? MediaTypeNames.Text.Html : MediaTypeNames.Application.Json;

        var fileReference = await fileManagementClient.UploadAsync(stream, contentType, fileName);
        return new DownloadContentResponse(fileReference);
    }

    [BlueprintActionDefinition(BlueprintAction.UploadContent)]
    [Action("Upload content", Description = "Update content from a file")]
    public async Task UploadContent(
        [ActionParameter] SiteRequest site,
        [ActionParameter] UploadContentRequest request)
    {
        await using var source = await fileManagementClient.DownloadAsync(request.Content);
        var bytes = await source.GetByteData();
        await using var stream = new MemoryStream(bytes);

        if (string.IsNullOrEmpty(request.ContentType))
            request.ContentType = GetContentType("");

        var service = _factory.GetContentService(request.ContentType);
        await service.UploadContent(stream, Client.GetSiteId(site.SiteId), request);
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
