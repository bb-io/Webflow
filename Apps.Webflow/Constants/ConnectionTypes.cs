namespace Apps.Webflow.Constants;

public class ConnectionTypes
{
    public const string OAuth2 = "OAuth2";
    public const string SiteToken = "Site token";

    public static readonly IEnumerable<string> SupportedConnectionTypes = [OAuth2, SiteToken];
}
