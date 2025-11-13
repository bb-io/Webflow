using Apps.Webflow.Constants;
using Apps.Webflow.Conversion.Component;
using Apps.Webflow.Conversion.Constants;
using Apps.Webflow.Conversion.Models;
using Apps.Webflow.Extensions;
using Apps.Webflow.Helper;
using Apps.Webflow.Models.Entities.Component;
using Apps.Webflow.Models.Entities.Content;
using Apps.Webflow.Models.Request.Components;
using Apps.Webflow.Models.Request.Content;
using Apps.Webflow.Models.Request.Date;
using Apps.Webflow.Models.Response.Components;
using Apps.Webflow.Models.Response.Content;
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

public class ComponentService(InvocationContext invocationContext, IFileManagementClient fileManagementClient) 
    : BaseContentService(invocationContext)
{
    private const string ContentType = ContentTypes.Component;

    public override async Task<SearchContentResponse> SearchContent(string siteId, SearchContentRequest input, ContentDateFilter dateFilter)
    {
        ThrowForDateInputs(dateFilter, ContentType);
        ThrowForPublishedDateInputs(input, ContentType);

        var endpoint = $"sites/{siteId}/components";
        var request = new RestRequest(endpoint, Method.Get);

        IEnumerable<ComponentEntity> pages = await Client.Paginate<ComponentEntity, ComponentsPaginationResponse>(request, r => r.Components);
        pages = FilterHelper.ApplyContainsFilter(pages, input.NameContains, r => r.Name);

        var result = pages.Select(x => new ContentItemEntity
        {
            ContentId = x.Id,
            Name = x.Name,
            Type = ContentType
        });

        return new SearchContentResponse(result);
    }

    public override async Task<FileReference> DownloadContent(string siteId, DownloadContentRequest input)
    {
        var domRequest = new RestRequest($"sites/{siteId}/components/{input.ContentId}/dom", Method.Get);

        if (!string.IsNullOrEmpty(input.Locale))
        {
            var localeId = await LocaleHelper.GetLocaleId(input.Locale, siteId, Client);
            domRequest.AddQueryParameter("localeId", localeId);
        }

        var componentDom = await Client.ExecuteWithErrorHandling<ComponentDomEntity>(domRequest);

        Stream outputStream = input.FileFormat switch
        {
            ContentFormats.InteroperableHtml => ComponentHtmlConverter.ToHtml(
                componentDom, 
                siteId,
                input.ContentId,
                input.Locale
            ),
            ContentFormats.OriginalJson => ComponentJsonConverter.ToJson(componentDom, siteId, input.Locale),
            _ => throw new PluginMisconfigurationException($"Unsupported output format: {input.FileFormat}")
        };

        var componentRequest = new RestRequest($"sites/{siteId}/components", Method.Get);
        var components = await Client.Paginate<ComponentEntity, ComponentsPaginationResponse>(componentRequest, c => c.Components);
        var component = components.FirstOrDefault(c => c.Id == input.ContentId);

        string name = component?.Name ?? input.ContentId;
        string contentFormat = input.FileFormat == "text/html" ? MediaTypeNames.Text.Html : MediaTypeNames.Application.Json;
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
                var htmlFromXliff = Transformation.Parse(fileText, "component.xlf").Target().Serialize()
                    ?? throw new PluginMisconfigurationException("XLIFF did not contain valid content.");

                memoryStream.SetLength(0);
                await memoryStream.WriteAsync(Encoding.UTF8.GetBytes(htmlFromXliff));
            }

            memoryStream.Position = 0;
            await UploadHtmlContent(memoryStream, siteId, input);
        }
    }

    private async Task UploadHtmlContent(Stream htmlStream, string siteId, UploadContentRequest input)
    {
        var doc = new HtmlAgilityPack.HtmlDocument();
        doc.Load(htmlStream);

        input.Locale ??= doc.DocumentNode.GetMetaValue("blackbird-locale");
        input.ContentId ??= doc.DocumentNode.GetMetaValue("blackbird-component-id");

        await ValidateAndNormalizeInputs(input, siteId);

        var elements = doc.DocumentNode
            .Descendants()
            .Where(x => x.NodeType == HtmlAgilityPack.HtmlNodeType.Element &&
                        x.Attributes[ConversionConstants.NodeId] != null)
            .ToList();

        var updateNodes = new List<UpdateComponentNode>();
        foreach (var element in elements)
        {
            var nodeId = element.Attributes[ConversionConstants.NodeId].Value;
            var propertyId = element.Attributes[ConversionConstants.PropertyId]?.Value;
            var textHtml = HttpUtility.HtmlDecode(element.InnerHtml).Trim();

            if (!string.IsNullOrEmpty(propertyId))
            {
                var existingNode = updateNodes.FirstOrDefault(n => n.NodeId == nodeId);
                if (existingNode == null)
                {
                    existingNode = new UpdateComponentNode { NodeId = nodeId, PropertyOverrides = [] };
                    updateNodes.Add(existingNode);
                }
                existingNode.PropertyOverrides?.Add(new ComponentPropertyOverride { PropertyId = propertyId, Text = textHtml });
            }
            else
            {
                updateNodes.Add(new UpdateComponentNode { NodeId = nodeId, Text = textHtml });
            }
        }

        await PatchComponentDom(siteId, input.ContentId!, input.Locale!, updateNodes);
    }

    private async Task UploadJsonContent(string jsonContent, string siteId, UploadContentRequest input)
    {
        var downloadedComponent = JsonConvert.DeserializeObject<DownloadedComponent>(jsonContent)
            ?? throw new PluginMisconfigurationException("Invalid JSON file format.");

        input.Locale ??= downloadedComponent.Locale;
        input.ContentId ??= downloadedComponent.Component.ComponentId;

        await ValidateAndNormalizeInputs(input, siteId);

        var updateNodes = downloadedComponent.Component.Nodes.Select(n => new UpdateComponentNode
        {
            NodeId = n.Id,
            Text = n.Text?.Html,
            PropertyOverrides = n.PropertyOverrides?.Select(p => new ComponentPropertyOverride
            {
                PropertyId = p.PropertyId,
                Text = p.Text.Html
            }).ToList()
        });

        await PatchComponentDom(siteId, input.ContentId!, input.Locale!, updateNodes);
    }

    private async Task ValidateAndNormalizeInputs(UploadContentRequest input, string siteId)
    {
        if (string.IsNullOrWhiteSpace(input.ContentId))
            throw new PluginMisconfigurationException("Component ID is missing. Provide it in the input or file.");

        if (string.IsNullOrWhiteSpace(input.Locale))
            throw new PluginMisconfigurationException("Locale is missing. Provide it in the input or file.");

        input.Locale = await LocaleHelper.GetLocaleId(input.Locale, siteId, Client);
    }

    private async Task PatchComponentDom(string siteId, string contentId, string localeId, IEnumerable<UpdateComponentNode> nodes)
    {
        var body = new UpdateComponentDomRequest { Nodes = nodes };
        var endpoint = $"sites/{siteId}/components/{contentId}/dom";

        var apiRequest = new RestRequest(endpoint, Method.Post)
            .WithJsonBody(body)
            .AddQueryParameter("localeId", localeId);

        apiRequest.RequestFormat = DataFormat.Json;
        await Client.ExecuteWithErrorHandling(apiRequest);
    }
}
