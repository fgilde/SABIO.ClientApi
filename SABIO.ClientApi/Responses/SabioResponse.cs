using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly Lazy<List<string>> _missingMemberNames;
        public bool Success { get; set; }
        public Status Status { get; set; }
        public SabioEvent[] Events { get; set; }

        [JsonIgnore]
        public string Json { get; set; }
        [JsonIgnore]
        public List<ErrorEventArgs> DeserializationWarnings { get; set; }

        public List<string> MissingMemberNames => _missingMemberNames.Value;


        public SabioResponse()
        {
            _missingMemberNames = new Lazy<List<string>>(() => (DeserializationWarnings ?? new List<ErrorEventArgs>()).Select(s => s.ErrorContext.Member.ToString()).ToList());
        }
    }
}