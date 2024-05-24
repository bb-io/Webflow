using Blackbird.Applications.Sdk.Common;

namespace Apps.Webflow.Models.Entities;

public class FieldEntity
{
    [Display("Field ID")] public string Id { get; set; }

    public string Type { get; set; }

    public string Slug { get; set; }

    [Display("Display name")] public string DisplayName { get; set; }

    [Display("Help text")] public string HelpText { get; set; }

    [Display("Is editable")] public bool IsEditable { get; set; }

    [Display("Is required")] public bool IsRequired { get; set; }
}