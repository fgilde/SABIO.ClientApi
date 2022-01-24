using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using SABIO.ClientApi.Extensions;
using SABIO.ClientApi.Types;

namespace SABIO.ClientApi.Responses.Types
{
    public class TreeResult : BaseResult<TreeNode>
    {
        public int TreeNodeCount { get; set; }
    }

    [DebuggerDisplay("[TreeNode] Title={Title}")]
    public class TreeNode
    {
        private TreeNode[] path;
        private string _parentId;

        public string Id { get; set; }
        public string ObjectType { get; set; } = "TreeResource";
        public Permission UserPermission { get; set; }
        public int AttachmentCount { get; set; }
        public int AllAttachmentCount { get; set; }
        public bool CanAddAttachment { get; set; }
        public TreeNode[] Children { get; set; }
        public bool Leaf { get; set; }
        [RequiredParameter]
        public string Title { get; set; }
        [JsonProperty("visible")]
        public bool IsVisible { get; set; }
        [RequiredParameter]
        public Branch[] Branches { get; set; }
        [RequiredParameter]
        public User CreatedBy { get; set; }
        [RequiredParameter]
        public Group Group { get; set; }
        [RequiredParameter]
        public string ParentId
        {
            get => _parentId ?? ParentNode.Id;
            set => _parentId = value;
        }

        [JsonIgnore]
        public bool ContainsResources => Leaf && AttachmentCount > 0;
        [JsonIgnore]
        public IEnumerable<TreeNode> Siblings => ParentNode?.Children.Where(n => n.Id != Id);
        [JsonIgnore]
        public TreeNode ParentNode { get; set; }
        [JsonIgnore]
        public bool IsRoot => Title == "ROOT" && ParentNode == null;
        [JsonIgnore]
        public TreeNode[] Path => path ?? (path = new[] { ParentNode }
                                      .Recursive(node => node?.ParentNode != null ? new[] { node.ParentNode } : new TreeNode[0])
                                      .Concat(new[] { this })
                                      .Where(node => node != null).ToArray());
        [JsonIgnore]
        public string PathString => string.Join("\\", Path.Select(node => node.Title));




    }
}