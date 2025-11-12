using Apps.Webflow.Constants;
using Apps.Webflow.Conversion.Constants;
using Apps.Webflow.Conversion.Models;
using Apps.Webflow.Conversion.Page;
using Apps.Webflow.Extensions;
using Apps.Webflow.Helper;
using Apps.Webflow.Models.Entities;
using Apps.Webflow.Models.Request.Content;
using Apps.Webflow.Models.Request.Date;
using Apps.Webflow.Models.Request.Pages;
using Apps.Webflow.Models.Response.Content;
using Apps.Webflow.Models.Response.Pages;
using Apps.Webflow.Models.Response.Pagination;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Filters.Transformations;
using Blackbird.Filters.Xliff.Xliff2;
using Newtonsoft.Json;
using RestSharp;
using System.Net.Mime;
using System.Text;
using System.Web;

namespace Apps.Webflow.Services.Concrete;

public class PageService(InvocationContext invocationContext, IFileManagementClient fileManagementClient) 
    : BaseContentService(invocationContext)
{
    private const string ContentType = ContentTypes.Page;

    public override async Task<SearchContentResponse> SearchContent(string siteId, SearchContentRequest input, ContentDateFilter dateFilter)
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

    public override async Task<FileReference> DownloadContent(string siteId, DownloadContentRequest input)
    {
        var domRequest = new RestRequest($"pages/{input.ContentId}/dom", Method.Get);

        if (!string.IsNullOrEmpty(input.Locale))
        {
            var localeId = await LocaleHelper.GetLocaleId(input.Locale, siteId, Client);
            domRequest.AddQueryParameter("localeId", localeId);
        }

        var pageDom = await Client.ExecuteWithErrorHandling<PageDomEntity>(domRequest);

        var pageRequest = new RestRequest($"pages/{pageDom.PageId}", Method.Get);
        var page = await Client.ExecuteWithErrorHandling<PageEntity>(pageRequest);

        Stream outputStream = input.FileFormat switch
        {
            "text/html" => PageHtmlConverter.ToHtml(pageDom, siteId, input.ContentId, page.Title, input.Locale),
            "original" => PageJsonConverter.ToJson(pageDom, siteId, page.Title, input.Locale),
            _ => throw new PluginMisconfigurationException($"Unsupported output format: {input.FileFormat}")
        };

        string name = page.Title ?? page.Id;
        string contentType = input.FileFormat == "text/html" ? MediaTypeNames.Text.Html : MediaTypeNames.Application.Json;
        var fileName = FileHelper.GetDownloadedFileName(name, contentType);

        FileReference fileReference = await fileManagementClient.UploadAsync(outputStream, contentType, fileName);
        await outputStream.DisposeAsync();
        return fileReference;
    }

    public override async Task UploadContent(Stream content, string siteId, UploadContentRequest input)
    {
        using var memoryStream = new MemoryStream();
        await content.CopyToAsync(memoryStream);
        string fileText = Encoding.UTF8.GetString(memoryStream.ToArray());

        if (JsonHelper.IsJson(fileText))
        {
            await UploadJsonContent(fileText, siteId, input);
        }
        else
        {
            if (Xliff2Serializer.IsXliff2(fileText))
            {
                var htmlFromXliff = Transformation.Parse(fileText, "page.xlf").Target().Serialize()
                    ?? throw new PluginMisconfigurationException("XLIFF did not contain valid content.");

                memoryStream.SetLength(0);
                await memoryStream.WriteAsync(Encoding.UTF8.GetBytes(htmlFromXliff));
            }

            memoryStream.Position = 0;
            await UploadHtmlContent(memoryStream, siteId, input);
        }
    }
    
    private async Task UploadHtmlContent(MemoryStream htmlStream, string siteId, UploadContentRequest input)
    {
        var doc = new HtmlAgilityPack.HtmlDocument();
        doc.Load(htmlStream);

        input.ContentId ??= doc.DocumentNode.GetMetaValue("blackbird-page-id");
        input.Locale ??= doc.DocumentNode.GetMetaValue("blackbird-locale-id");

        await ValidateAndNormalizeInputs(input, siteId);

        var elements = doc.DocumentNode
            .Descendants()
            .Where(x => x.NodeType == HtmlAgilityPack.HtmlNodeType.Element &&
                        x.Attributes[ConversionConstants.NodeId] != null)
            .ToList();

        var updateNodes = new List<UpdatePageNode>();
        foreach (var element in elements)
        {
            updateNodes.Add(new UpdatePageNode
            {
                NodeId = element.Attributes[ConversionConstants.NodeId].Value,
                Text = HttpUtility.HtmlDecode(element.InnerHtml).Trim()
            });
        }

        await PatchPageDom(input.ContentId!, input.Locale!, updateNodes);
    }

    private async Task UploadJsonContent(string jsonContent, string siteId, UploadContentRequest input)
    {
        var downloadedPage = JsonConvert.DeserializeObject<DownloadedPage>(jsonContent)
            ?? throw new PluginMisconfigurationException("Invalid JSON file format.");

        input.ContentId ??= downloadedPage.Page.PageId;
        input.Locale ??= downloadedPage.Locale;

        await ValidateAndNormalizeInputs(input, siteId);

        var updateNodes = downloadedPage.Page.Nodes.Select(n => new UpdatePageNode
        {
            NodeId = n.Id,
            Text = n.Text?.Html
        });

        await PatchPageDom(input.ContentId!, input.Locale!, updateNodes);
    }

    private async Task ValidateAndNormalizeInputs(UploadContentRequest input, string siteId)
    {
        if (string.IsNullOrWhiteSpace(input.ContentId))
            throw new PluginMisconfigurationException("Page ID is missing. Provide it in the input or file.");

        if (string.IsNullOrWhiteSpace(input.Locale))
            throw new PluginMisconfigurationException("Locale is missing. Provide it in the input or file.");

        input.Locale = await LocaleHelper.GetLocaleId(input.Locale, siteId, Client);
    }

    private async Task PatchPageDom(string pageId, string localeId, IEnumerable<UpdatePageNode> nodes)
    {
        var body = new UpdatePageDomRequest { Nodes = nodes };

        var endpoint = $"pages/{pageId}/dom";
        var request = new RestRequest(endpoint, Method.Post)
            .WithJsonBody(body)
            .AddQueryParameter("localeId", localeId);

        request.RequestFormat = DataFormat.Json;
        await Client.ExecuteWithErrorHandling(request);
    }
}
