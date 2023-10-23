using Newtonsoft.Json;
using SABIO.ClientApi.Types;

namespace SABIO.ClientApi.Responses.Types
{
    public class TextResult : BaseResult<Text>
    {}

    public class Text 
    {
        public string Id { get; set; }
        [RequiredParameter]
        public string Title { get; set; }
        [RequiredParameter]
        public Branch[] Branches { get; set; }
        [RequiredParameter]
        public User CreatedBy { get; set; }
        [RequiredParameter]
        public Group Group { get; set; }
        [RequiredParameter]
        public Path[][] Paths { get; set; }
        [RequiredParameter]
        public Fragment[] Fragments { get; set; }
        public string Resource { get; set; }
        public string Type { get; set; }
        public string ObjectType { get; set; } = "TextResource";
        public string RealmId { get; set; }
        public string Created { get; set; }
        [JsonProperty("visible")]
        public bool IsVisible { get; set; }
        public string LastModified { get; set; }
        public User LastModifiedBy { get; set; }
        public string TreePath { get; set; }
        public Permission GroupPermission { get; set; }
        public Permission UserPermission { get; set; }
        public Permission OtherPermission { get; set; }
        public int Rating { get; set; }
        public bool Bookmark { get; set; }
        public int ArchivesCount { get; set; }
        public object[] Archives { get; set; }
        public bool Hidden { get; set; }
        public TextTemplate Template { get; set; }
    }

    public class TextTemplate
    {
        public string Id { get; set; }
        public string ObjectType { get; set; }
        public string Title { get; set; }
    }


    public class Fragment 
    {
        public string Id { get; set; }
        [RequiredParameter]
        public Branch[] Branches { get; set; }
        [RequiredParameter]
        public string Content { get; set; }
        public string ObjectType { get; set; } = "TextFragmentResource";
        public string Resource { get; set; }
        public string Type { get; set; }
        public object[] Tags { get; set; }
        public int AttachmentCount { get; set; }
        public string[] AttachedFiles { get; set; } // Document management
        public object[] Attachments { get; set; } // Document management
        public int SubmissionCount { get; set; }
        public string SubmissionId { get; set; }
    }
}