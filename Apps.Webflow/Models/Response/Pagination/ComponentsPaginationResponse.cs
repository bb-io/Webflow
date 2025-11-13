using Apps.Webflow.Models.Entities.Component;

namespace Apps.Webflow.Models.Response.Pagination;

public class ComponentsPaginationResponse : IPaginatableResponse<ComponentEntity>
{
    public List<ComponentEntity> Components { get; set; }

    public PaginationInfo Pagination { get; set; }
}