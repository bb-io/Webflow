using Apps.Webflow.DataSourceHandlers.Site;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Webflow.Models.Request.Site;

public class PublishSiteRequest
{
    [Display("Custom domains", Description = "Array of custom domain IDs to publish")]
    [DataSource(typeof(CustomDomainDataSourceHandler))]
    public IEnumerable<string>? CustomDomains { get; set; }
}
