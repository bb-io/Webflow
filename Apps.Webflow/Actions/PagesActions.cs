using System.Web;
using Apps.Webflow.Api;
using Apps.Webflow.HtmlConversion;
using Apps.Webflow.HtmlConversion.Constants;
using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Request.Pages;
using Apps.Webflow.Models.Response;
using Apps.Webflow.Models.Response.Pages;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using RestSharp;

namespace Apps.Webflow.Actions
{
    [ActionList]
    public class PagesActions : WebflowInvocable
    {
        private readonly IFileManagementClient _fileManagementClient;

        public PagesActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) :
        base(invocationContext)
        {
            _fileManagementClient = fileManagementClient;
        }


        [Action("Search pages", Description = "Search pages using filters")]
        public async Task<ListPagesResponse> SearchPages([ActionParameter] SearchPagesRequest input)
        {
            var endpoint = $"sites/{input.SiteId}/pages";
            var request = new WebflowRequest(endpoint, Method.Get, Creds);

            if (input.Offset != null)
            {
                request.AddQueryParameter("offset", input.Offset);
            }

            if (input.Limit != null)
            {
                request.AddQueryParameter("limit", input.Limit);
            }

            if (input.LocaleId != null)
            {
                request.AddQueryParameter("localeId", input.LocaleId);
            }

            var pages = await Client.ExecuteWithErrorHandling<ListPagesResponse>(request);

            return pages;
        }

        [Action("Get page content as HTML", Description = "Get the page content in HTML file")]
        public async Task<FileReference> GetPageAsHtml([ActionParameter] GetPageAsHtmlRequest input)
        {
            var endpoint = $"pages/{input.PageId}/dom";
            var request = new WebflowRequest(endpoint, Method.Get, Creds);

            if (!string.IsNullOrEmpty(input.LocaleId))
                request.AddQueryParameter("localeId", input.LocaleId);

            var pageDom = await Client.ExecuteWithErrorHandling<PageDomEntity>(request);

            var htmlStream = PageHtmlConverter.ToHtml(pageDom);


            var fileName = $"page_{input.PageId}.html";
            var contentType = "text/html";

            var fileReference = await _fileManagementClient.UploadAsync(
                    htmlStream,
                    contentType,
                    fileName);

            fileReference.Name = fileName;
            fileReference.ContentType = contentType;

            return fileReference;
        }

        [Action("Update page content as HTML", Description = "Update page content using HTML file")]
        public async Task<UpdatePageContentResponse> UpdatePageContentAsHtml([ActionParameter] UpdatePageContentRequest input)
        {
            var fileStream = await _fileManagementClient.DownloadAsync(input.File);
            fileStream.Position = 0;
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.Load(fileStream);

            var elements = doc.DocumentNode
         .Descendants()
         .Where(x => x.NodeType == HtmlAgilityPack.HtmlNodeType.Element
                     && x.Attributes[ConversionConstants.NodeId] != null)
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
}
