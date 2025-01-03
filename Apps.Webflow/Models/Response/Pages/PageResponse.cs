using Blackbird.Applications.Sdk.Common;

namespace Apps.Webflow.Models.Response.Pages
{
    public class PageResponse
    {
        [Display("Site ID")]
        public string SiteId { get; set; }

        [Display("Page ID")]
        public string Id { get; set; }

        [Display("Page title")]
        public string Title { get; set; }

        [Display("Slug")]
        public string Slug { get; set; }

        [Display("Created on")]
        public DateTime CreatedOn { get; set; }

        [Display("Last updated")]
        public DateTime LastUpdated { get; set; }

        [Display("Locale ID")]
        public string LocaleId { get; set; }

        [Display("Published path")]
        public string PublishedPath { get; set; }
    }
}
