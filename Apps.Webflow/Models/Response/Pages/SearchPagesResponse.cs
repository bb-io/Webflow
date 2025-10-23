using Apps.Webflow.Models.Entities;

namespace Apps.Webflow.Models.Response.Pages;

public class SearchPagesResponse(IEnumerable<PageEntity> pages)
{
    public IEnumerable<PageEntity> Pages { get; set; } = pages;
}
