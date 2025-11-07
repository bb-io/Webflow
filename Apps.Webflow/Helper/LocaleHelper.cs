using RestSharp;
using Apps.Webflow.Api;
using Apps.Webflow.Models.Entities;
using Blackbird.Applications.Sdk.Common.Exceptions;

namespace Apps.Webflow.Helper;

public static class LocaleHelper
{
    public async static Task<string> GetCmsLocaleId(string languageCode, string siteId, WebflowClient client)
    {
        var request = new RestRequest($"/sites/{client.GetSiteId(siteId)}", Method.Get);
        var site = await client.ExecuteWithErrorHandling<SiteEntity>(request);

        var primaryLocale = site.Locales?.Primary;
        var secondaryLocales = site.Locales?.Secondary;

        if (primaryLocale?.Tag == languageCode)
            return primaryLocale.CmsLocaleId!;

        var matchingSecondary = secondaryLocales?.FirstOrDefault(l => l.Tag == languageCode);
        if (matchingSecondary?.CmsLocaleId is not null)
            return matchingSecondary.CmsLocaleId;

        throw new PluginApplicationException($"Can't match language code {languageCode} to cmsLocaleId");
    }
}
