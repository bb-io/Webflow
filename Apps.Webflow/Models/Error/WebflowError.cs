using Newtonsoft.Json;

namespace Apps.Webflow.Models.Error;

public class WebflowError
{
    [JsonProperty("message")]
    public string Message { get; set; }

    [JsonProperty("code")]
    public string Code { get; set; }

    [JsonProperty("externalReference")]
    public string ExternalReference { get; set; }

    [JsonProperty("details")]
    public List<ErrorDetail> Details { get; set; }

    public override string ToString()
    {
        if (Details is { Count: > 0 })
        {
            var formattedDetails = string.Join("; ", Details.Select(d => d.ToString()));
            return $"{Message} (code: {Code}): {formattedDetails}";
        }

        return $"{Message} (code: {Code})";
    }
}