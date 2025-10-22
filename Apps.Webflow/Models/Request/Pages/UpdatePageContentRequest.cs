using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apps.Webflow.DataSourceHandlers;
using Apps.Webflow.DataSourceHandlers.Locale;
using Apps.Webflow.DataSourceHandlers.Site;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;
using Newtonsoft.Json;

namespace Apps.Webflow.Models.Request.Pages
{
    public class UpdatePageContentRequest
    {
        [Display("Site ID")]
        [DataSource(typeof(SiteDataSourceHandler))]
        public string? SiteId { get; set; }

        [Display("Page ID")]
        [DataSource(typeof(PageDataSourceHandler))]
        public string? PageId { get; set; }

        [Display("Locale ID")]
        [DataSource(typeof(SiteLocaleDataSourceHandler))]
        public string? LocaleId { get; set; }

        [Display("HTML file")]
        public FileReference File { get; set; }
    }

    public class UpdatePageDomRequest
    {
        [JsonProperty("localeId")]
        public string? LocaleId { get; set; }

        [JsonProperty("nodes")]
        public IEnumerable<UpdatePageNode> Nodes { get; set; }
    }

    public class UpdatePageNode
    {
        [JsonProperty("nodeId")]
        public string NodeId { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }
}
