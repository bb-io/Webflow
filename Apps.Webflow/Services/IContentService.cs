using Apps.Webflow.Models.Request.Content;
using Apps.Webflow.Models.Request.Date;
using Apps.Webflow.Models.Response.Content;

namespace Apps.Webflow.Services;

public interface IContentService
{
    Task<SearchContentResponse> SearchContent(string siteId, SearchContentRequest input, ContentDateFilter dateFilter);
    Task<Stream> DownloadContent(string siteId, DownloadContentRequest input);
    Task UploadContent(Stream content, string? siteId, UploadContentRequest input);
}
