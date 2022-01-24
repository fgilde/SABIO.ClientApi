namespace SABIO.ClientApi.Responses.Types
{
    public class Link
    {
        public string BranchId { get; set; }
        public Branch[] Branches { get; set; }
        public string Id { get; set; }
        public string NodeId { get; set; }
        public string ObjectType { get; set; }
        public string Resource { get; set; }
        public string TextId { get; set; }
        public string Title { get; set; }
        public string TreePath { get; set; }
        public string Type { get; set; }
    }
}