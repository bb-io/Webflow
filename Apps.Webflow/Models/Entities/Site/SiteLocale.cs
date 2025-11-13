namespace Apps.Webflow.Models.Entities.Site;

public class SiteLocale
{
    public string? Id { get; set; }
    public string? CmsLocaleId { get; set; }
    public bool? Enabled { get; set; }
    public string? DisplayName { get; set; }
    public string? DisplayImageId { get; set; }
    public bool? Redirect { get; set; }
    public string? Subdirectory { get; set; }
    public string? Tag { get; set; }
}
