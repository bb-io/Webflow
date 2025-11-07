using Apps.Webflow.Models.Request.Date;
using Blackbird.Applications.Sdk.Common.Exceptions;

namespace Apps.Webflow.Helper;

public static class ValidatorHelper
{
    public static void ValidateInputDates(IDateFilter date)
    {
        if (!IsCorrectDateRange(date.CreatedBefore, date.CreatedAfter))
            throw new PluginMisconfigurationException("Please specify a valid date range. 'Created after' date cannot be later than the 'Created before' date");

        if (!IsCorrectDateRange(date.LastUpdatedBefore, date.LastUpdatedAfter))
            throw new PluginMisconfigurationException("Please specify a valid date range. 'Last updated after' date cannot be later than the 'Last upodated before' date");
    }

    public static void ValidatePublishedInputDates(DateTime? lastPublishedBefore, DateTime? lastPublishedAfter)
    {
        if (!IsCorrectDateRange(lastPublishedBefore, lastPublishedAfter))
            throw new PluginMisconfigurationException("Please specify a valid date range. 'Last published after' date cannot be later than the 'Last published before' date");
    }

    private static bool IsCorrectDateRange(DateTime? before, DateTime? after)
    {
        if (!before.HasValue || !after.HasValue)
            return true;

        return after <= before;
    }
}
