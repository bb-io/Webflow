using Apps.Webflow.Constants;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Webflow.DataSourceHandlers.Content;

public class ContentTypeStaticDataHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData()
    {
        return new List<DataSourceItem>
        {
            new DataSourceItem(ContentTypes.Page, ContentTypes.Page),
            new DataSourceItem(ContentTypes.Component, ContentTypes.Component)
        };
    }
}
