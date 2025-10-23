using Apps.Webflow.Models.Entities;
using Apps.Webflow.Models.Request;

namespace Apps.Webflow.Helper;

public static class FilterHelper
{
    public static IEnumerable<T> ApplyDateFilters<T>(IEnumerable<T> items, DateFilter filter) where T : IDateFilterable
    {
        var result = items;

        if (filter.CreatedAfter.HasValue)
            result = result.Where(x => x.CreatedOn >= filter.CreatedAfter.Value);

        if (filter.CreatedBefore.HasValue)
            result = result.Where(x => x.CreatedOn <= filter.CreatedBefore.Value);

        if (filter.LastUpdatedAfter.HasValue)
            result = result.Where(x => x.LastUpdated >= filter.LastUpdatedAfter.Value);

        if (filter.LastUpdatedBefore.HasValue)
            result = result.Where(x => x.LastUpdated <= filter.LastUpdatedBefore.Value);

        return result;
    }
}
