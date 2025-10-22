using Apps.Webflow.Models.Entities;

namespace Apps.Webflow.Models.Response.Content;

public class SearchContentResponse(IEnumerable<ContentItemEntity> result)
{
    public IEnumerable<ContentItemEntity> Result { get; set; } = result;
}
