using Apps.Webflow.Webhooks.Models;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Newtonsoft.Json;

namespace Apps.Webflow.Extensions;

public static class WebhookRequestExtensions
{
    public static T GetPayload<T>(this WebhookRequest request)
    {
        var body = request.Body?.ToString() ?? throw new ArgumentNullException(nameof(request.Body));
        
        var response = JsonConvert.DeserializeObject<WebflowWebhookResponse<T>>(body) ?? throw new ArgumentNullException(nameof(request.Body));
        return response.Payload;
    }

}