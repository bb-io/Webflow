using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Response.Webhooks;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using RestSharp;

namespace Apps.Webflow.Webhooks.Handlers;

public abstract class WebflowWebhookHandler : WebflowInvocable, IWebhookEventHandler
{
    private readonly string _siteId;
    protected abstract string EventType { get; }

    public WebflowWebhookHandler(InvocationContext invocationContext, string siteId) : base(invocationContext)
    {
        _siteId = siteId;
    }

    public Task SubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProvider,
        Dictionary<string, string> values)
    {
        var request = new RestRequest($"sites/{_siteId}/webhooks", Method.Post)
            .WithJsonBody(new
            {
                triggerType = EventType,
                url = values["payloadUrl"]
            });

        return Client.ExecuteWithErrorHandling(request);
    }

    public async Task UnsubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProvider,
        Dictionary<string, string> values)
    {
        var webhooks = await GetAllWebhooks();

        var webhookToDelete = webhooks.Webhooks.FirstOrDefault(x => x.Url == values["payloadUrl"]);

        if (webhookToDelete is null)
            return;

        var request = new RestRequest($"webhooks/{webhookToDelete.Id}", Method.Delete);
        await Client.ExecuteWithErrorHandling(request);
    }

    private Task<ListWebhooksResponse> GetAllWebhooks()
    {
        var request = new RestRequest($"sites/{_siteId}/webhooks", Method.Get);
        return Client.ExecuteWithErrorHandling<ListWebhooksResponse>(request);
    }
}