using System.Web;
using Microsoft.JSInterop;
using SABIO.ClientApi.Core;
using SABIO.ClientApi.Core.Api;
using SABIO.ClientApi.Extensions;
using SABIO.ClientApi.Responses;
using SABIO.ClientApi.Responses.Types;

namespace SABIO.Blazor.Extensions
{
    public static class SabioClientHelper
    {
        internal static LoginArgs UsedLoginArgs { get; set; } // Just for faking doc view
        public static async Task<bool?> ExecuteLoginAsync(this SabioClient sabioClient, IJSRuntime runtime, LoginArgs args = null)
        {
            args = args ?? UsedLoginArgs;
            UsedLoginArgs = args;
            var clientConfig = (await sabioClient.Api<ConfigApi>().ClientConfigAsync())?.Data.Result;
            if (string.IsNullOrEmpty(args.Token) && (clientConfig?.WebSSO ?? false))
            {
                var _url = clientConfig.Authentication.LoginPageUrl;                
                await runtime.InvokeAsync<bool>("loginWebSso", _url);                
                return null;
            }
            CredentialResponse result;
            if (!string.IsNullOrEmpty(args.Token))
            {
                result = await sabioClient.Apis.Authentication.LoginAsync(args.Token);
            }
            else
            {
                result = await sabioClient.Apis.Authentication.LoginAsync(args.User, args.Password);
            }
            return result?.Success ?? false;
        }

        public static string CreateViewerUrl(this Document document, SabioClient client)
        {
            //var res = $"http://localhost:52470/Document/Display?docId={document.Id}&realm={client.Realm}&server={client.BaseUrl}{UsedLoginArgs.GetQueryString()}";
            //return res;
            var authorizedUrl = HttpUtility.UrlEncode(client.Apis.Authentication.AuthorizeUrl(document.InlineUri));
            //return $"http://localhost:52470/Document/From?url={authorizedUrl}&fileName={document.FileName}";
            return $"https://sabiofilter.azurewebsites.net/Document/From?url={authorizedUrl}&fileName={document.FileName}";
        }

    }

    public class LoginArgs
    {
        public string User { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }

        public string GetQueryString(string firstDelimiter = "&")
        {
            return this.ToQueryString(firstDelimiter);
        }
    }
}