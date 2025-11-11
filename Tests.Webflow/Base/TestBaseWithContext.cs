using Blackbird.Applications.Sdk.Common.Dynamic;
using Newtonsoft.Json;

namespace Tests.Webflow.Base;

public class TestBaseWithContext : TestBase
{
    public new TestContext TestContext
    {
        get => base.TestContext!;
        set => base.TestContext = value;
    }

    protected void PrintResult(object result)
    {
        TestContext?.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
    }

    protected void PrintDataHandlerResult(IEnumerable<DataSourceItem> items)
    {
        foreach (var item in items)
            TestContext?.WriteLine($"ID: {item.Value}, Display name: {item.DisplayName}");
    }
}
