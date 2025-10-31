using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json.Linq;

namespace Apps.Webflow.Webhooks.Models.Response;

public class CollectionWebhookResponse : CollectionItemResponse
{
    [DefinitionIgnore]
    public JObject FieldData { get; set; }
}