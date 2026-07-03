namespace Apps.Webflow.Polling.Models;

public class PageMemory
{
    public DateTime? LastPollingTime { get; set; }
    public bool Triggered { get; set; }
}
