using Apps.Webflow.Constants;
using Apps.Webflow.Helper;
using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Entities.Page;
using Apps.Webflow.Models.Identifiers;
using Apps.Webflow.Models.Request.Content;
using Apps.Webflow.Models.Request.Date;
using Apps.Webflow.Models.Request.Pages;
using Apps.Webflow.Models.Response.Pages;
using Apps.Webflow.Models.Response.Pagination;
using Apps.Webflow.Services;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using RestSharp;

namespace Apps.Webflow.Actions;

[ActionList("Pages")]
public class PagesActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : WebflowInvocable(invocationContext)
{
    private readonly ContentServicesFactory _factory = new(invocationContext, fileManagementClient);

    [Action("Search pages", Description = "Search pages using filters")]
    public async Task<SearchPagesResponse> SearchPages(
        [ActionParameter] SiteIdentifier site,
        [ActionParameter] SearchPagesRequest input,
        [ActionParameter] BasicDateFilter dateFilter)
    {
        ValidatorHelper.ValidateInputDates(dateFilter);

        var request = new RestRequest($"sites/{Client.GetSiteId(site.SiteId)}/pages", Method.Get);

        var allPages = await Client.Paginate<PageEntity, PagesPaginationResponse>(request, r => r.Pages);
        if (allPages.Count == 0)
            return new SearchPagesResponse([]);

        var filtered = ApplySearchPageFilters(allPages, dateFilter, input);
        return new(filtered);
    }

    [Action("Download page", Description = "Download the page content")]
    public async Task<DownloadPageResponse> DownloadPage(
        [ActionParameter] SiteIdentifier site,
        [ActionParameter] DownloadPageRequest input,
        [ActionParameter] LocaleIdentifier locale)
    {
        string fileFormat = input.FileFormat ?? ContentFormats.InteroperableHtml;

        var service = _factory.GetContentService(ContentTypes.Page);
        var request = new DownloadContentRequest
        {
            Locale = locale.Locale,
            ContentId = input.PageId,
            FileFormat = fileFormat,
            IncludeSlug = input.IncludeSlug,
            IncludeMetadata = input.IncludeMetadata,
        };

        var file = await service.DownloadContent(Client.GetSiteId(site.SiteId), request);

        PageEntity? metadata = null;

        if (!input.DisplayMetadata.HasValue || input.DisplayMetadata == true)
        {
            var metadataEndpoint = $"pages/{input.PageId}";
            var metadataRequest = new RestRequest(metadataEndpoint, Method.Get);
            metadata = await Client.ExecuteWithErrorHandling<PageEntity>(metadataRequest);
        }

        return new DownloadPageResponse(file, metadata);
    }

    [Action("Upload page", Description = "Update page content from a file")]
    public async Task UploadPage(
        [ActionParameter] SiteIdentifier site,
        [ActionParameter] UpdatePageContentRequest input,
        [ActionParameter] LocaleIdentifier locale)
    {
        await using var source = await fileManagementClient.DownloadAsync(input.File);
        var bytes = await source.GetByteData();   
        await using var stream = new MemoryStream(bytes);

        var uploadRequest = new UploadContentRequest
        {
            Locale = locale.Locale,
            ContentId = input.PageId
        };

        var service = _factory.GetContentService(ContentTypes.Page);
        await service.UploadContent(stream, Client.GetSiteId(site.SiteId), uploadRequest);
    }

    private static List<PageEntity> ApplySearchPageFilters(List<PageEntity> pages, BasicDateFilter dateFilter, SearchPagesRequest input)
    {
        IEnumerable<PageEntity> filtered = pages;

        filtered = FilterHelper.ApplyBooleanFilter(filtered, input.Draft, p => p.Draft);
        filtered = FilterHelper.ApplyBooleanFilter(filtered, input.Archived, p => p.Archived);
        filtered = FilterHelper.ApplyDateFilters(filtered, dateFilter);
        filtered = FilterHelper.ApplyContainsFilter(filtered, input.TitleContains, p => p.Title);
        filtered = FilterHelper.ApplyContainsFilter(filtered, input.SlugContains, p => p.Slug);
        filtered = FilterHelper.ApplyContainsFilter(filtered, input.PublishedPathContains, p => p.PublishedPath);

        return filtered.ToList();
    }
}