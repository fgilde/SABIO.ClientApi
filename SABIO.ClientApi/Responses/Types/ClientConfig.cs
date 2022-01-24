using Newtonsoft.Json;
using SABIO.ClientApi.Extensions;

namespace SABIO.ClientApi.Responses.Types
{
    public class ClientConfigResult : BaseResult<ClientConfig>
    {}

    public class ClientConfig
    {
        public Authentication Authentication { get; set; }
        public bool WebSSO => !string.IsNullOrEmpty(Authentication?.LoginPageUrl);
        public string BuildNumber { get; set; }
        public string ClientPath { get; set; }
        public string Environment { get; set; }
        public bool FrontendMonitoringEnabled { get; set; }
        public string FrontendMonitoringHost { get; set; }
        public bool IsOverrideJsFilePath { get; set; }
        public string LoginLogoUrl { get; set; }
        public bool OnlineHelp { get; set; }
        public string OverrideJsFilePath { get; set; }
        public PathConfig Path { get; set; }
        public string Realm { get; set; }
        public string ThemeId { get; set; }
        public string ThemePackageName { get; set; }
        public UrlConfig Url { get; set; }
        public bool UseUrlLoginToken { get; set; }
    }

    public class OpenIdConnect
    {
        [JsonProperty("authorization_endpoint")]
        public string AuthorizationEndpoint { get; set; }
        [JsonProperty("token_endpoint")]
        public string TokenEndpoint { get; set; }
        [JsonProperty("end_session_endpoint")]
        public string EndSessionEndpoint { get; set; }

        public string GetLoginUrl(string redirectToUrl)
        {
            if (string.IsNullOrEmpty(AuthorizationEndpoint))
                return string.Empty;
            return AuthorizationEndpoint + new { client_id = "knowledge-sabio5", response_type = "token", redirect_uri = redirectToUrl }.ToQueryString("?");
        }
    }

    public class Authentication
    {
        public bool Cookie { get; set; }
        public string LoginPageUrl { get; set; }
        public string LogoutPageUrl { get; set; }
        public bool NativeApp { get; set; }
        public bool RecoverPassword { get; set; }
        public bool Saml { get; set; }
        public bool SamlAuthInIframe { get; set; }
        public bool SingleSignOn { get; set; }
        public bool Tracking { get; set; }
        public OpenIdConnect OpenIdConnect { get; set; }
    }



    public class PathConfig
    {
        public string Data { get; set; }
    }

    public class UrlConfig
    {
        public string Manual { get; set; }
    }
}