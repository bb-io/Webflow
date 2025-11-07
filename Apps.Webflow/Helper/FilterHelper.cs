using Apps.Webflow.Models.Entities;
using Apps.Webflow.Models.Request.Date;

namespace Apps.Webflow.Helper;

public static class FilterHelper
{
    public static IEnumerable<T> ApplyDateFilters<T>(IEnumerable<T> items, IDateFilter filter) where T : IDateFilterable
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

    public static IEnumerable<T> ApplyContainsFilter<T>(
        IEnumerable<T> items,
        string? contains,
        Func<T, string?> selector)
    {
        if (string.IsNullOrWhiteSpace(contains))
            return items;

        return items.Where(x =>
            !string.IsNullOrEmpty(selector(x)) &&
            selector(x)!.Contains(contains, StringComparison.OrdinalIgnoreCase));
    }

    public static IEnumerable<T> ApplyDoesNotContainFilter<T>(
        IEnumerable<T> items,
        string? doesNotContain,
        Func<T, string?> selector)
    {
        if (string.IsNullOrWhiteSpace(doesNotContain))
            return items;

        return items.Where(x =>
            string.IsNullOrEmpty(selector(x)) ||
            !selector(x)!.Contains(doesNotContain, StringComparison.OrdinalIgnoreCase));
    }

    public static IEnumerable<T> ApplyBooleanFilter<T>(
        IEnumerable<T> items,
        bool? value,
        Func<T, bool?> selector)
    {
        if (!value.HasValue)
            return items;

        return items.Where(x => selector(x).HasValue && selector(x)!.Value == value.Value);
    }
}
