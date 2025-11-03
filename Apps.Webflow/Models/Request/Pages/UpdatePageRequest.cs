using Newtonsoft.Json;

namespace Apps.Webflow.Models.Request.Pages;

public class UpdatePageDomRequest
{
    [JsonProperty("localeId")]
    public string? LocaleId { get; set; }

    [JsonProperty("nodes")]
    public IEnumerable<UpdatePageNode> Nodes { get; set; }
}

public class UpdatePageNode
{
    [JsonProperty("nodeId")]
    public string NodeId { get; set; }

    [JsonProperty("text")]
    public string Text { get; set; }
}
