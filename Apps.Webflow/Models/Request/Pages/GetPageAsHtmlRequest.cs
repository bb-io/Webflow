﻿using Apps.Webflow.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Webflow.Models.Request.Pages
{
    public class GetPageAsHtmlRequest
    {
        [Display("Site ID")]
        [DataSource(typeof(SiteDataSourceHandler))]
        public string SiteId {  get; set; }

        [Display("Page ID")]
        [DataSource(typeof(PageDataSourceHandler))]
        public string PageId { get; set; }

        [Display("Locale ID")]
        [DataSource(typeof(SiteLocaleDataSourceHandler))]
        public string? LocaleId { get; set; }
    }
}
