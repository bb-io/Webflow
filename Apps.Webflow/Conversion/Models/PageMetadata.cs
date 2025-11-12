using Apps.Webflow.Models.Entities.Page;

namespace Apps.Webflow.Conversion.Models;

public record PageMetadata(string? PageTitle, string? Slug, PageSeo? SeoMetadata, PageOpenGraph? OpenGraphMetadata);