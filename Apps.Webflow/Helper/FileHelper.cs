using System.Net.Mime;

namespace Apps.Webflow.Helper;

public static class FileHelper
{
    public static string GetDownloadedFileName(string fileFormat, string contentId, string contentType)
    {
        string fileExtension = fileFormat == MediaTypeNames.Text.Html ? "html" : "json";
        return $"{contentType.Replace(' ', '_').ToLower()}_{contentId}.{fileExtension}";
    }
}
