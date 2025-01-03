using Apps.Webflow.Models.Entities;

namespace Apps.Webflow.Polling.Models
{
    public class ListPagesPollingResponse
    {
        public IEnumerable<PagePollingEntity> Pages { get; set; }
    }
}
