using System.Threading.Tasks;
using SABIO.ClientApi.Responses;
using SABIO.ClientApi.Responses.Types;

namespace SABIO.ClientApi.Core.Api
{
    public class ConfigApi : SabioApiBase
    {
        private ConfigResponse cachedConfig;

        protected override void ClientChanged()
        {
            Client.Apis.Authentication.Logout += (sender, args) => cachedConfig = null;
        }

        public Task<ConfigResponse> ConfigAsync(bool forceReload = false)
        {
            return cachedConfig != null && !forceReload ? Task.FromResult(cachedConfig) :
                Client.GetAsync<ConfigResponse>("/config").ContinueWith(task => cachedConfig = task.Result);
        }

        public Task<SabioResponse<ClientConfigResult>> ClientConfigAsync()
        {
            return Client.GetAsync<SabioResponse<ClientConfigResult>>($"/_client/{Client.Realm}");
        }

        public async Task<bool> IsConfigAvailableAsync()
        {
            try
            {
                var sabioResponse = await ClientConfigAsync();
                return sabioResponse.Success && sabioResponse.Data?.Result != null;
            }
            catch
            {
                return false;
            }
        }
    }
}