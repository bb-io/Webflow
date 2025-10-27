using Apps.Webflow.DataSourceHandlers.Content;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;

namespace Apps.Webflow.Models.Request.Content;

public class ContentRequest : IDownloadContentInput
{
    [DataSource(typeof(ContentDataHandler))]
    [Display("Content ID")]
    public string ContentId { get; set; }
}
