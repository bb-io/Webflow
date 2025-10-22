using Apps.Webflow.Constants;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Webflow.DataSourceHandlers.Content;

public class ContentTypeStaticDataHandler : IDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData(DataSourceContext context)
    {
        return new List<DataSourceItem>
        {
            new DataSourceItem(ContentTypes.Page, ContentTypes.Page),
            new DataSourceItem(ContentTypes.Component, ContentTypes.Component)
        };
    }
}
