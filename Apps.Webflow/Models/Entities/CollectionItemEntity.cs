using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json.Linq;

namespace Apps.Webflow.Models.Entities;

public class CollectionItemEntity
{
    [Display("Collection item ID")]
    public string Id { get; set; }
    
    [Display("Locale ID")]
    public string CmsLocaleId { get; set; }
    
    public string Name => FieldData?["name"]?.ToString() ?? Id;

    [Display("Last updated")]
    public DateTime? LastUpdated { get; set; }

    [DefinitionIgnore]
    public JObject FieldData { get; set; }
}