using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SABIO.ClientApi.Responses;
using SABIO.ClientApi.Responses.Types;
using SABIO.ClientApi.Types;

namespace SABIO.ClientApi.Core.Api
{
    public class ResourceApi<TDetail, TResult> : ResourceApi<TDetail>
        where TDetail : BaseResult<TResult>
    {
        public Task<SabioResponse<TDetail>> EditAsync(TDetail detail)
        {
            return EditAsync(detail.Result);
        }

        public Task<SabioResponse<TDetail>> EditAsync(string id, TDetail detail)
        {
            return EditAsync(id, detail.Result);
        }

        public Task<SabioResponse<TDetail>> EditAsync(string id, TResult detail)
        {
            Client.Cache?.Clear();
            return Client.PutAsync<SabioResponse<TDetail>>($"{ApiPath.Path}/{id}", EnsureRequiredPropertiesAreSet(detail));
        }

        public Task<SabioResponse<TDetail>> EditAsync(TResult detail)
        {
            // TODO Interface or base class for detail with id
            return EditAsync(typeof(TResult).GetProperty("Id")?.GetValue(detail).ToString(), detail);
        }

        public Task<SabioResponse<TDetail>> CreateAsync(TDetail detail)
        {
            return CreateAsync(detail.Result);
        }

        public Task<SabioResponse<TDetail>> CreateAsync(TResult detail)
        {
            Client.Cache?.Clear();
            return Client.PostAsync<SabioResponse<TDetail>>($"{ApiPath.Path}", EnsureRequiredPropertiesAreSet(detail));
        }

        private T EnsureRequiredPropertiesAreSet<T>(T model)
        {
            // TODO: Model validation with Data Annotation or fluent validations
            return Check.NotNull<T, RequiredParameterAttribute>(() => model);
        }

    }

    public class ResourceApi<TDetail> : ResourceApi
    {
        public Task<SabioResponse<TDetail>> GetAsync(string id)
        {
            return Client.GetAsync<SabioResponse<TDetail>>($"{ApiPath.Path}/{id}");
        }

        public async Task<TDetail> GetFirstAsync()
        {
            var pageResponse = await GetAllAsync(new SearchQuery { Limit = 1 });
            if (pageResponse.Success && pageResponse.Data.Result.Any())
                return (await GetAsync(pageResponse.Data.Result.First().Id)).Data;
            return default;
        }

        public async Task<TDetail[]> GetAllDetailsAsync()
        {
            return (await GetAllExplicitAsync()).Select(r => r.Data).ToArray();
        }

        public async Task<SabioResponse<TDetail>[]> GetAllExplicitAsync(SearchQuery searchQuery = null)
        {
            return await GetAsync((await GetAllAsync(searchQuery)).Data.Result.Select(result => result.Id));
        }

        public async Task<SabioResponse<TDetail>[]> GetAsync(IEnumerable<string> ids)
        {
            var tasks = ids.Select(s => Client.GetAsync<SabioResponse<TDetail>>($"{ApiPath.Path}/{s}")).ToArray();
            await Task.WhenAll(tasks);
            return tasks.Select(task => task.Result).ToArray();
        }

        public Task<SabioResponse<TDetail>[]> GetAsync(string id, params string[] ids)
        {
            return GetAsync(new List<string>(new[] { id }).Concat(ids));
        }

    }

    public class ResourceApi : SabioApiBase
    {
        public ApiPathAttribute ApiPath => GetType().GetCustomAttributes<ApiPathAttribute>().FirstOrDefault();

        public Task<PageResponse<PagedContent<SearchResult>>> GetAllAsync(SearchQuery searchQuery = null, bool ignoreCache = false)
        {
            var apiPathPath = ApiPath?.Path ?? "/search";
            return Client.GetAsync<PageResponse<PagedContent<SearchResult>>>(apiPathPath, searchQuery ?? SearchQuery.Default, ignoreCache);
        }

        public Task DeleteAsync(string id)
        {            
            return Client.DeleteAsync($"{ApiPath?.Path ?? "/search"}/{id}");
        }

        public async Task<SearchResult> GetLastModifiedAsync()
        {            
            return (await GetAllAsync(new SearchQuery(limit: 1)
                    .SortBy(result => result.LastModified, SortDirection.Descending)
                    //.FilterBy(result => result.Resource, "text")
                , true))?.Data?.Result?.FirstOrDefault();            
        }
    }
}