namespace Apps.Webflow.Models.Entities;

public class WebhookEntity
{
    public string Id { get; set; }
    
    public string Url { get; set; }
    
    public string TriggerType { get; set; }
}