namespace SABIO.ClientApi.Responses.Types
{
    public class Path 
    {
        public Path()
        {}

        public Path(TreeNode node)
        {
            Branches = node.Branches;
            Title = node.Title;
            Id = node.Id;
        }
        public string Id { get; set; }

        public Branch[] Branches { get; set; }
        public string Title { get; set; }
    }
}