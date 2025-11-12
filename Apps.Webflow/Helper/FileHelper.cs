using System.Net.Mime;

namespace Apps.Webflow.Helper;

public static class FileHelper
{
    public static string GetDownloadedFileName(string contentName, string fileFormat)
    {
        string fileExtension = fileFormat == MediaTypeNames.Text.Html ? "html" : "json";
        return $"{contentName}.{fileExtension}";
    }
}
