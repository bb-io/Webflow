namespace Apps.Webflow.Webhooks.Models;

public class WebflowWebhookResponse<T>
{
    public T Payload { get; set; }
}