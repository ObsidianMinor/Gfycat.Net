using Gfycat.Rest;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Gfycat
{
    internal static class RestClientExtensions
    {
        internal static async Task<T> SendJsonAsync<T>(this IRestClient client, string method, string endpoint, object json, RequestOptions options = null)
        {
            return (await SendJsonAsync(client, method, endpoint, json, options)).ReadAsJson<T>();
        }

        internal static Task<RestResponse> SendJsonAsync(this IRestClient client, string method, string endpoint, object json, RequestOptions options = null)
        {
            string jsonString = JsonConvert.SerializeObject(json);
            return client.SendAsync(method, endpoint, jsonString, options.CancellationToken);
        }
    }
}
