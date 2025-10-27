using Apps.Webflow.DataSourceHandlers.Content;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Webflow.Models.Request.Content;

public class ContentFilter
{
    [Display("Content type")]
    [StaticDataSource(typeof(ContentTypeStaticDataHandler))]
    public IEnumerable<string> ContentTypes { get; set; }
}
