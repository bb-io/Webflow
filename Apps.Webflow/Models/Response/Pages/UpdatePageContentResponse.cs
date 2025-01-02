using Blackbird.Applications.Sdk.Common;

namespace Apps.Webflow.Models.Response.Pages
{
    public class UpdatePageContentResponse
    {
        [Display("Success")]
        public bool Success { get; set; }

        public string? Error { get; set; }
    }
}
