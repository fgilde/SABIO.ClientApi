using SABIO.ClientApi.Responses.Types;
using SABIO.ClientApi.Types;

namespace SABIO.ClientApi.Core.Api
{
    [ApiPath("/motd")]
    public class PinboardsApi : ResourceApi<PinboardResult, Pinboard>
    { } 

}