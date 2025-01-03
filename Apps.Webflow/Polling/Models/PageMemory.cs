using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Webflow.Polling.Models
{
    public class PageMemory 
    {
        public DateTime? LastPollingTime { get; set; }

        public bool Triggered { get; set; }

    }
}
