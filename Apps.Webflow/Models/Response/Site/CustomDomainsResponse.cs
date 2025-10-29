using Apps.Webflow.Models.Entities;

namespace Apps.Webflow.Models.Response.Site;

public class CustomDomainsResponse
{
    public IEnumerable<CustomDomainEntity> CustomDomains { get; set; }
}
