using SABIO.ClientApi.Core;

namespace SABIO.Api.Tests.Helper.Facades;

public class FacadeBase
{
    public SabioClient Client { get; private set; }
    public FacadeBase(SabioClient client)
    {
        Client = client;
    }
}