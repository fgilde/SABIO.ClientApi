using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using System.Xml.Linq;
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

        public async Task<List<TreeNode>> CreateNodeStructureAsync(string path, TreeNode parentNode = null, Branch[] branches = null, User user = null, Group group = null)
        {
            var result = new List<TreeNode>();
            var node = parentNode ?? (await TreeAsync()).Data.Result;
            if (node != null && !string.IsNullOrEmpty(path) && path != "/")
            {
                branches ??= node.Branches;
                user ??= await Client.Apis.Authentication.GetCurrentUserAsync();
                group ??= node.Group ?? (await Client.Apis.Texts.GetGroupsAsync(branches)).Data.Result.FirstOrDefault();
                foreach (var segment in path.Split('/').Where(s => !string.IsNullOrWhiteSpace(s)))
                {
                    node = node?.Children?.FirstOrDefault(n => n.Title == segment) ?? await CreateNodeAsync(segment, node, branches, user, group);
                    if(node != null)
                        result.Add(node);
                }
            }

            return result;
        }

        public async Task<TreeNode> CreateNodeAsync(string title, TreeNode parentNode = null, Branch[] branches = null, User user = null, Group group = null)
        {
            var res = await CreateNodeAsync(new TreeNode { Title = title, Group = group, CreatedBy = user, Branches = branches }, parentNode);
            if (res?.Success == true)
            {
                var node = await FindNodeAsync(res.Data.Result.Id);
                node.NewlyCreated = true;
                return node;
            }
            return null;
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