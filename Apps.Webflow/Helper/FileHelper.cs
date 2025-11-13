using Apps.Webflow.Constants;

namespace Apps.Webflow.Helper;

public static class FileHelper
{
    public static string GetDownloadedFileName(string contentName, string fileFormat)
    {
        string fileExtension = fileFormat == ContentFormats.InteroperableHtml ? "html" : "json";
        return $"{contentName}.{fileExtension}";
    }
}
