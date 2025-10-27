using Apps.Webflow.Extensions;
using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Request.Content;
using Apps.Webflow.Models.Response.Content;
using Apps.Webflow.Services;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Blueprints;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using System.Net.Mime;

namespace Apps.Webflow.Actions;

[ActionList]
public class ContentActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) 
    : WebflowInvocable(invocationContext)
{
    private readonly ContentServicesFactory _factory = new ContentServicesFactory(invocationContext);

    [BlueprintActionDefinition(BlueprintAction.SearchContent)]
    [Action("Search content", Description = "Search for any type of content")]
    public async Task<SearchContentResponse> SearchContent(
        [ActionParameter] SiteRequest siteRequest,
        [ActionParameter] SearchContentRequest request,
        [ActionParameter] DateFilter dateFilter)
    {
        var contentServices = _factory.GetContentServices(request.ContentTypes);
        return await contentServices.ExecuteMany(siteRequest, request, dateFilter);
    }

    [BlueprintActionDefinition(BlueprintAction.DownloadContent)]
    [Action("Download content", Description = "Download content as HTML for a specific content type based on its ID")]
    public async Task<DownloadContentResponse> DownloadContent(
        [ActionParameter] SiteRequest siteRequest,
        [ActionParameter] DownloadContentRequest request,
        [ActionParameter] ContentFilter contentFilter)
    {
        var service = _factory.GetContentService(contentFilter.ContentType);
        var stream = await service.DownloadContent(siteRequest, request);
        var fileReference = await fileManagementClient.UploadAsync(stream, MediaTypeNames.Text.Html, $"page_{request.ContentId}.html");
        return new DownloadContentResponse(fileReference);
    }
}
