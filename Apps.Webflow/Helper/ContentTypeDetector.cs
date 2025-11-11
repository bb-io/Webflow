using Apps.Webflow.Constants;
using Apps.Webflow.Extensions;
using Apps.Webflow.Services;
using Blackbird.Applications.Sdk.Common.Exceptions;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace Apps.Webflow.Helper;

public static class ContentTypeDetector
{
    private class ContentMetadata
    {
        [JsonProperty("contentType")]
        public string? ContentType { get; set; }
    }

    public static string GetContentType(string fileText)
    {
        if (JsonHelper.IsJson(fileText))
        {
            try
            {
                var metadata = JsonConvert.DeserializeObject<ContentMetadata>(fileText);

                if (metadata?.ContentType is not null)
                    return MapKebabCaseToContentType(metadata.ContentType);
            }
            catch (JsonException) { }
        }
        else
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(fileText);

            var head = doc.DocumentNode.SelectSingleNode("//head") 
                ?? throw new PluginMisconfigurationException(
                   "File is not a valid HTML file with a 'head' section. Unable to determine content type.");

            var contentTypeMeta = head.GetMetaValue("blackbird-content-type");
            if (contentTypeMeta is not null)
                return MapKebabCaseToContentType(contentTypeMeta);
        }

        throw new PluginMisconfigurationException(
            "Unable to recognize the content type from file metadata. Please provide in the input."
        );
    }

    private static string MapKebabCaseToContentType(string kebabCaseType)
    {
        if (kebabCaseType == ContentTypes.Page.ToKebabCase())
            return ContentTypes.Page;

        if (kebabCaseType == ContentTypes.Component.ToKebabCase())
            return ContentTypes.Component;

        if (kebabCaseType == ContentTypes.CollectionItem.ToKebabCase())
            return ContentTypes.CollectionItem;

        throw new InvalidOperationException($"Unknown content type: {kebabCaseType}");
    }
}