using Apps.Webflow.DataSourceHandlers.Content;
using Apps.Webflow.DataSourceHandlers.Locale;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;

namespace Apps.Webflow.Models.Request.Content;

public class UploadContentRequest : IUploadContentInput
{
    [Display("Content")]
    public FileReference Content { get; set; }

    [Display("Locale")]
    [DataSource(typeof(SiteLocaleDataSourceHandler))]
    public string? Locale { get; set; }

    [Display("Content ID")]
    [DataSource(typeof(ContentDataHandler))]
    public string? ContentId { get; set; }

    [Display("Locale")]
    [DataSource(typeof(UpdateCollectionItemLocaleDataSourceHandler))]
    public string? CmsLocaleId { get; set; }
}
