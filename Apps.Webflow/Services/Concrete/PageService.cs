using Apps.Webflow.Constants;
using Apps.Webflow.Conversion.Constants;
using Apps.Webflow.Conversion.Models;
using Apps.Webflow.Conversion.Page;
using Apps.Webflow.Extensions;
using Apps.Webflow.Helper;
using Apps.Webflow.Models.Entities.Content;
using Apps.Webflow.Models.Entities.Page;
using Apps.Webflow.Models.Entities.Site;
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
using HtmlAgilityPack;
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
        string localeIdToUse = string.Empty;
        bool shouldSendLocaleParam = false;

        if (!string.IsNullOrEmpty(input.Locale))
        {
            var locale = await LocaleHelper.GetLocale(input.Locale, siteId, Client);
            localeIdToUse = locale.Id;
            shouldSendLocaleParam = !locale.IsPrimary;
        }

        var domRequest = new RestRequest($"pages/{input.ContentId}/dom", Method.Get);
        if (shouldSendLocaleParam)
            domRequest.AddQueryParameter("localeId", localeIdToUse);

        var pageDom = await Client.ExecuteWithErrorHandling<PageDomEntity>(domRequest);

        var pageRequest = new RestRequest($"pages/{pageDom.PageId}", Method.Get);
        if (shouldSendLocaleParam)
            pageRequest.AddQueryParameter("localeId", localeIdToUse);

        var page = await Client.ExecuteWithErrorHandling<PageEntity>(pageRequest);
        string? slug = input.IncludeSlug == true ? page.Slug : null;

        PageMetadata? metadata = null;
        if (input.IncludeMetadata == true)
        {
            var openGraphMetadata = page.OpenGraph;
            if (openGraphMetadata?.TitleCopied == true)
                openGraphMetadata.Title = null;
            if (openGraphMetadata?.DescriptionCopied == true)
                openGraphMetadata.Description = null;

            metadata = new PageMetadata(page.Title, slug, page.Seo, openGraphMetadata);
        }
        else
            metadata = new PageMetadata { Slug = slug };

        Stream outputStream = input.FileFormat switch
        {
            ContentFormats.InteroperableHtml => PageHtmlConverter.ToHtml(
                pageDom, 
                siteId,
                input.ContentId,
                input.Locale, 
                metadata
            ),
            ContentFormats.OriginalJson => PageJsonConverter.ToJson(pageDom, siteId, input.Locale, metadata),
            _ => throw new PluginMisconfigurationException($"Unsupported output format: {input.FileFormat}")
        };

        string name = page.Title ?? page.Id;
        string contentFormat = 
            input.FileFormat == ContentFormats.InteroperableHtml 
            ? MediaTypeNames.Text.Html 
            : MediaTypeNames.Application.Json;
        var fileName = FileHelper.GetDownloadedFileName(ContentType, input.ContentId, name, contentFormat);

        FileReference fileReference = await fileManagementClient.UploadAsync(outputStream, contentFormat, fileName);
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
        var doc = new HtmlDocument();
        doc.Load(htmlStream);

        input.ContentId ??= doc.DocumentNode.GetMetaValue("blackbird-page-id");
        input.Locale ??= doc.DocumentNode.GetMetaValue("blackbird-locale");

        var localeInfo = await ValidateAndNormalizeInputs(input, siteId);
        if (localeInfo.IsPrimary)
        {
            throw new PluginMisconfigurationException(
                "Webflow does not allow updating the content (DOM) of the primary locale via API. " +
                "Please edit the content directly in the Webflow Designer."
            );
        }

        var metadata = ParseTranslatableMetadata(doc);
        if (metadata is not null)
            await PatchPageMetadataAsync(input.ContentId!, input.Locale!, metadata);

        var nodes = doc.DocumentNode
            .Descendants()
            .Where(x => x.NodeType == HtmlNodeType.Element &&
                        x.Attributes[ConversionConstants.NodeId] is not null)
            .GroupBy(x => x.Attributes[ConversionConstants.NodeId].Value)
            .ToList();

        var updateNodes = new List<UpdatePageNode>();

        foreach (var node in nodes)
        {
            var updateNode = new UpdatePageNode { NodeId = node.Key };

            foreach (var element in node)
            {    
                var content = HttpUtility.HtmlDecode(element.InnerHtml).Trim();
                var propertyIdAttr = element.Attributes[ConversionConstants.PropertyId]?.Value;

                if (string.IsNullOrEmpty(content))
                    continue;

                // Check if this is a placeholder
                if (element.Attributes["data-node-placeholder"] != null)
                {
                    updateNode.Placeholder = content;
                    continue;
                }

                if (string.IsNullOrEmpty(propertyIdAttr))
                {
                    updateNode.Text = content; // it's a text node
                    continue;
                }

                // if we are here, it's a property override
                var propertyOverride = new PropertyOverride
                {
                    PropertyId = propertyIdAttr,
                    Text = content
                };

                updateNode.PropertyOverrides ??= [];
                updateNode.PropertyOverrides = updateNode.PropertyOverrides.Append(propertyOverride);
            }

            updateNodes.Add(updateNode);
        }

        await PatchPageDom(input.ContentId!, input.Locale!, updateNodes);
    }

    private async Task UploadJsonContent(string jsonContent, string siteId, UploadContentRequest input)
    {
        var downloadedPage = JsonConvert.DeserializeObject<DownloadedPage>(jsonContent)
            ?? throw new PluginMisconfigurationException("Invalid JSON file format.");

        input.ContentId ??= downloadedPage.Page.PageId;
        input.Locale ??= downloadedPage.Locale;

        var localeInfo = await ValidateAndNormalizeInputs(input, siteId);

        if (downloadedPage.Metadata != null)
            await PatchPageMetadataAsync(input.ContentId, input.Locale!, downloadedPage.Metadata);

        var updateNodes = downloadedPage.Page.Nodes.Select(n => new UpdatePageNode
        {
            NodeId = n.Id,
            Text = n.Text?.Html,
            Placeholder = n.Placeholder
        });

        await PatchPageDom(input.ContentId!, input.Locale!, updateNodes);
    }

    private static PageMetadata ParseTranslatableMetadata(HtmlDocument doc)
    {
        var body = doc.DocumentNode.SelectSingleNode("//body");

        string GetNodeText(string id)
        {
            return body?.SelectSingleNode($"descendant::div[@id='{id}']")?.InnerHtml?.Trim() ?? string.Empty;
        }

        bool GetAttributeBool(string id, string attribute)
        {
            var node = body?.SelectSingleNode($"descendant::div[@id='{id}']");
            return node?.GetAttributeValue(attribute, "false") == "true";
        }

        string pageTitle = GetNodeText("blackbird-page-title");
        string slug = GetNodeText("blackbird-page-slug");

        var seo = new PageSeo
        {
            Title = GetNodeText("blackbird-seo-title"),
            Description = GetNodeText("blackbird-seo-description")
        };

        var openGraph = new PageOpenGraph
        {
            Title = GetNodeText("blackbird-opengraph-title"),
            TitleCopied = GetAttributeBool("blackbird-opengraph-title", "data-copied"),
            Description = GetNodeText("blackbird-opengraph-description"),
            DescriptionCopied = GetAttributeBool("blackbird-opengraph-description", "data-copied")
        };

        return new(pageTitle, slug, seo, openGraph);
    }

    private async Task<SiteLocale> ValidateAndNormalizeInputs(UploadContentRequest input, string siteId)
    {
        if (string.IsNullOrWhiteSpace(input.ContentId))
            throw new PluginMisconfigurationException("Page ID is missing. Provide it in the input or file.");

        if (string.IsNullOrWhiteSpace(input.Locale))
            throw new PluginMisconfigurationException("Locale is missing. Provide it in the input or file.");

        var locale = await LocaleHelper.GetLocale(input.Locale, siteId, Client);
        input.Locale = locale.Id;
        return locale;
    }

    private async Task PatchPageDom(string pageId, string localeId, IEnumerable<UpdatePageNode> nodes)
    {
        if (nodes == null! || !nodes.Any()) 
            return;

        var body = new UpdatePageDomRequest { Nodes = nodes };

        var endpoint = $"pages/{pageId}/dom";
        var request = new RestRequest(endpoint, Method.Post)
            .WithJsonBody(body, JsonConfig.Settings)
            .AddQueryParameter("localeId", localeId);

        // TODO Log errors, right now payload validation errors with 200 http code are not visible
        await Client.ExecuteWithErrorHandling(request);
    }

    private async Task PatchPageMetadataAsync(string pageId, string localeId, PageMetadata metadata)
    {
        string? openGraphTitle = metadata.OpenGraph?.TitleCopied == true ? null : metadata.OpenGraph?.Title;
        string? openGraphDescription = metadata.OpenGraph?.DescriptionCopied == true ? null : metadata.OpenGraph?.Description;
        string? title = string.IsNullOrEmpty(metadata.PageTitle) ? null : metadata.PageTitle;
        string? slug = string.IsNullOrEmpty(metadata.Slug) ? null : metadata.Slug;

        var payload = new
        {
            title,
            slug,
            seo = metadata.Seo,
            openGraph = new
            {
                title = openGraphTitle,
                titleCopied = metadata.OpenGraph?.TitleCopied,
                description = openGraphDescription,
                descriptionCopied = metadata.OpenGraph?.DescriptionCopied
            }
        };

        var request = new RestRequest($"pages/{pageId}", Method.Put)
            .WithJsonBody(payload, JsonConfig.Settings)
            .AddQueryParameter("localeId", localeId);

        await Client.ExecuteWithErrorHandling(request);
    }
}
