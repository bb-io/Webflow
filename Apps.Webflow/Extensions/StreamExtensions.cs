using System.Security.Cryptography;

namespace Apps.Webflow.Extensions;

public static class StreamExtensions
{
    public static string CalculateMd5(this Stream stream)
    {
        using var md5 = MD5.Create();
        var hash = md5.ComputeHash(stream);
        stream.Position = 0;
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}