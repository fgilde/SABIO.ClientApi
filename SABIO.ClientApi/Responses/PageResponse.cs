using SABIO.ClientApi.Responses.Types;

namespace SABIO.ClientApi.Responses
{
    public class PageResponse<TContent> : SabioResponse<TContent> 
        where TContent: PagedContent
    {}

}