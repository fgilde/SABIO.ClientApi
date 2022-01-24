using SABIO.ClientApi.Responses.Types;
using SABIO.ClientApi.Types;

namespace SABIO.ClientApi.Core.Api
{
    [ApiPath("/text")]
    public class TextsApi : ResourceApi<TextResult, Text>
    {}

}