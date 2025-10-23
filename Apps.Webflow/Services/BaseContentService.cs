using Apps.Webflow.Invocables;
using Apps.Webflow.Models.Entities;
using Apps.Webflow.Models.Request;
using Apps.Webflow.Models.Request.Content;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Webflow.Services;

public abstract class BaseContentService<T>(InvocationContext invocationContext) : WebflowInvocable(invocationContext), IContentService<T> 
{
    public abstract Task<IEnumerable<T>> SearchContent(SiteRequest site, SearchContentRequest input, DateFilter dateFilter);

    protected static void ValidateInputDates(DateFilter date)
    {
        if (!IsCorrectDateRange(date.CreatedBefore, date.CreatedAfter))
            throw new PluginMisconfigurationException("Please specify a valid date range. 'Created after' date cannot be later than the 'Created before' date");

        if (!IsCorrectDateRange(date.LastUpdatedBefore, date.LastUpdatedAfter))
            throw new PluginMisconfigurationException("Please specify a valid date range. 'Last published after' date cannot be later than the 'Last published before' date");
    }

    protected static bool IsCorrectDateRange(DateTime? before, DateTime? after)
    {
        if (!before.HasValue || !after.HasValue)
            return true;

        return after <= before;
    }

    protected IEnumerable<T> ApplyDateFilters<T>(IEnumerable<T> items, DateFilter filter) where T : IDateFilterable
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
