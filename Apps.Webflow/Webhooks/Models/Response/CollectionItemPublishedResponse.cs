using Newtonsoft.Json;

namespace Apps.Webflow.Webhooks.Models.Response;

public class CollectionItemPublishedResponse
{
    // You can always use Items.First() safely.
    // Although it's a list, the API will always return one element in it.
    // For some reason :/
    [JsonProperty("items")]
    public List<CollectionWebhookResponse> Items { get; set; }
}
