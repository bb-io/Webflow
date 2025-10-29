using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;

namespace Apps.Webflow.Models.Response.Content;

public class DownloadContentResponse(FileReference content) : IDownloadContentOutput
{
    public FileReference Content { get; set; } = content;
}
