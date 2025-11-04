namespace Apps.Webflow.Polling.Models;

public class ListPagesPollingResponse(IEnumerable<PagePollingEntity> pages)
{
    public IEnumerable<PagePollingEntity> Pages { get; set; } = pages;
}
