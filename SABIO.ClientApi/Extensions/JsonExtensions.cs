using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using SABIO.ClientApi.Core;

namespace SABIO.ClientApi.Extensions
{
    public static class JsonExtensions
    {
        public static StringContent ToStringContent(this object o, string json = null) =>
            new(json ?? ToJson(o), Encoding.UTF8, "application/json");

        public static string ToJson(this object o)
            => o is string && IsValidJson(o.ToString()) ? o.ToString() : JsonConvert.SerializeObject(o, JsonSettings);

        private static bool IsValidJson(string s)
            => Check.TryCatch(() => JsonConvert.DeserializeObject(s) != null);


        private static JsonSerializerSettings JsonSettings =>
            new JsonSerializerSettings { ContractResolver = new SabioContractResolver(), NullValueHandling = NullValueHandling.Ignore };

    }
}