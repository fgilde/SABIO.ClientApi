using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SABIO.ClientApi.Responses.Types;

namespace SABIO.ClientApi.Responses
{
    public class SabioResponse<TResponseContent> : SabioResponse
    {
        public TResponseContent Data { get; set; }
    }

    public class SabioResponse
    {
        public bool Success { get; set; }
        public Status Status { get; set; }
        public SabioEvent[] Events { get; set; }

        [JsonIgnore]
        public string Json { get; set; }
        [JsonIgnore]
        public List<ErrorEventArgs> DeserializationWarnings { get; set; }
    }
}