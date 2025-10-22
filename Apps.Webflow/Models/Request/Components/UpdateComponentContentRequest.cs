using Apps.Webflow.DataSourceHandlers;
using Apps.Webflow.DataSourceHandlers.Locale;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;
using Newtonsoft.Json;

namespace Apps.Webflow.Models.Request.Components;

public class UpdateComponentContentRequest
{
    [Display("HTML file")]
    public FileReference File { get; set; } = new();

    [Display("Locale ID")]
    [DataSource(typeof(SiteLocaleDataSourceHandler))]
    public string LocaleId { get; set; } = string.Empty;

    [Display("Site ID")]
    [DataSource(typeof(SiteDataSourceHandler))]
    public string? SiteId { get; set; }

    [Display("Component ID")]
    [DataSource(typeof(ComponentDataSourceHandler))]
    public string? ComponentId { get; set; }
}

public class UpdateComponentDomRequest
{
    [JsonProperty("nodes")]
    public IEnumerable<UpdateComponentNode> Nodes { get; set; } = [];
}

public class UpdateComponentNode
{
    [JsonProperty("nodeId")]
    public string NodeId { get; set; } = string.Empty;

    [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
    public string? Text { get; set; }

    [JsonProperty("propertyOverrides", NullValueHandling = NullValueHandling.Ignore)]
    public List<ComponentPropertyOverride>? PropertyOverrides { get; set; }
}

public class ComponentPropertyOverride
{
    [JsonProperty("propertyId")]
    public string PropertyId { get; set; } = string.Empty;

    [JsonProperty("text")]
    public string Text { get; set; } = string.Empty;
}
