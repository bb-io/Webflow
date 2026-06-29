using Apps.Webflow.DataSourceHandlers.Collection;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Webflow.Models.Request.CollectionItem;

public class UploadFileToCollectionItemRequest
{
    [Display("File")]
    public FileReference File { get; set; } = null!;

    [Display("Field slug"), DataSource(typeof(CollectionFileFieldSlugDataHandler))]
    public string FieldSlug { get; set; } = string.Empty;
}