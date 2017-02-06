using System.Threading.Tasks;

namespace Gfycat
{
    public class GfycatClient
    {
        long _clientId;
        string _clientSecret;

        public GfycatClient(long clientId, string clientSecret)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        public async Task AuthenticateAsync()
        {

        }

        public async Task AuthenticateAsync(string username, string password)
        {

        }
    }
}
