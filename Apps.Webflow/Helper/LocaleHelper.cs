using RestSharp;
using Apps.Webflow.Api;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Apps.Webflow.Models.Entities.Site;

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

    public async static Task<Dictionary<string, string>> GetLocaleMap(string siteId, WebflowClient client)
    {
        var request = new RestRequest($"/sites/{siteId}", Method.Get);
        var site = await client.ExecuteWithErrorHandling<SiteEntity>(request);

        var map = new Dictionary<string, string>();

        if (site.Locales?.Primary != null)
        {
            var p = site.Locales.Primary;
            if (p.Id != null && p.Tag != null) map[p.Id] = p.Tag;
            if (p.CmsLocaleId != null && p.Tag != null) map[p.CmsLocaleId] = p.Tag;
        }

        if (site.Locales?.Secondary != null)
        {
            foreach (var s in site.Locales.Secondary)
            {
                if (s.Id != null && s.Tag != null) map[s.Id] = s.Tag;
                if (s.CmsLocaleId != null && s.Tag != null) map[s.CmsLocaleId] = s.Tag;
            }
        }

        return map;
    }

    private async static Task<SiteLocale?> GetLocaleEntity(string languageCode, string siteId, WebflowClient client)
    {
        var request = new RestRequest($"/sites/{siteId}", Method.Get);
        var site = await client.ExecuteWithErrorHandling<SiteEntity>(request);

        if (site.Locales?.Primary?.Tag == languageCode)
            return site.Locales.Primary;

        return site.Locales?.Secondary?.FirstOrDefault(l => l.Tag == languageCode);
    }
}
