using Newtonsoft.Json;

namespace Apps.Webflow.Webhooks.Models.Response;

public class CollectionItemPublishedResponse
{
    [JsonProperty("items")]
    public List<CollectionCreatedWebhookResponse> Items { get; set; }
}
