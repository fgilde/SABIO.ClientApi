using SABIO.ClientApi.Core.Api;

namespace SABIO.ClientApi.Core
{
    public class Apis
    {
        private readonly SabioClient _client;

        public Apis(SabioClient client)
        {
            _client = client;
        }

        //TODO: Generate with additional files code gen
        public AuthenticationApi Authentication => _client.Api<AuthenticationApi>();
        public TextsApi Texts => _client.Api<TextsApi>();
        public DocumentsApi Documents => _client.Api<DocumentsApi>();
        public ConfigApi Config => _client.Api<ConfigApi>();
        public ResourceApi ResourceApi => _client.Api<ResourceApi>();
        public TreeApi Tree => _client.Api<TreeApi>();        
        public PinboardsApi Pinboards => _client.Api<PinboardsApi>();        
    }
}