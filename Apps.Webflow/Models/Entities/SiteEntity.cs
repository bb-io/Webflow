using Apps.Webflow.Models.Response;

namespace Apps.Webflow.Models.Entities;

public class SiteEntity
{
    public string Id { get; set; }
    
    public string DisplayName { get; set; }
    
    public DateTime? LastPublished { get; set; }

    public DateTime? LastUpdated { get; set; }

    public LocalesResponse? Locales { get; set; }
}