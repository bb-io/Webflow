using Newtonsoft.Json.Linq;

namespace Apps.Webflow.Models.Entities;

public class CollectionItemEntity
{
    public string Id { get; set; }
    
    public string CmsLocaleId { get; set; }
    
    public JObject FieldData { get; set; }
}