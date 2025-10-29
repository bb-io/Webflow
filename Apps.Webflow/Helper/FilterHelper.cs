using Apps.Webflow.Models.Entities;
using Apps.Webflow.Models.Request;

namespace Apps.Webflow.Helper;

public static class FilterHelper
{
    public static IEnumerable<T> ApplyDateFilters<T>(IEnumerable<T> items, DateFilter filter) where T : IDateFilterable
    {
        if (filter.CreatedAfter.HasValue)
            items = items.Where(x => x.CreatedOn >= filter.CreatedAfter.Value);

        if (filter.CreatedBefore.HasValue)
            items = items.Where(x => x.CreatedOn <= filter.CreatedBefore.Value);

        if (filter.LastUpdatedAfter.HasValue)
            items = items.Where(x => x.LastUpdated >= filter.LastUpdatedAfter.Value);

        if (filter.LastUpdatedBefore.HasValue)
            items = items.Where(x => x.LastUpdated <= filter.LastUpdatedBefore.Value);

        return items;
    }
}
