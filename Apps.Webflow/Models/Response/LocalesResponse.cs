using Apps.Webflow.Models.Entities;

namespace Apps.Webflow.Models.Response;

public class LocalesResponse
{
    public LocaleEntity Primary { get; set; }
    public IEnumerable<LocaleEntity> Secondary { get; set; }
}