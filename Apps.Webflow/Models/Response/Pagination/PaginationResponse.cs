namespace Apps.Webflow.Models.Response.Pagination;

public class PaginationResponse<T> : IPaginatableResponse<T>
{
    public IEnumerable<T> Items { get; set; }
    
    public PaginationInfo Pagination { get; set; }
}