using Blackbird.Applications.Sdk.Common;

namespace Apps.Webflow.Models.Response.Components;

public class UpdateComponentContentResponse
{
    [Display("Success")]
    public bool Success { get; set; }

    [Display("Error message")]
    public string? Error { get; set; }
}
