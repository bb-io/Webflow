using Apps.Webflow.Constants;
using Apps.Webflow.Conversion.Constants;
using Apps.Webflow.Conversion.Page;
using Apps.Webflow.Helper;
using Apps.Webflow.Models.Entities;
using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Request.Content;
using Apps.Webflow.Models.Request.Pages;
using Apps.Webflow.Models.Response.Content;
using Apps.Webflow.Models.Response.Pages;
using Apps.Webflow.Models.Response.Pagination;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;
using System.Web;

namespace Apps.Webflow.Services.Concrete;

public class PageService(InvocationContext invocationContext) : BaseContentService(invocationContext)
{
    private const string ContentType = ContentTypes.Page;

    public override async Task<SearchContentResponse> SearchContent(string siteId, SearchContentRequest input, DateFilter dateFilter)
    {
        ThrowForPublishedDateInputs(input, ContentType);

        ValidatorHelper.ValidateInputDates(dateFilter);

        var endpoint = $"sites/{siteId}/pages";
        var request = new RestRequest(endpoint, Method.Get);

        var pages = await Client.Paginate<PageEntity, PagesPaginationResponse>(request, r => r.Pages);

        IEnumerable<PageEntity> filtered = FilterHelper.ApplyDateFilters(pages, dateFilter);
        filtered = FilterHelper.ApplyContainsFilter(filtered, input.NameContains, r => r.Title);

        var result = filtered.Select(x => new ContentItemEntity
        {
            ContentId = x.Id,
            Name = x.Title,
            Type = ContentType
        });

        return new SearchContentResponse(result);
    }

    public override async Task<Stream> DownloadContent(string siteId, DownloadContentRequest input)
    {
        var domEndpoint = $"pages/{input.ContentId}/dom";
        var domRequest = new RestRequest(domEndpoint, Method.Get);

        if (!string.IsNullOrEmpty(input.Locale))
            domRequest.AddQueryParameter("localeId", input.Locale);

        var pageDom = await Client.ExecuteWithErrorHandling<PageDomEntity>(domRequest);

        Stream outputStream = input.FileFormat switch
        {
            "text/html" => PageHtmlConverter.ToHtml(pageDom, siteId, input.ContentId, input.Locale),
            "original" => PageJsonConverter.ToJson(pageDom, siteId, input.Locale),
            _ => throw new PluginMisconfigurationException($"Unsupported output format: {input.FileFormat}")
        };

        var memoryStream = new MemoryStream();
        await outputStream.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        return memoryStream;
    }
    
    public override async Task UploadContent(Stream content, string siteId, UploadContentRequest input)
    {
        var memoryStream = new MemoryStream();
        await content.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        var doc = new HtmlAgilityPack.HtmlDocument();
        doc.Load(memoryStream);

        if (string.IsNullOrEmpty(input.ContentId))
        {
            var metaPageIdNode = doc.DocumentNode.SelectSingleNode("//meta[@name='blackbird-page-id']");

            if (metaPageIdNode != null && string.IsNullOrEmpty(input.ContentId))
                input.ContentId = metaPageIdNode.GetAttributeValue("content", string.Empty);
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
            LocaleId = input.Locale,
            Nodes = updateNodes
        };

        var endpoint = $"pages/{input.ContentId}/dom";
        var request = new RestRequest(endpoint, Method.Post) { RequestFormat = DataFormat.Json };

        if (!string.IsNullOrEmpty(input.Locale))
            request.AddQueryParameter("localeId", input.Locale);

        request.AddJsonBody(body);

        await Client.ExecuteWithErrorHandling(request);
    }
}
