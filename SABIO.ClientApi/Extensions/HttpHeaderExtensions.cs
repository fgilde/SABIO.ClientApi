using System.Net.Http.Headers;

namespace SABIO.ClientApi.Extensions
{
    public static class HttpHeaderExtensions
    {
        public static void AddOrUpdate(this HttpRequestHeaders headers, string name, string value)
        {
            if (headers.Contains(name))
                headers.Remove(name);
            headers.Add(name, value);
        }
    }
}