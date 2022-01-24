namespace SABIO.ClientApi.Responses.Types
{
    public class Status
    {
        public int Code { get; set; }
        public int HttpStatus { get; set; }
        public string Text { get; set; }
        public bool Success { get; set; }
        public string RequestId { get; set; }
        public string[] Details { get; set; }
    }
}