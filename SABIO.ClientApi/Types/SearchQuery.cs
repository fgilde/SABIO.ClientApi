using System;
using System.Linq;
using System.Linq.Expressions;
using Newtonsoft.Json;
using SABIO.ClientApi.Extensions;
using SABIO.ClientApi.Responses.Types;

namespace SABIO.ClientApi.Types
{
    public class SearchQuery
    {
        public static SearchQuery Default => new SearchQuery();

        public SearchQuery(string term = "", int start = 0, int limit = 25)
        {
            Term = term;
            Start = start;
            Limit = limit;
        }

        [JsonProperty("q")]
        public string Term { get; set; }
        public int Start { get; set; }        
        public int Limit { get; set; }        

        [JsonProperty("sort")]
        public SortOptions[] Sortings { get; set; }

        [JsonProperty("filter")]
        public SearchFilter[] Filters { get; set; }

        public SearchQuery SortBy(Expression<Func<SearchResult, object>> expression, SortDirection direction)
        {            
            return SortBy(new SortOptions(expression.GetMemberName(), direction));
        }

        public SearchQuery FilterBy(Expression<Func<SearchResult, object>> expression, string value)
        {            
            return FilterBy(new SearchFilter(expression.GetMemberName(), value));
        }

        public SearchQuery SortBy(SortOptions options)
        {
            var sorts = (Sortings ?? new SortOptions[0]).ToList();
            sorts.Add(options);
            Sortings = sorts.ToArray();
            return this;
        }

        public SearchQuery FilterBy(SearchFilter filter)
        {
            var filters = (Filters ?? new SearchFilter[0]).ToList();
            filters.Add(filter);
            Filters = filters.ToArray();
            return this;
        }
    }
    
}