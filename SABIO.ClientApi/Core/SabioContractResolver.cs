using Newtonsoft.Json.Serialization;

namespace SABIO.ClientApi.Core
{
    public class SabioContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
            => string.IsNullOrEmpty(propertyName) ? propertyName : propertyName[0].ToString().ToLower() + propertyName.Substring(1);
    }
}