using System;
using System.Linq;
using System.Threading.Tasks;
using SABIO.ClientApi.Extensions;
using SABIO.ClientApi.Responses;
using SABIO.ClientApi.Responses.Types;

namespace SABIO.ClientApi.Core.Api
{
    public class TreeApi : SabioApiBase
    {
        public Task<SabioResponse<TreeResult>> CreateNodeAsync(TreeNode newNode, TreeNode parent = null, int position = default)
        {
            TreeNode prev = null;
            TreeNode next = null;
            if (parent != null)
            {
                newNode.ParentId = parent.Id;
                newNode.Branches = newNode.Branches ?? parent.Branches;
                if (position >= 0)
                {
                    position = Math.Min(position, parent.Children.Length);
                    prev = position > 0 ? parent.Children[Math.Max(0, position - 1)]: null;
                    next = parent?.Children?.Any() == true ? Check.TryCatch(() => parent.Children[position]) : null;
                }
            }
            return CreateNodeAsync(newNode, prev?.Id, next?.Id);
        }
        public Task<SabioResponse<TreeResult>> CreateNodeAsync(TreeNode newNode, string previousSiblingNodeId, string nextSiblingNodeId = null)
        {
            return Client.PostAsync<SabioResponse<TreeResult>>($"/tree/", newNode, new
            {
                nextSiblingNodeId, previousSiblingNodeId
            });
        }

        public Task DeleteNodeAsync(string nodeId)
        {
            return Client.DeleteAsync($"/tree/{nodeId}");
        }

        public async Task<TreeNode> FindNodeAsync(string nodeId)
        {
            // return (await Client.GetAsync<SabioResponse<TreeNode>>($"/tree/findOne/{nodeId}", defaultGetParams)).Data;
            return new[] {(await TreeAsync()).Data.Result}.Recursive(node => node.Children, node => node.Id == nodeId).FirstOrDefault();
        }

        public Task<SabioResponse<TreeResult>> TreeAsync(int treeId = 0)
        {
            return Client.GetAsync<SabioResponse<TreeResult>>($"/tree/{treeId}", defaultGetParams)
                .ContinueWith(task => AddParentInformations(task.Result));
        }

        private SabioResponse<TreeResult> AddParentInformations(SabioResponse<TreeResult> response)
        {
            response.Data.TreeNodeCount = response.Data.Result.Children.Apply(treeNode => treeNode.ParentNode = response.Data.Result) 
                .Recursive(node => node.Children.Apply(treeNode => treeNode.ParentNode = node)).Count(); // Important we need a toList, toArray or count call to force enumeration
            return response;
        }

        private readonly object defaultGetParams = new {
            fields = "attachmentCount,children,id,leaf,title,branches",
            filter = @"[{""property"":""depths"",""value"":-1},{""property"":""visible"",""value"":true}]",
            q = "*"
        };
    }
}