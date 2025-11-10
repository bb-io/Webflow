using RestSharp;
using Apps.Webflow.Api;
using Apps.Webflow.Models.Entities;
using Blackbird.Applications.Sdk.Common.Exceptions;

namespace Apps.Webflow.Helper;

public static class LocaleHelper
{
    public async static Task<string> GetCmsLocaleId(string languageCode, string siteId, WebflowClient client)
    {
        var locale = await GetLocaleEntity(languageCode, siteId, client);

        if (locale?.CmsLocaleId is not null)
            return locale.CmsLocaleId;

        throw new PluginApplicationException($"Can't match language code {languageCode} to cmsLocaleId");
    }

    public async static Task<string> GetLocaleId(string languageCode, string siteId, WebflowClient client)
    {
        var locale = await GetLocaleEntity(languageCode, siteId, client);

        if (locale?.Id is not null)
            return locale.Id;

        throw new PluginApplicationException($"Can't match language code {languageCode} to localeId");
    }

    private async static Task<SiteLocale?> GetLocaleEntity(string languageCode, string siteId, WebflowClient client)
    {
        var request = new RestRequest($"/sites/{client.GetSiteId(siteId)}", Method.Get);
        var site = await client.ExecuteWithErrorHandling<SiteEntity>(request);

        if (site.Locales?.Primary?.Tag == languageCode)
            return site.Locales.Primary;

        return site.Locales?.Secondary?.FirstOrDefault(l => l.Tag == languageCode);
    }
}
