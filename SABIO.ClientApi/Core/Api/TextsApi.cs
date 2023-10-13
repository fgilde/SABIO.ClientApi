using System.Linq;
using SABIO.ClientApi.Responses;
using SABIO.ClientApi.Responses.Types;
using SABIO.ClientApi.Types;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SABIO.ClientApi.Core.Api
{
    [ApiPath("/text")]
    public class TextsApi : ResourceApi<TextResult, Text>
    {
        public Task<SabioResponse<BaseResult<Group[]>>> GetGroupsAsync(params Branch[] branches)
        {
            return Client.PostAsync<SabioResponse<BaseResult<Group[]>>>($"{ApiPath.Path}/_options/group", new
            {
                q = "*",
                filter = JsonConvert.SerializeObject(branches.Select(b => new SearchFilter("branchIds", b.Id)).ToArray())
            });
        }
    }

}