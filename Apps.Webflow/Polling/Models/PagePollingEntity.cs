using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Webflow.Polling.Models
{
    public class PagePollingEntity
    {
        public string Id { get; set; } 
        public string SiteId { get; set; }
        public DateTime? LastUpdated { get; set; }
        public string Title {  get; set; }
    }
}
