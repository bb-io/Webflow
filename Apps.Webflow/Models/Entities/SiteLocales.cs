namespace Apps.Webflow.Models.Entities;

public class SiteLocales
{
    public SiteLocale? Primary { get; set; }
    public IEnumerable<SiteLocale>? Secondary { get; set; }
}