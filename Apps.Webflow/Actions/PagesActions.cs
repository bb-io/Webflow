using Apps.Webflow.Api;
using Apps.Webflow.HtmlConversion;
using Apps.Webflow.HtmlConversion.Constants;
using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Request.Pages;
using Apps.Webflow.Models.Response;
using Apps.Webflow.Models.Response.Pages;
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
    public async Task<ListPagesResponse> SearchPages([ActionParameter] SearchPagesRequest input)
    {
        var allPages = new List<PageResponse>();
        var offset = 0;
        const int pageSize = 100;
        int total = int.MaxValue;

        while (allPages.Count < total)
        {
            var endpoint = $"sites/{input.SiteId}/pages";
            var request = new WebflowRequest(endpoint, Method.Get, Creds);

            if (!string.IsNullOrEmpty(input.LocaleId))
                request.AddQueryParameter("localeId", input.LocaleId);

            request.AddQueryParameter("offset", offset.ToString());
            request.AddQueryParameter("limit", pageSize.ToString());

            var batch = await Client.ExecuteWithErrorHandling<ListPagesResponse>(request);

            var batchPages = batch.Pages?.ToList() ?? new List<PageResponse>();
            total = batch.Pagination?.Total ?? batchPages.Count;

            allPages.AddRange(batchPages);

            if (batchPages.Count == 0) break;
            offset += batchPages.Count;
        }

        IEnumerable<PageResponse> filtered = allPages;

        if (!string.IsNullOrWhiteSpace(input.TitleContains))
            filtered = filtered.Where(p => !string.IsNullOrEmpty(p.Title) &&
                                           p.Title.Contains(input.TitleContains, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(input.SlugContains))
            filtered = filtered.Where(p => !string.IsNullOrEmpty(p.Slug) &&
                                           p.Slug.Contains(input.SlugContains, StringComparison.OrdinalIgnoreCase));

        if (input.CreatedAfter.HasValue)
            filtered = filtered.Where(p => p.CreatedOn >= input.CreatedAfter.Value);

        if (input.CreatedBefore.HasValue)
            filtered = filtered.Where(p => p.CreatedOn <= input.CreatedBefore.Value);

        if (input.LastUpdatedAfter.HasValue)
            filtered = filtered.Where(p => p.LastUpdated >= input.LastUpdatedAfter.Value);

        if (input.LastUpdatedBefore.HasValue)
            filtered = filtered.Where(p => p.LastUpdated <= input.LastUpdatedBefore.Value);

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
        var domRequest = new WebflowRequest(domEndpoint, Method.Get, Creds);

        if (!string.IsNullOrEmpty(input.LocaleId))
            domRequest.AddQueryParameter("localeId", input.LocaleId);

        var pageDom = await Client.ExecuteWithErrorHandling<PageDomEntity>(domRequest);

        var htmlStream = PageHtmlConverter.ToHtml(pageDom, input.SiteId, input.PageId);

        var fileName = $"page_{input.PageId}.html";
        var contentType = "text/html";

        var fileReference = await fileManagementClient.UploadAsync(htmlStream, contentType, fileName);

        fileReference.Name = fileName;
        fileReference.ContentType = contentType;

        PageResponse? metadata = null;

        if (!input.IncludeMetadata.HasValue || input.IncludeMetadata == true)
        {
            var metadataEndpoint = $"pages/{input.PageId}";
            var metadataRequest = new WebflowRequest(metadataEndpoint, Method.Get, Creds);
            metadata = await Client.ExecuteWithErrorHandling<PageResponse>(metadataRequest);
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
        var request = new WebflowRequest(endpoint, Method.Post, Creds)
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