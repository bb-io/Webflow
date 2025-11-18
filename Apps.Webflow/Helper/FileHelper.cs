using Apps.Webflow.Constants;
using System.Text.RegularExpressions;

namespace Apps.Webflow.Helper;

public static class FileHelper
{
    private static readonly Regex AllowedCharsRegex = new(@"[^a-z0-9_-]", RegexOptions.Compiled);

    public static string GetDownloadedFileName(string contentType, string contentId, string contentName, string fileFormat)
    {
        string fileExtension = fileFormat == ContentFormats.InteroperableHtml ? "html" : "json";
        string contentTypeSanitized = Sanitize(contentType);
        string contentNameSanitized = Sanitize(contentName);
        return $"{contentTypeSanitized}_{contentId}_{contentNameSanitized}.{fileExtension}";
    }

    private static string Sanitize(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        string lower = input.ToLower().Replace(' ', '_');

        return AllowedCharsRegex.Replace(lower, "");
    }
}
