namespace Apps.Webflow.Models.Entities;

public interface IDateFilterable
{
    DateTime? CreatedOn { get; }
    DateTime? LastUpdated { get; }
}
