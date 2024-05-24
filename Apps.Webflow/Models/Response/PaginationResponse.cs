namespace Apps.Webflow.Models.Response;

public class PaginationResponse<T>
{
    public IEnumerable<T> Items { get; set; }
    
    public PaginationInfo Pagination { get; set; }
}