using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SABIO.Blazor.Extensions;
using SABIO.ClientApi;
using SABIO.ClientApi.Core;
using SABIO.ClientApi.Core.Api;
using SABIO.ClientApi.Extensions;
using SABIO.ClientApi.Responses.Types;

namespace SABIO.Blazor.Pages
{
    public partial class Login
    {
        [Parameter] public string loginToken { get; set; }

        private bool serverAddressHidden = true;
        private bool loginFailed = false;
        private bool showUserAndPass = true;
        private string name;
        private string password;
        ClientConfig clientConfig;


        protected override void OnInitialized()
        {
            base.OnInitialized();
            //UpdateUI();
        }

        private void OnKeyUp(KeyboardEventArgs args)
        {
            if (args.Key == "Enter")
                OnLogin();
        }

        private void SetProperty(params Action<SabioClient>[] actions)
        {
            this.sabioClient.SetProperties(actions);
            UpdateUI();
        }

        private void onRealmChange(ChangeEventArgs args)
        {
            SetProperty(c => c.Realm = args.Value.ToString());
        }

        private void onServerChange(ChangeEventArgs args)
        {
            SetProperty(c => c.BaseUrl = new Uri($"{(args.Value.ToString().Contains("http") ? "" : "https://")}{args.Value}"));
        }

        private async void OnLogin()
        {
            this.loginFailed = false;
            var result = await sabioClient.ExecuteLoginAsync(runtime, new LoginArgs() { Token = loginToken, User = name, Password = password });
            url.NavigateWhen("/", !(loginFailed = !(result ?? true)));
        }

        private void UpdateUI()
        {
            clientConfig = Check.TryCatch(() => sabioClient.Api<ConfigApi>().ClientConfigAsync().Result?.Data.Result);
            showUserAndPass = !clientConfig?.WebSSO ?? false;
        }

        private void ShowServer()
        {
            serverAddressHidden = !serverAddressHidden;
        }
    }
}