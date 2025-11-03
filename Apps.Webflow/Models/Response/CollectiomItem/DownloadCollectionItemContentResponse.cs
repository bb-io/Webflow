using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Webflow.Models.Response.CollectiomItem;

public class DownloadCollectionItemContentResponse(FileReference file)
{
    [Display("Collection item")]
    public FileReference File { get; set; } = file;
}
