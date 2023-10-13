using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SABIO.ClientApi.Responses;

namespace SABIO.ClientApi.Extensions
{
    public static class ResponseExtensions
    {
        public static async Task<TContent> ReadAsAsync<TContent>(this HttpResponseMessage message) where TContent : class
        {
            var responseJson = await message.Content.ReadAsStringAsync();
            if (typeof(TContent) == typeof(string))
                return responseJson as TContent;
            var deserializationWarnings = new List<ErrorEventArgs>();
            var result = JsonConvert.DeserializeObject<TContent>(responseJson, new JsonSerializerSettings
            {
                Error = (sender, args) =>
                {
                    deserializationWarnings.Add(args);
                    args.ErrorContext.Handled = true;
                },
                MissingMemberHandling = MissingMemberHandling.Error
            });
            if (result is SabioResponse response)
            {
                response.Json = responseJson;
                response.DeserializationWarnings = deserializationWarnings;
            }
            return result;
        }

        public static async Task<TContent> ReadAsAsync<TContent>(this Task<HttpResponseMessage> message) where TContent : class
        {
            var response = await message;
            return await response.ReadAsAsync<TContent>();
        }
    }
}