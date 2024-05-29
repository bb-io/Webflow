using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json.Linq;

namespace Apps.Webflow.Webhooks.Models.Response;

public class CollectionCreatedWebhookResponse : CollectionItemResponse
{
    [DefinitionIgnore]
    public JObject FieldData { get; set; }
}