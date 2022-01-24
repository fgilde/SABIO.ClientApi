using System;
using System.Linq;
using System.Threading.Tasks;
using SABIO.ClientApi.Extensions;
using SABIO.ClientApi.Responses;

namespace SABIO.ClientApi.Core.Api
{
    public class AuthenticationApi : SabioApiBase
    {
        public event EventHandler<EventArgs> Logout;
        public event EventHandler<EventArgs> Login;

        public bool IsLoggedIn => Client.HttpClient.DefaultRequestHeaders.Contains("sabio-auth-token");
        public string AuthToken => Client.HttpClient.DefaultRequestHeaders.GetValues("sabio-auth-token").FirstOrDefault();

        public async Task<CredentialResponse> LoginAsync(string userName, string password)
        {
            var responseContent = await Client.PostAsync<CredentialResponse>("/authentication/credentials", new { key = password, login = userName, persistent = true, realm = Client.Realm });
            if (responseContent.Success)
                UpdateRequestHeaders(responseContent);

            return responseContent;
        }

        public async Task<CredentialResponse> LoginAsync(string token)
        {
            var responseContent = await Client.PostAsync<CredentialResponse>("/authentication/token", new { key = token, persistent = true, realm = Client.Realm });
            if (responseContent.Success)
                UpdateRequestHeaders(responseContent);
            return responseContent;
        }

        public async Task<User> GetCurrentUserAsync()
        {
            return (await Client.Apis.Config.ConfigAsync()).Data.User;
        }

        public bool IsExternalLoginAvailable(out string loginUrl)
        {
            var clientConfigResult = Client.Api<ConfigApi>().ClientConfigAsync().Result.Data;
            loginUrl = clientConfigResult?.Result?.Authentication?.OpenIdConnect?.AuthorizationEndpoint ?? clientConfigResult?.Result?.Authentication?.LoginPageUrl;
            return !string.IsNullOrEmpty(loginUrl);
        }

        public async Task<bool> LogoutAsync()
        {
            try
            {
                Client.Cache?.Clear();
                var responseContent = await Client.PostAsync<CredentialResponse>("/authentication/logout", null);
                var authentication = (await Client.Api<ConfigApi>().ClientConfigAsync()).Data?.Result?.Authentication;
                if (!string.IsNullOrEmpty(authentication?.LogoutPageUrl))
                    await Client.PostAsync<object>(authentication.LogoutPageUrl, null);
                if (!string.IsNullOrEmpty(authentication?.OpenIdConnect?.EndSessionEndpoint))
                    await Client.GetAsync<object>(authentication.OpenIdConnect.EndSessionEndpoint, null);
                return responseContent.Status.Success;
            }
            catch
            {
                return false;
            }
            finally
            {
                Client.HttpClient.DefaultRequestHeaders.Remove("sabio-auth-token");
                Logout?.Invoke(this, EventArgs.Empty);
            }
        }

        public string AuthorizeUrl(string url)
        {
            url += (url.Contains("?") ? "&" : "?") + "sabio-auth-token=" + AuthToken;
            return url;
        }

        private void UpdateRequestHeaders(CredentialResponse credential)
        {
            Client.HttpClient.DefaultRequestHeaders.AddOrUpdate("sabio-auth-token", credential.Data.Key);
            Login?.Invoke(this, EventArgs.Empty);
        }

    }
}