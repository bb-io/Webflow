using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Webflow.Models.Response.Components;

public class DownloadComponentResponse(FileReference file)
{
    [Display("Component")]
    public FileReference File { get; set; } = file;
}
