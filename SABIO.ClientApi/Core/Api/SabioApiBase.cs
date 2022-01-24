
namespace SABIO.ClientApi.Core.Api
{
    public abstract class SabioApiBase 
    {
        private SabioClient _client;

        public SabioClient Client
        {
            get => _client;
            internal set
            {
                if (Equals(value, _client)) return;
                _client = value;
                ClientChanged();
            }
        }

        protected virtual void ClientChanged()
        {

        }

    }
}