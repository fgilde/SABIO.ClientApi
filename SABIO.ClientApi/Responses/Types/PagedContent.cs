namespace SABIO.ClientApi.Responses.Types
{
    public class PagedContent
    {
        public int Start { get; set; }
        public int Limit { get; set; }
        public int Total { get; set; }
        public Filter[] Filter { get; set; }
    }

    public class PagedContent<TContentResult> : PagedContent
        where TContentResult : SearchResult
    {
        public TContentResult[] Result { get; set; }
    }
}