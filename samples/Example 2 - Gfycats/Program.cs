using Gfycat;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GfycatsSample
{
    class Program
    {
        const string _clientId = "REPLACE_WITH_YOUR_CLIENT_ID";
        const string _clientSecret = "REPLACE_WITH_YOUR_CLIENT_SECRET";
        GfycatClient _client;

        static void Main(string[] args) => new Program().StartAsync().GetAwaiter().GetResult();

        async Task StartAsync()
        {
            _client = new GfycatClient(_clientId, _clientSecret);
            await _client.Authentication.AuthenticateClientAsync(); // gfy actions such as creating and fetching don't require that a user logs in

            string gfyId = await _client.CreateGfyAsync(File.Open("somefile", FileMode.Open));

            GfyStatus status = await _client.GetGfyUploadStatusAsync(gfyId);
            while(status.Task == Status.Encoding)
            {
                await Task.Delay(TimeSpan.FromSeconds(status.Time));
                status = await _client.GetGfyUploadStatusAsync(gfyId);
            }
        }

        async Task<string> FetchGfySouceAsync(string gfyName)
        {
            Gfy gfy = await _client.GetGfyAsync(gfyName); // provided you have a gfy's ID, you can fetch it with GetGfyAsync
            return gfy.Source; // if this gfy was made by a youtube video or another image link, this field will not be null
        }
    }
}