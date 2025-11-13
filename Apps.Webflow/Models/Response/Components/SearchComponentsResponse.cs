using Apps.Webflow.Models.Entities.Component;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Webflow.Models.Response.Components;

public class SearchComponentsResponse(IEnumerable<ComponentEntity> components)
{
    [Display("Components")]
    public IEnumerable<ComponentEntity> Components { get; set; } = components;
}
