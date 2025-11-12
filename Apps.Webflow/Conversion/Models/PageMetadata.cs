using Apps.Webflow.Models.Entities.Page;

namespace Apps.Webflow.Conversion.Models;

public class PageMetadata
{
    public PageMetadata(string? pageTitle, string? slug, PageSeo? seo, PageOpenGraph? openGraph)
    {
        PageTitle = pageTitle;
        Slug = slug;
        Seo = seo;
        OpenGraph = openGraph;
    }

    public PageMetadata() { }

    public string? PageTitle { get; set; }
    public string? Slug { get; set; }
    public PageSeo? Seo { get; set; }
    public PageOpenGraph? OpenGraph { get; set; }
};