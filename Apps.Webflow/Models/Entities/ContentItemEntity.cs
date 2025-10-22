using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;

namespace Apps.Webflow.Models.Entities;

public class ContentItemEntity : IDownloadContentInput
{
    [Display("Content ID")]
    public string ContentId { get; set; } = string.Empty;

    [Display("Name")]
    public string Name { get; set; } = string.Empty;

    [Display("Content type")]
    public string Type { get; set; } = string.Empty;
}
