using RestSharp;
using Apps.Webflow.Api;
using Apps.Webflow.Models.Entities.Site;
using Blackbird.Applications.Sdk.Common.Exceptions;

namespace Apps.Webflow.Helper;

public static class LocaleHelper
{
    public async static Task<SiteLocale> GetLocale(string languageCode, string siteId, WebflowClient client)
    {
        var request = new RestRequest($"/sites/{siteId}", Method.Get);
        var site = await client.ExecuteWithErrorHandling<SiteEntity>(request);

        if (site.Locales?.Primary?.Tag == languageCode)
        {
            var primary = site.Locales.Primary;
            primary.IsPrimary = true;
            return primary;
        }

        var secondary = site.Locales?.Secondary?.FirstOrDefault(l => l.Tag == languageCode);
        if (secondary != null)
        {
            secondary.IsPrimary = false;
            return secondary;
        }

        throw new PluginApplicationException($"Can't match language code {languageCode} to any locale in site {siteId}");
    }

    public async static Task<string> GetCmsLocaleId(string languageCode, string siteId, WebflowClient client)
    {
        var locale = await GetLocale(languageCode, siteId, client);

        if (locale.CmsLocaleId is not null)
            return locale.CmsLocaleId;

        throw new PluginApplicationException($"Locale matched, but CmsLocaleId was null for {languageCode}");
    }

    public async static Task<string> GetLocaleId(string languageCode, string siteId, WebflowClient client)
    {
        var locale = await GetLocale(languageCode, siteId, client);

        if (locale.Id is not null)
            return locale.Id;

        throw new PluginApplicationException($"Locale matched, but Id was null for {languageCode}");
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
}