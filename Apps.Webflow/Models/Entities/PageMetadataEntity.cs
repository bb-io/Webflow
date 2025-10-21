using Newtonsoft.Json;

namespace Apps.Webflow.Models.Entities;

// https://developers.webflow.com/data/v2.0.0/reference/pages-and-components/pages/get-metadata
public class PageMetadataEntity
{
    [JsonProperty("siteId")]
    public string? SiteId { get; set; }

    [JsonProperty("title")]
    public string? Title { get; set; }

    [JsonProperty("slug")]
    public string? Slug { get; set; }

    [JsonProperty("createdOn")]
    public DateTime? CreatedOn { get; set; }

    [JsonProperty("lastUpdated")]
    public DateTime? LastUpdated { get; set; }

    [JsonProperty("archived")]
    public bool? IsArchived { get; set; }

    [JsonProperty("draft")]
    public bool? IsDraft { get; set; }

    [JsonProperty("localeId")]
    public string? LocaleId { get; set; }

    [JsonProperty("publishedPath")]
    public string? PublishedPath { get; set; }
}
