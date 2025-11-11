using Newtonsoft.Json;

namespace Apps.Webflow.Services;

public class JsonHelper
{
    public static bool IsJson(string jsonString)
    {
        if (string.IsNullOrWhiteSpace(jsonString))
            return false;

        using var stringReader = new StringReader(jsonString);
        using var jsonReader = new JsonTextReader(stringReader);
        try
        {
            while (jsonReader.Read()) { }
            return true;
        }
        catch (JsonReaderException)
        {
            return false;
        }
    }
}
