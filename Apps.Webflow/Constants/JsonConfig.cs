using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Apps.Webflow.Constants;

public static class JsonConfig
{
    public static JsonSerializerSettings Settings => new()
    {
        ContractResolver = new DefaultContractResolver()
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        },
        NullValueHandling = NullValueHandling.Ignore,
        Formatting = Formatting.Indented,
    };
}