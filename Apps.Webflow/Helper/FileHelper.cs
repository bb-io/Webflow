using Apps.Webflow.Constants;

namespace Apps.Webflow.Helper;

public static class FileHelper
{
    public static string GetDownloadedFileName(string contentType, string contentId, string contentName, string fileFormat)
    {
        string fileExtension = fileFormat == ContentFormats.InteroperableHtml ? "html" : "json";
        string contentTypeLowerCase = contentType.ToLower().Replace(' ', '_');
        string contentNameLowerCase = contentName.ToLower().Replace(' ', '_');
        return $"{contentTypeLowerCase}_{contentId}_{contentNameLowerCase}.{fileExtension}";
    }
}
