using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SABIO.ClientApi.Core.Api;
using SABIO.ClientApi.Responses.Types;
using SABIO.ClientApi.Types;

namespace SABIO.Blazor.Components
{
    public class ResourceListComponent<TApi, TDetail> : ComponentBase
        where TApi : ResourceApi<TDetail>
    {
        [Inject]
        public TApi Api { get; set; }

        [Inject]
        public NavigationManager UriHelper { get; set; }

        public static string Query { get; set; }

        [Parameter]
        public string Id { get; set; }

        public static SearchResult[] Results { get; private set; }

        public TDetail Detail { get; set; }

        public Task LoadAll()
        {
            return Load(true);
        }

        public Task Load()
        {
            return Load(false);
        }

        public async Task Load(bool all)
        {
            if (EnsureLoggedIn())
            {
                Results = Results == null || !Results.Any() || all
                    ? await GetResults(all ? new SearchQuery(limit: -1) : SearchQuery.Default)
                    : Results.Concat(await GetResults(new SearchQuery
                        {Limit = SearchQuery.Default.Limit, Start = Results.Length})).ToArray();
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            if (EnsureLoggedIn() && Id != null)
                Detail = (await Api.GetAsync(Id)).Data;
        }

        private bool EnsureLoggedIn()
        {
            if (!Api.Client.IsLoggedIn && !UriHelper.Uri.Contains("login"))
            {
                UriHelper.NavigateTo("login");
                return false;
            }

            return true;
        }

        public async Task<SearchResult[]> GetResults(SearchQuery query)
        {
            query.Term = Query;            
            return (await Api.GetAllAsync(query)).Data.Result;
        }


        protected override async Task OnInitializedAsync()
        {
            if (Results == null)
                await Load();
        }

        protected void CheckEnterAndLoad(KeyboardEventArgs args)
        {
            if (args.Code == "Enter" || args.Code == "NumpadEnter")
                LoadAll();
        }
    }
}