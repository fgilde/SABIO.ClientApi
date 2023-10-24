using SABIO.Api.Tests.Helper.Facades;
using SABIO.ClientApi.Core;
using SABIO.ClientApi.Responses;

namespace SABIO.Api.Tests.Helper
{
    public static class TestExtensions
    {
        public static CredentialResponse Login(this SabioClient client, TestUser user)
        {
            if (client.IsLoggedIn)
                client.Apis.Authentication.LogoutAsync().Wait();
            return client.Apis.Authentication.LoginAsync(user.Name, user.Password).Result;
        }
    }
}