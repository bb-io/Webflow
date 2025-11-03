using System.Web;
using Apps.Webflow.HtmlConversion;
using Apps.Webflow.HtmlConversion.Constants;
using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Entities;
using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Request.Components;
using Apps.Webflow.Models.Response.Components;
using Apps.Webflow.Models.Response.Pagination;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using RestSharp;

namespace Apps.Webflow.Actions;

[ActionList("Components")]
public class ComponentsActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : WebflowInvocable(invocationContext)
{
    [Action("Search components", Description = "Search all components for a site")]
    public async Task<SearchComponentsResponse> SearchComponents(
        [ActionParameter] SiteRequest site,
        [ActionParameter] SearchComponentsRequest input)
    {
        var endpoint = $"sites/{Client.GetSiteId(site.SiteId)}/components";
        var request = new RestRequest(endpoint, Method.Get);

        IEnumerable<ComponentEntity> pages = await Client.Paginate<ComponentEntity, ComponentsPaginationResponse>(request, r => r.Components);

        if (!string.IsNullOrWhiteSpace(input.NameContains))
            pages = pages.Where(c => !string.IsNullOrEmpty(c.Name) &&
                                           c.Name.Contains(input.NameContains, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(input.NameContains))
            pages = pages.Where(c => !string.IsNullOrEmpty(c.Name) &&
                                           c.Name.Contains(input.NameContains, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(input.GroupContains))
            pages = pages.Where(c => !string.IsNullOrEmpty(c.Group) &&
                                           c.Group.Contains(input.GroupContains, StringComparison.OrdinalIgnoreCase));

        if (input.InludeReadOnly != true)
            pages = pages.Where(c => c.ReadOnly != true);

        return new SearchComponentsResponse(pages.ToList());
    }

    [Action("Download component", Description = "Get the component content in HTML file")]
    public async Task<DownloadComponentResponse> GetComponentAsHtml(
        [ActionParameter] SiteRequest site,
        [ActionParameter] DownloadComponentContentRequest input)
    {
        var endpoint = $"sites/{Client.GetSiteId(site.SiteId)}/components/{input.ComponentId}/dom";
        var request = new RestRequest(endpoint, Method.Get);

        if (!string.IsNullOrEmpty(input.LocaleId))
            request.AddQueryParameter("localeId", input.LocaleId);

        var componentDom = await Client.ExecuteWithErrorHandling<ComponentDomEntity>(request);

        var htmlStream = ComponentHtmlConverter.ToHtml(componentDom, Client.GetSiteId(site.SiteId), input.ComponentId);

        var file = await fileManagementClient.UploadAsync(htmlStream, "text/html", $"component_{input.ComponentId}.html");
        return new(file);
    }

    [Action("Upload component", Description = "Update component content using HTML file")]
    public async Task UpdateComponentContentAsHtml(
        [ActionParameter] SiteRequest site,
        [ActionParameter] UpdateComponentContentRequest input)
    {
        var fileStream = await fileManagementClient.DownloadAsync(input.File);

        var doc = new HtmlAgilityPack.HtmlDocument();
        doc.Load(fileStream);

        if (string.IsNullOrEmpty(input.LocaleId))
            throw new PluginMisconfigurationException("Locale ID is required.");

        if (string.IsNullOrEmpty(input.ComponentId))
        {
            var metaComponentIdNode = doc.DocumentNode.SelectSingleNode("//meta[@name='blackbird-component-id']");
            input.ComponentId = metaComponentIdNode.GetAttributeValue("content", string.Empty);

            if (string.IsNullOrEmpty(input.ComponentId))
                throw new PluginMisconfigurationException(
                    "Component ID not found in the HTML file. " +
                    "Please provide it in input or ensure that file contains <meta name=\"blackbird-component-id\"> tag."
                );
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

        var endpoint = $"sites/{Client.GetSiteId(site.SiteId)}/components/{input.ComponentId}/dom";
        var apiRequest = new RestRequest(endpoint, Method.Post).WithJsonBody(body);

        apiRequest.RequestFormat = DataFormat.Json;
        apiRequest.AddQueryParameter("localeId", input.LocaleId);

        await Client.ExecuteWithErrorHandling(apiRequest);
    }
}
