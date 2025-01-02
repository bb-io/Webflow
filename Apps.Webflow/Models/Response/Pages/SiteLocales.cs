using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Webflow.Models.Response.Pages
{
    public class SiteLocales
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public DateTime? LastPublished { get; set; }
        public LocalesResponse Locales { get; set; }
    }
    public class LocalesResponse
    {
        public CmsLocale Primary { get; set; }
        public IEnumerable<CmsLocale> Secondary { get; set; }
    }

    public class CmsLocale
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string CmsLocaleId { get; set; } 
    }
}
