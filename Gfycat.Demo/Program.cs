using Gfycat;
using System.Threading.Tasks;

class Program
{
    static void Main(string[] args) => new Program().Start().GetAwaiter().GetResult();

    public async Task Start()
    {
        string clientId = "CLIENT_ID";
        string clientSecret = "CLIENT_SECRET";

        GfycatClient _client = new GfycatClient(clientId, clientSecret); // create the client with our client ID and client secret

        await _client.AuthenticateAsync(); // authenticate using the client ID and secret

        
    }
}