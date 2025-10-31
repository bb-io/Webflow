using Apps.Webflow.Constants;
using Apps.Webflow.HtmlConversion;
using Apps.Webflow.HtmlConversion.Constants;
using Apps.Webflow.Models.Entities;
using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Request.Components;
using Apps.Webflow.Models.Request.Content;
using Apps.Webflow.Models.Response.Components;
using Apps.Webflow.Models.Response.Content;
using Apps.Webflow.Models.Response.Pagination;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using RestSharp;
using System.Web;

namespace Apps.Webflow.Services.Concrete;

public class ComponentService(InvocationContext invocationContext) : BaseContentService(invocationContext)
{
    private const string ContentType = ContentTypes.Component;

    public async override Task<SearchContentResponse> SearchContent(string siteId, SearchContentRequest input, DateFilter dateFilter)
    {
        ThrowForDateInputs(dateFilter, ContentType);
        ThrowForPublishedDateInputs(input, ContentType);

        var endpoint = $"sites/{siteId}/components";
        var request = new RestRequest(endpoint, Method.Get);

        IEnumerable<ComponentEntity> pages = await Client.Paginate<ComponentEntity, ComponentsPaginationResponse>(request, r => r.Components);

        if (!string.IsNullOrWhiteSpace(input.NameContains))
            pages = pages.Where(c => !string.IsNullOrEmpty(c.Name) &&
                                           c.Name.Contains(input.NameContains, StringComparison.OrdinalIgnoreCase));

        var result = pages.Select(x => new ContentItemEntity
        {
            ContentId = x.Id,
            Name = x.Name,
            Type = ContentType
        });

        return new SearchContentResponse(result);
    }

    public override async Task<Stream> DownloadContent(string siteId, DownloadContentRequest input)
    {
        var endpoint = $"sites/{siteId}/components/{input.ContentId}/dom";
        var request = new RestRequest(endpoint, Method.Get);

        if (!string.IsNullOrEmpty(input.Locale))
            request.AddQueryParameter("localeId", input.Locale);

        var componentDom = await Client.ExecuteWithErrorHandling<ComponentDomEntity>(request);

        var stream = ComponentHtmlConverter.ToHtml(componentDom, siteId, input.ContentId);
        var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        return memoryStream;
    }

    public override async Task UploadContent(Stream content, string siteId, UploadContentRequest input)
    {
        if (string.IsNullOrEmpty(input.Locale))
            throw new PluginMisconfigurationException("Please specify the 'Locale' input");

        var memoryStream = new MemoryStream();
        await content.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        var doc = new HtmlAgilityPack.HtmlDocument();
        doc.Load(memoryStream);

        if (string.IsNullOrEmpty(input.ContentId))
        {
            var metaComponentIdNode = doc.DocumentNode.SelectSingleNode("//meta[@name='blackbird-component-id']");
            input.ContentId = metaComponentIdNode?.GetAttributeValue("content", string.Empty);

            if (string.IsNullOrEmpty(input.ContentId))
                throw new PluginMisconfigurationException("Component ID not found in the HTML file. Please, provide it in input or ensure that file contains <meta name=\"blackbird-component-id\"> tag.");
        }

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
                // This is a component instance with property overrides
                var existingNode = updateNodes.FirstOrDefault(n => n.NodeId == nodeId);
                if (existingNode == null)
                {
                    existingNode = new UpdateComponentNode
                    {
                        NodeId = nodeId,
                        PropertyOverrides = []
                    };
                    updateNodes.Add(existingNode);
                }

                existingNode.PropertyOverrides?.Add(new ComponentPropertyOverride
                {
                    PropertyId = propertyId,
                    Text = textHtml
                });
            }
            else
            {
                // This is a text node
                updateNodes.Add(new UpdateComponentNode
                {
                    NodeId = nodeId,
                    Text = textHtml
                });
            }
        }

        var body = new UpdateComponentDomRequest { Nodes = updateNodes };

        var endpoint = $"sites/{siteId}/components/{input.ContentId}/dom";
        var apiRequest = new RestRequest(endpoint, Method.Post).WithJsonBody(body);

        apiRequest.RequestFormat = DataFormat.Json;
        apiRequest.AddQueryParameter("localeId", input.Locale);

        await Client.ExecuteWithErrorHandling(apiRequest);
    }
}
