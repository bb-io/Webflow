using Newtonsoft.Json;

namespace Apps.Webflow.Models.Error;

public class ErrorDetail
{
    [JsonProperty("param")]
    public string Param { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    public override string ToString() => $"{Param}: {Description}";
}
