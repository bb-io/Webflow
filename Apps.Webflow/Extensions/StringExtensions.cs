namespace Apps.Webflow.Extensions;

public static class StringExtensions
{
    public static string ToKebabCase(this string text)
    {
        return text.ToLower().Replace(' ', '-');
    }
}
