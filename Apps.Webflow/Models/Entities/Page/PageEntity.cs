using Blackbird.Applications.Sdk.Common;

namespace Apps.Webflow.Models.Entities.Page;

public class PageEntity : IDateFilterable
{
    [Display("Site ID")]
    public string? SiteId { get; set; }

    [Display("Page ID")]
    public string Id { get; set; }

    [Display("Page title")]
    public string? Title { get; set; }

    [Display("Slug")]
    public string? Slug { get; set; }

    [Display("Created on")]
    public DateTime? CreatedOn { get; set; }

    [Display("Last updated")]
    public DateTime? LastUpdated { get; set; }

    [Display("Published path")]
    public string? PublishedPath { get; set; }

    [Display("Archived")]
    public bool? Archived { get; set; }

    [Display("Is draft")]
    public bool? Draft { get; set; }

    [Display("Parent ID")]
    public string? ParentId { get; set; }

    [Display("SEO")]
    public PageSeo? Seo { get; set; }

    [Display("Open graph")]
    public PageOpenGraph? OpenGraph { get; set; }
}
