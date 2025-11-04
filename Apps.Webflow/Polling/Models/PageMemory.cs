namespace Apps.Webflow.Polling.Models;

public class PageMemory(DateTime? lastPollingTime, bool triggered)
{
    public DateTime? LastPollingTime { get; set; } = lastPollingTime;
    public bool Triggered { get; set; } = triggered;
}
