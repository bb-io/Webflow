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
            ContentFormats.InteroperableHtml => await ComponentHtmlConverter.ToHtml(
                componentDom, 
                siteId,
                input.ContentId,
                input.Locale,
                Client
            ),
            ContentFormats.OriginalJson => await ComponentJsonConverter.ToJson(componentDom, siteId, input.Locale, Client),
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

        // Extract properties (elements with data-property-id attribute only, no data-node-id)
        var propertyElements = doc.DocumentNode
            .Descendants()
            .Where(x => x.NodeType == HtmlAgilityPack.HtmlNodeType.Element &&
                        x.Attributes[ConversionConstants.PropertyIdAttr] != null &&
                        x.Attributes[ConversionConstants.NodeId] == null)
            .ToList();

        var updateProperties = new List<UpdateComponentProperty>();
        foreach (var element in propertyElements)
        {
            var propertyId = element.Attributes[ConversionConstants.PropertyIdAttr].Value;
            var textHtml = HttpUtility.HtmlDecode(element.InnerHtml).Trim();
            updateProperties.Add(new UpdateComponentProperty { PropertyId = propertyId, Text = textHtml });
        }

        // Update properties if any
        if (updateProperties.Any())
        {
            await PatchComponentProperties(siteId, input.ContentId!, input.Locale!, updateProperties);
        }

        // Extract nodes (elements with data-node-id attribute)
        var nodeElements = doc.DocumentNode
            .Descendants()
            .Where(x => x.NodeType == HtmlAgilityPack.HtmlNodeType.Element &&
                        x.Attributes[ConversionConstants.NodeId] != null)
            .ToList();

        var nodeGroups = nodeElements.GroupBy(x => x.Attributes[ConversionConstants.NodeId].Value);
        var updateNodes = new List<UpdateComponentNode>();

        foreach (var nodeGroup in nodeGroups)
        {
            var nodeId = nodeGroup.Key;
            var node = new UpdateComponentNode { NodeId = nodeId };

            foreach (var element in nodeGroup)
            {
                var propertyId = element.Attributes[ConversionConstants.PropertyId]?.Value;
                var textHtml = HttpUtility.HtmlDecode(element.InnerHtml).Trim();

                // Check if this is a property override
                if (!string.IsNullOrEmpty(propertyId))
                {
                    node.PropertyOverrides ??= new List<ComponentPropertyOverride>();
                    node.PropertyOverrides.Add(new ComponentPropertyOverride { PropertyId = propertyId, Text = textHtml });
                }
                // Check if this is a placeholder
                else if (element.Attributes["data-node-placeholder"] != null)
                {
                    node.Placeholder = textHtml;
                }
                // Check if this is a value
                else if (element.Attributes["data-node-value"] != null)
                {
                    node.Value = textHtml;
                }
                // Check if this is waiting text
                else if (element.Attributes["data-node-waiting-text"] != null)
                {
                    node.WaitingText = textHtml;
                }
                // Check if this is a choice
                else if (element.Attributes[ConversionConstants.NodeChoices] != null)
                {
                    var choiceValue = element.Attributes[ConversionConstants.NodeChoices].Value;
                    node.Choices ??= new List<SelectChoiceUpdate>();
                    node.Choices.Add(new SelectChoiceUpdate { Value = choiceValue, Text = textHtml });
                }
                // Otherwise it's regular text
                else
                {
                    node.Text = textHtml;
                }
            }

            updateNodes.Add(node);
        }

        // Update nodes if any
        if (updateNodes.Any())
        {
            await PatchComponentDom(siteId, input.ContentId!, input.Locale!, updateNodes);
        }
    }

    private async Task UploadJsonContent(string jsonContent, string siteId, UploadContentRequest input)
    {
        var downloadedComponent = JsonConvert.DeserializeObject<DownloadedComponent>(jsonContent)
            ?? throw new PluginMisconfigurationException("Invalid JSON file format.");

        input.Locale ??= downloadedComponent.Locale;
        input.ContentId ??= downloadedComponent.Component.ComponentId;

        await ValidateAndNormalizeInputs(input, siteId);

        // Update properties if any
        if (downloadedComponent.Properties.Any())
        {
            var updateProperties = downloadedComponent.Properties.Select(p => new UpdateComponentProperty
            {
                PropertyId = p.PropertyId,
                Text = p.Text.Html ?? p.Text.Text ?? string.Empty
            });

            await PatchComponentProperties(siteId, input.ContentId!, input.Locale!, updateProperties);
        }

        // Update nodes
        var updateNodes = downloadedComponent.Component.Nodes.Select(n => new UpdateComponentNode
        {
            NodeId = n.Id,
            Text = n.Text?.Html,
            Placeholder = n.Placeholder,
            Value = n.Value,
            WaitingText = n.WaitingText,
            Choices = n.Choices?.Select(c => new SelectChoiceUpdate
            {
                Value = c.Value,
                Text = c.Text
            }).ToList(),
            PropertyOverrides = n.PropertyOverrides?.Select(p => new ComponentPropertyOverride
            {
                PropertyId = p.PropertyId,
                Text = p.Text.Html ?? p.Text.Text ?? string.Empty
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

    private async Task PatchComponentProperties(string siteId, string contentId, string localeId, IEnumerable<UpdateComponentProperty> properties)
    {
        var body = new UpdateComponentPropertiesRequest { Properties = properties };
        var endpoint = $"sites/{siteId}/components/{contentId}/properties";

        var apiRequest = new RestRequest(endpoint, Method.Post)
            .WithJsonBody(body)
            .AddQueryParameter("localeId", localeId);

        apiRequest.RequestFormat = DataFormat.Json;
        await Client.ExecuteWithErrorHandling(apiRequest);
    }
}
