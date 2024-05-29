using Apps.Webflow.Models.Entities;

namespace Apps.Webflow.Models.Response.Webhooks;

public class ListWebhooksResponse
{
    public IEnumerable<WebhookEntity> Webhooks { get; set; }
}