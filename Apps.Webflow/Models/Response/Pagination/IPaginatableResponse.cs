namespace Apps.Webflow.Models.Response.Pagination;

public interface IPaginatableResponse<T>
{
    PaginationInfo Pagination { get; set; }
}