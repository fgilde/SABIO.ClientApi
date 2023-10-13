using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using SABIO.ClientApi.Core.Api;
using SABIO.ClientApi.Extensions;

namespace SABIO.ClientApi.Core
{
    public class SabioClient
    {
        private HttpClient client;
        private readonly ConcurrentDictionary<Type, SabioApiBase> apis = new ConcurrentDictionary<Type, SabioApiBase>();

        public CacheProvider Cache { get; private set; }
        public string Realm { get; set; }
        public Uri BaseUrl { get; set; }
        public HttpClient HttpClient => client ?? (client = CreateHttpClient());
        public Apis Apis { get; }
        public bool IsCachingEnabled => Cache != null;
        public bool IsLoggedIn => Api<AuthenticationApi>().IsLoggedIn;
        public Task<bool> CanWorkAsync() => Api<ConfigApi>().IsConfigAvailableAsync();

        public SabioClient(string url, string realm = "")
            : this(new Uri(url.ToLower().StartsWith("http") ? url : $"https://{url}"), realm)
        { }

        public SabioClient(Uri baseUrl, string realm = "")
        {
            BaseUrl = EnsureServiceUrl(baseUrl, out string detectedRealm);
            Realm = !string.IsNullOrEmpty(realm) ? realm : detectedRealm;
            Apis = new Apis(this);
            if (string.IsNullOrEmpty(Realm))
                Realm = Apis.Config.ClientConfigAsync().Result.Data.Result.Realm;
        }

        public SabioClient EnableAutomaticCaching(IMemoryCache cache = null)
        {
            Cache = new CacheProvider(cache).ClearWhen(c =>
            {
                return Check.TryCatch(() =>
                {
                    var modified = Apis.Authentication.IsLoggedIn ? Apis.ResourceApi.GetLastModifiedAsync().Result : null;
                    return modified?.LastModifiedDate > c.LastWriteTime || modified?.CreatedDate > c.LastWriteTime;
                });
            });
            return this;
        }

        public SabioClient DisableAutomaticCaching()
        {
            Cache?.Clear();
            Cache = null;
            return this;
        }

        public Task<TResponse> PutAsync<TResponse>(string path, object httpContent = null, object getParams = null) where TResponse : class
        {
            return PutAsync<TResponse>(path, httpContent?.ToStringContent(), getParams);
        }

        public Task<TResponse> PutAsync<TResponse>(string path, HttpContent httpContent = null, object getParams = null) where TResponse : class
        {
            return HttpClient.PutAsync(BuildPath(path, getParams), httpContent).ReadAsAsync<TResponse>();
        }

        public Task<TResponse> PostAsync<TResponse>(string path, object httpContent = null, object getParams = null) where TResponse : class
        {
            var stringContent = httpContent?.ToStringContent();
            return PostAsync<TResponse>(path, stringContent, getParams);
        }

        public Task<TResponse> PostAsync<TResponse>(string path, HttpContent httpContent = null, object getParams = null) where TResponse : class
        {
            return HttpClient.PostAsync(BuildPath(path, getParams), httpContent).ReadAsAsync<TResponse>();
        }

        public Task<TResponse> GetAsync<TResponse>(string path, object getParams = null, bool ignoreCache = false) where TResponse : class
        {
            string fullPath = BuildPath(path, getParams);
            if (!ignoreCache && IsCachingEnabled)
                return HttpClient.ExecuteWithCache(Cache, c => c.GetAsync(fullPath)).ReadAsAsync<TResponse>();

            return HttpClient.GetAsync(fullPath).ReadAsAsync<TResponse>();
        }

        public Task DeleteAsync(string path, object getParams = null)
        {
            return HttpClient.DeleteAsync(BuildPath(path, getParams));
        }

        public T Api<T>() where T : SabioApiBase, new()
        {
            return Api(typeof(T)) as T;
        }

        public SabioApiBase Api(Type type)
        {
            return apis.GetOrAdd(type, t => Activator.CreateInstance(t) as SabioApiBase).SetProperties(a => a.Client = this);
        }

        private string BuildPath(string path, object getParams = null)
        {
            var result = BaseUrl + path;
            result += getParams?.ToQueryString(result.Contains("?") ? "&" : "?");
            return result;
        }

        private HttpClient CreateHttpClient()
        {
            var result = new HttpClient();
            result.BaseAddress = BaseUrl;
            result.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            result.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36");
            return result;
        }

        private Uri EnsureServiceUrl(Uri baseUrl, out string detectedRealm)
        {
            // TODO: Currently horrorble implementation, but supports old and new sabio urls as well
            detectedRealm = "";
            var builder = new UriBuilder(baseUrl);
            var paths = builder.Path.Split('/').Where(s => !string.IsNullOrEmpty(s)).ToList();
            if (paths.Contains("client") && paths.IndexOf("client") == paths.Count - 2) // last part is then realm name
            {
                detectedRealm = paths.Last();
                paths.RemoveRange(paths.Count - 2, 2);
            }
            if (!paths.Any() || paths.Last() != "services")
            {
                paths.Clear();
                paths.AddRange(
                    CreateHttpClient().GetAsync($"{builder.Scheme}://{builder.Host}/sabio/services/_client").Result
                        .StatusCode == HttpStatusCode.OK
                        ? new[] { "sabio", "services" }
                        : new[] { "sabio-web", "services" });
            }
            builder.Path = string.Join("/", paths);
            return builder.Uri;
        }
    }
}