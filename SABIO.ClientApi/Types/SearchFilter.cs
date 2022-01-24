using Newtonsoft.Json;

namespace SABIO.ClientApi.Types
{
    public class SearchFilter
    {
        public SearchFilter(string property, string value) : this()
        {
            Property = property;
            Value = value;
        }

        public SearchFilter()
        { }

        [JsonProperty("property")]
        public string Property { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public class SortOptions
    {
        public SortOptions(string property, SortDirection direction) : this()
        {
            Property = property;
            Direction = direction;
        }

        public SortOptions()
        { }

        [JsonProperty("property")]
        public string Property { get; set; }

        [JsonProperty("direction")]
        public string Direction { get; set; }
    }

    public class SortDirection
    {
        private readonly string _value;

        private SortDirection(string value)
        {
            _value = value;
        }

        public static SortDirection Ascending = new SortDirection("ASC");
        public static SortDirection Descending = new SortDirection("DESC");

        public static implicit operator string(SortDirection v)
        {
            return v._value;
        }
        public static implicit operator SortDirection(string v)
        {
            return new SortDirection(v);
        }
    }
}