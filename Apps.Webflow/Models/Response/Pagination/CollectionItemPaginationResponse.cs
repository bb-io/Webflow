using Apps.Webflow.Models.Entities;

namespace Apps.Webflow.Models.Response.Pagination;

public class CollectionItemPaginationResponse : IPaginatableResponse<CollectionItemEntity>
{
    public List<CollectionItemEntity> Items { get; set; }

    public PaginationInfo Pagination { get; set; }
}
