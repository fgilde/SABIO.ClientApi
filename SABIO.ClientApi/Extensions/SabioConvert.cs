using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SABIO.ClientApi.Responses;
using SABIO.ClientApi.Responses.Types;
using SABIO.ClientApi.Types;

namespace SABIO.ClientApi.Extensions
{
    public static class SabioConvert
    {
        public static Path[][] ToPathsArray(this IEnumerable<TreeNode> nodes)
        {
            var treeNodes = nodes as TreeNode[] ?? nodes.ToArray();
            Path[][] paths = new Path[treeNodes.Length][];
            for (int i = 0; i < treeNodes.Length; i++)
                paths[i] = treeNodes[i].Path.Select(n => new Path(n)).ToArray();

            return paths;
        }

        public static IEnumerable<Branch> GetUniqueBranches(this IEnumerable<TreeNode> nodes)
        {
            return nodes.SelectMany(node => node.Branches).Distinct(new PropertyComparer<Branch>(b => b.Id));
        }

        public static DateTime? ParseSabioDate(string dateString)
        {
            if (!string.IsNullOrWhiteSpace(dateString))
            {
                dateString = dateString.Contains("GMT") ? dateString.Substring(0, dateString.IndexOf("GMT") - 1) : dateString;
                return DateTime.TryParseExact(dateString, new[] { "ddd MMM dd yyyy HH:mm:ss" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result) ? result : (DateTime?)null;
            }

            return null;
        }
    }
}