namespace Apps.Webflow.Models.Request.Date;

// We need this interface to create the same date filter classes but with different display names
public interface IDateFilter
{
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
    public DateTime? LastUpdatedAfter { get; set; }
    public DateTime? LastUpdatedBefore { get; set; }
}
