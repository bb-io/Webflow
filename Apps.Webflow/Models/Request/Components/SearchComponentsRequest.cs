using Blackbird.Applications.Sdk.Common;

namespace Apps.Webflow.Models.Request.Components;

public class SearchComponentsRequest
{
    [Display("Name contains")]
    public string? NameContains { get; set; }

    [Display("Group contains")]
    public string? GroupContains { get; set; }

    [Display("Description contains")]
    public string? DescriptionContains { get; set; }

    [Display("Include read only components")]
    public bool? IncludeReadOnly { get; set; }
}
