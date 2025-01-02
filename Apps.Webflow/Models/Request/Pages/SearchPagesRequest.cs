using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apps.Webflow.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Webflow.Models.Request.Pages
{
    public class SearchPagesRequest
    {
        [Display("Site ID")]
        [DataSource(typeof(SiteDataSourceHandler))]
        public string SiteId {  get; set; }

        [Display("Locale ID")]
        public string? LocaleId {  get; set; }

        [Display("Offset")]
        public string? Offset {  get; set; }

        [Display("Limit")]
        public string? Limit { get; set; }
    }
}
