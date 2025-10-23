using System.Web;
using Apps.Webflow.HtmlConversion;
using Apps.Webflow.HtmlConversion.Constants;
using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Request.Components;
using Apps.Webflow.Models.Response.Components;
using Apps.Webflow.Models.Response.Pagination;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using RestSharp;

namespace Apps.Webflow.Actions;

[ActionList("Components")]
public class ComponentsActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : WebflowInvocable(invocationContext)
{
    [Action("List components", Description = "List all components for a site")]
    public async Task<ListComponentsResponse> ListComponents([ActionParameter] SearchComponentsRequest input)
    {
        var allComponents = new List<ComponentResponse>();
        var offset = 0;
        const int pageSize = 100;
        int total = int.MaxValue;

        while (allComponents.Count < total)
        {
            var endpoint = $"sites/{input.SiteId}/components";
            var request = new RestRequest(endpoint, Method.Get);

            request.AddQueryParameter("offset", offset.ToString());
            request.AddQueryParameter("limit", pageSize.ToString());

            var batch = await Client.ExecuteWithErrorHandling<ListComponentsResponse>(request);

            var batchComponents = batch.Components?.ToList() ?? [];
            total = batch.Pagination?.Total ?? batchComponents.Count;

            allComponents.AddRange(batchComponents);

            if (batchComponents.Count == 0) break;
            offset += batchComponents.Count;
        }

        IEnumerable<ComponentResponse> filtered = allComponents;

        if (!string.IsNullOrWhiteSpace(input.NameContains))
            filtered = filtered.Where(c => !string.IsNullOrEmpty(c.Name) &&
                                           c.Name.Contains(input.NameContains, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(input.GroupContains))
            filtered = filtered.Where(c => !string.IsNullOrEmpty(c.Group) &&
                                           c.Group.Contains(input.GroupContains, StringComparison.OrdinalIgnoreCase));

        if (input.InludeReadOnly != true)
            filtered = filtered.Where(c => c.ReadOnly != true);

        var resultComponents = filtered.ToList();

        return new ListComponentsResponse
        {
            Components = resultComponents,
            Pagination = new PaginationInfo
            {
                Total = resultComponents.Count
            }
        };
    }

    [Action("Get component content as HTML", Description = "Get the component content in HTML file")]
    public async Task<FileReference> GetComponentAsHtml([ActionParameter] GetComponentContentRequest input)
    {
        var endpoint = $"sites/{input.SiteId}/components/{input.ComponentId}/dom";
        var request = new RestRequest(endpoint, Method.Get);

        if (!string.IsNullOrEmpty(input.LocaleId))
            request.AddQueryParameter("localeId", input.LocaleId);

        var componentDom = await Client.ExecuteWithErrorHandling<ComponentDomEntity>(request);

        var htmlStream = ComponentHtmlConverter.ToHtml(componentDom, input.SiteId, input.ComponentId);

        return await fileManagementClient.UploadAsync(
            htmlStream,
            "text/html",
            $"component_{input.ComponentId}.html");
    }

    [Action("Update component content as HTML", Description = "Update component content using HTML file")]
    public async Task<UpdateComponentContentResponse> UpdateComponentContentAsHtml([ActionParameter] UpdateComponentContentRequest input)
    {
        var fileStream = await fileManagementClient.DownloadAsync(input.File);

        var doc = new HtmlAgilityPack.HtmlDocument();
        doc.Load(fileStream);

        if (string.IsNullOrEmpty(input.LocaleId))
            throw new PluginMisconfigurationException("Locale ID is required.");

        if (string.IsNullOrEmpty(input.SiteId))
        {
            var metaSiteIdNode = doc.DocumentNode.SelectSingleNode("//meta[@name='blackbird-site-id']");
            input.SiteId = metaSiteIdNode.GetAttributeValue("content", string.Empty);

            if (string.IsNullOrEmpty(input.SiteId))
                throw new PluginMisconfigurationException("Site ID not found in the HTML file. Please, provide it in input or ensure that file contains <meta name=\"blackbird-site-id\"> tag.");
        }

        if (string.IsNullOrEmpty(input.ComponentId))
        {
            var metaComponentIdNode = doc.DocumentNode.SelectSingleNode("//meta[@name='blackbird-component-id']");
            input.ComponentId = metaComponentIdNode.GetAttributeValue("content", string.Empty);

            if (string.IsNullOrEmpty(input.ComponentId))
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

        var body = new UpdateComponentDomRequest
        {
            Nodes = updateNodes
        };

        var endpoint = $"sites/{input.SiteId}/components/{input.ComponentId}/dom";
        var apiRequest = new RestRequest(endpoint, Method.Post).WithJsonBody(body);

        apiRequest.RequestFormat = DataFormat.Json;
        apiRequest.AddQueryParameter("localeId", input.LocaleId);

        await Client.ExecuteWithErrorHandling(apiRequest);

        return new UpdateComponentContentResponse
        {
            Success = true
        };
    }
}
