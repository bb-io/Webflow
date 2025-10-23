using Apps.Webflow.HtmlConversion;
using Apps.Webflow.HtmlConversion.Constants;
using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Entities;
using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Request.Pages;
using Apps.Webflow.Models.Response.Pages;
using Apps.Webflow.Models.Response.Pagination;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using RestSharp;
using System.Web;

namespace Apps.Webflow.Actions;

[ActionList("Pages")]
public class PagesActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : WebflowInvocable(invocationContext)
{
    [Action("Search pages", Description = "Search pages using filters")]
    public async Task<ListPagesResponse> SearchPages(
        [ActionParameter] SiteRequest site,
        [ActionParameter] SearchPagesRequest input,
        [ActionParameter] DateFilter dateFilter)
    {
        var allPages = new List<PageEntity>();
        var offset = 0;
        const int pageSize = 100;
        int total = int.MaxValue;

        while (allPages.Count < total)
        {
            var endpoint = $"sites/{site.SiteId}/pages";
            var request = new RestRequest(endpoint, Method.Get);

            if (!string.IsNullOrEmpty(input.LocaleId))
                request.AddQueryParameter("localeId", input.LocaleId);

            request.AddQueryParameter("offset", offset.ToString());
            request.AddQueryParameter("limit", pageSize.ToString());

            var batch = await Client.ExecuteWithErrorHandling<ListPagesResponse>(request);

            var batchPages = batch.Pages?.ToList() ?? new List<PageEntity>();
            total = batch.Pagination?.Total ?? batchPages.Count;

            allPages.AddRange(batchPages);

            if (batchPages.Count == 0) break;
            offset += batchPages.Count;
        }

        IEnumerable<PageEntity> filtered = allPages;

        if (!string.IsNullOrWhiteSpace(input.TitleContains))
            filtered = filtered.Where(p => !string.IsNullOrEmpty(p.Title) &&
                                           p.Title.Contains(input.TitleContains, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(input.SlugContains))
            filtered = filtered.Where(p => !string.IsNullOrEmpty(p.Slug) &&
                                           p.Slug.Contains(input.SlugContains, StringComparison.OrdinalIgnoreCase));

        if (dateFilter.CreatedAfter.HasValue)
            filtered = filtered.Where(p => p.CreatedOn >= dateFilter.CreatedAfter.Value);

        if (dateFilter.CreatedBefore.HasValue)
            filtered = filtered.Where(p => p.CreatedOn <= dateFilter.CreatedBefore.Value);

        if (dateFilter.LastUpdatedAfter.HasValue)
            filtered = filtered.Where(p => p.LastUpdated >= dateFilter.LastUpdatedAfter.Value);

        if (dateFilter.LastUpdatedBefore.HasValue)
            filtered = filtered.Where(p => p.LastUpdated <= dateFilter.LastUpdatedBefore.Value);

        if (input.Archived.HasValue)
            filtered = filtered.Where(p => p.Archived.HasValue && p.Archived.Value == input.Archived.Value);

        if (input.Draft.HasValue)
            filtered = filtered.Where(p => p.Draft.HasValue && p.Draft.Value == input.Draft.Value);

        var resultPages = filtered.ToList();

        return new ListPagesResponse
        {
            Pages = resultPages,
            Pagination = new PaginationInfo
            {
                Total = resultPages.Count
            }
        };
    }

    [Action("Get page content as HTML", Description = "Get the page content in HTML file")]
    public async Task<GetPageAsHtmlResponse> GetPageAsHtml([ActionParameter] GetPageAsHtmlRequest input)
    {
        var domEndpoint = $"pages/{input.PageId}/dom";
        var domRequest = new RestRequest(domEndpoint, Method.Get);

        if (!string.IsNullOrEmpty(input.LocaleId))
            domRequest.AddQueryParameter("localeId", input.LocaleId);

        var pageDom = await Client.ExecuteWithErrorHandling<PageDomEntity>(domRequest);

        var htmlStream = PageHtmlConverter.ToHtml(pageDom, input.SiteId, input.PageId);

        var fileName = $"page_{input.PageId}.html";
        var contentType = "text/html";

        var fileReference = await fileManagementClient.UploadAsync(htmlStream, contentType, fileName);

        fileReference.Name = fileName;
        fileReference.ContentType = contentType;

        PageEntity? metadata = null;

        if (!input.IncludeMetadata.HasValue || input.IncludeMetadata == true)
        {
            var metadataEndpoint = $"pages/{input.PageId}";
            var metadataRequest = new RestRequest(metadataEndpoint, Method.Get);
            metadata = await Client.ExecuteWithErrorHandling<PageEntity>(metadataRequest);
        }

        return new GetPageAsHtmlResponse(fileReference, metadata);
    }

    [Action("Update page content as HTML", Description = "Update page content using HTML file")]
    public async Task<UpdatePageContentResponse> UpdatePageContentAsHtml([ActionParameter] UpdatePageContentRequest input)
    {
        var fileStream = await fileManagementClient.DownloadAsync(input.File);

        var memoryStream = new MemoryStream();
        await fileStream.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        var doc = new HtmlAgilityPack.HtmlDocument();
        doc.Load(memoryStream);

        if (string.IsNullOrEmpty(input.PageId) || string.IsNullOrEmpty(input.SiteId))
        {
            var metaPageIdNode = doc.DocumentNode.SelectSingleNode("//meta[@name='blackbird-page-id']");
            var metaSiteIdNode = doc.DocumentNode.SelectSingleNode("//meta[@name='blackbird-site-id']");
            if (metaPageIdNode != null && string.IsNullOrEmpty(input.PageId))
            {
                input.PageId = metaPageIdNode.GetAttributeValue("content", string.Empty);
            }
            if (metaSiteIdNode != null && string.IsNullOrEmpty(input.SiteId))
            {
                input.SiteId = metaSiteIdNode.GetAttributeValue("content", string.Empty);
            }
        }

        var elements = doc.DocumentNode
            .Descendants()
            .Where(x => x.NodeType == HtmlAgilityPack.HtmlNodeType.Element &&
                        x.Attributes[ConversionConstants.NodeId] != null)
            .ToList();

        var updateNodes = new List<UpdatePageNode>();

        foreach (var element in elements)
        {
            var nodeId = element.Attributes[ConversionConstants.NodeId].Value;
            var textHtml = HttpUtility.HtmlDecode(element.InnerHtml);

            updateNodes.Add(new UpdatePageNode
            {
                NodeId = nodeId,
                Text = textHtml
            });
        }

        var body = new UpdatePageDomRequest
        {
            LocaleId = input.LocaleId,
            Nodes = updateNodes
        };

        var endpoint = $"pages/{input.PageId}/dom";
        var request = new RestRequest(endpoint, Method.Post)
        {
            RequestFormat = DataFormat.Json
        };

        if (!string.IsNullOrEmpty(input.LocaleId))
            request.AddQueryParameter("localeId", input.LocaleId);

        request.AddJsonBody(body);

        var response = await Client.ExecuteWithErrorHandling<UpdatePageContentResponse>(request);

        return new UpdatePageContentResponse
        {
            Success = true
        };

    }
}