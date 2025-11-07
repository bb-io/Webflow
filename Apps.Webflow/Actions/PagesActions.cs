using Apps.Webflow.Constants;
using Apps.Webflow.Helper;
using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Entities;
using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Request.Content;
using Apps.Webflow.Models.Request.Date;
using Apps.Webflow.Models.Request.Pages;
using Apps.Webflow.Models.Response.Pages;
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

[ActionList("Pages")]
public class PagesActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : WebflowInvocable(invocationContext)
{
    private readonly ContentServicesFactory _factory = new(invocationContext);

    [Action("Search pages", Description = "Search pages using filters")]
    public async Task<SearchPagesResponse> SearchPages(
        [ActionParameter] SiteRequest site,
        [ActionParameter] SearchPagesRequest input,
        [ActionParameter] BasicDateFilter dateFilter)
    {
        ValidatorHelper.ValidateInputDates(dateFilter);

        var endpoint = $"sites/{Client.GetSiteId(site.SiteId)}/pages";
        var request = new RestRequest(endpoint, Method.Get);

        var allPages = await Client.Paginate<PageEntity, PagesPaginationResponse>(request, r => r.Pages);
        if (allPages.Count == 0)
            return new SearchPagesResponse([]);

        var filtered = ApplyPageFilters(allPages, dateFilter, input);
        return new SearchPagesResponse(filtered);
    }

    [Action("Download page", Description = "Get the page content in HTML file")]
    public async Task<DownloadPageResponse> DownloadPage(
        [ActionParameter] SiteRequest site,
        [ActionParameter] DownloadPageRequest input)
    {
        string fileFormat = input.FileFormat ?? MediaTypeNames.Text.Html;

        var service = _factory.GetContentService(ContentTypes.Page);
        var request = new DownloadContentRequest
        {
            Locale = input.LocaleId,
            ContentId = input.PageId,
            FileFormat = fileFormat,
        };

        var stream = await service.DownloadContent(Client.GetSiteId(site.SiteId), request);
        string fileName = FileHelper.GetDownloadedFileName(fileFormat, input.PageId, ContentTypes.Page);
        string contentType = fileFormat == MediaTypeNames.Text.Html ? MediaTypeNames.Text.Html : MediaTypeNames.Application.Json;

        var fileReference = await fileManagementClient.UploadAsync(stream, contentType, fileName);

        PageEntity? metadata = null;

        if (!input.IncludeMetadata.HasValue || input.IncludeMetadata == true)
        {
            var metadataEndpoint = $"pages/{input.PageId}";
            var metadataRequest = new RestRequest(metadataEndpoint, Method.Get);
            metadata = await Client.ExecuteWithErrorHandling<PageEntity>(metadataRequest);
        }

        return new DownloadPageResponse(fileReference, metadata);
    }

    [Action("Upload page", Description = "Update page content using HTML file")]
    public async Task UploadPage(
        [ActionParameter] SiteRequest site,
        [ActionParameter] UpdatePageContentRequest input)
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

        if (string.IsNullOrEmpty(input.PageId))
        {
            var metaPageIdNode = doc.DocumentNode.SelectSingleNode("//meta[@name='blackbird-page-id']");

            if (metaPageIdNode != null && string.IsNullOrEmpty(input.PageId))
                input.PageId = metaPageIdNode.GetAttributeValue("content", string.Empty);
        }

        if (string.IsNullOrEmpty(input.LocaleId))
        {
            var localeIdNode = doc.DocumentNode.SelectSingleNode("//meta[@name='blackbird-locale-id']");

            if (localeIdNode != null && string.IsNullOrEmpty(input.LocaleId))
                input.LocaleId = localeIdNode.GetAttributeValue("content", string.Empty);
        }

        if (string.IsNullOrEmpty(input.PageId))
            throw new PluginMisconfigurationException("Page ID was not found in the file. Please specify it in the input value");
        if (string.IsNullOrEmpty(input.LocaleId))
            throw new PluginMisconfigurationException("Locale ID was not found in the file. Please specify it in the input value");

        var uploadRequest = new UploadContentRequest
        {
            ContentId = input.PageId,
            Locale = input.LocaleId
        };

        var service = _factory.GetContentService(ContentTypes.Page);
        await service.UploadContent(ms, site.SiteId, uploadRequest);
    }

    private static List<PageEntity> ApplyPageFilters(List<PageEntity> pages, BasicDateFilter dateFilter, SearchPagesRequest input)
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