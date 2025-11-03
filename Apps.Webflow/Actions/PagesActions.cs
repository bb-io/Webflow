using Apps.Webflow.Constants;
using Apps.Webflow.Helper;
using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Entities;
using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Request.Content;
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
using System.Text;

namespace Apps.Webflow.Actions;

[ActionList("Pages")]
public class PagesActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : WebflowInvocable(invocationContext)
{
    private readonly ContentServicesFactory _factory = new ContentServicesFactory(invocationContext);

    [Action("Search pages", Description = "Search pages using filters")]
    public async Task<SearchPagesResponse> SearchPages(
        [ActionParameter] SiteRequest site,
        [ActionParameter] SearchPagesRequest input,
        [ActionParameter] DateFilter dateFilter)
    {
        ValidatorHelper.ValidateInputDates(dateFilter);

        var endpoint = $"sites/{Client.GetSiteId(site.SiteId)}/pages";
        var request = new RestRequest(endpoint, Method.Get);

        var allPages = await Client.Paginate<PageEntity, PagesPaginationResponse>(request, r => r.Pages);
        if (!allPages.Any())
            return new SearchPagesResponse(new List<PageEntity>());

        IEnumerable<PageEntity> filtered = allPages;

        if (!string.IsNullOrWhiteSpace(input.TitleContains))
            filtered = filtered.Where(p => !string.IsNullOrEmpty(p.Title) &&
                                           p.Title.Contains(input.TitleContains, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(input.SlugContains))
            filtered = filtered.Where(p => !string.IsNullOrEmpty(p.Slug) &&
                                           p.Slug.Contains(input.SlugContains, StringComparison.OrdinalIgnoreCase));

        filtered = FilterHelper.ApplyDateFilters(filtered, dateFilter);

        if (input.Archived.HasValue)
            filtered = filtered.Where(p => p.Archived.HasValue && p.Archived.Value == input.Archived.Value);

        if (input.Draft.HasValue)
            filtered = filtered.Where(p => p.Draft.HasValue && p.Draft.Value == input.Draft.Value);

        return new SearchPagesResponse(filtered.ToList());
    }

    [Action("Download page", Description = "Get the page content in HTML file")]
    public async Task<DownloadPageResponse> GetPageAsHtml(
        [ActionParameter] SiteRequest site,
        [ActionParameter] DownloadPageRequest input)
    {
        var service = _factory.GetContentService(ContentTypes.Page);
        var request = new DownloadContentRequest
        {
            Locale = input.LocaleId,
            ContentId = input.PageId
        };
        var htmlStream = await service.DownloadContent(Client.GetSiteId(site.SiteId), request);
        var fileReference = await fileManagementClient.UploadAsync(htmlStream, "text/html", $"page_{input.PageId}.html");

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
    public async Task UpdatePageContentAsHtml(
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
}