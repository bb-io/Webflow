using Apps.Webflow.Models.Entities.Page;

namespace Apps.Webflow.Models.Response.Pagination;

public class PagesPaginationResponse : IPaginatableResponse<PageEntity>
{
    public List<PageEntity> Pages { get; set; }

    public PaginationInfo Pagination { get; set; }
}
