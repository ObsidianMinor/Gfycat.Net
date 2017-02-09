using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Gfycat
{
    internal static class WebHelper
    {
        internal static async Task<T> SendJsonAsync<T>(this HttpClient client, string method, string endpoint, object json, string accessToken = null)
        {
            HttpRequestMessage message = CreateMessage(new HttpMethod(method), endpoint, accessToken);
            message.AddJsonContent(json);
            HttpResponseMessage result = await client.SendAsync(message);

            if (!result.IsSuccessStatusCode)
                throw await GetExceptionFromResponse(result);

            return await GetJsonFromResponse<T>(result);
        }

        internal static async Task SendJsonAsync(this HttpClient client, string method, string endpoint, object json, string accessToken = null)
        {
            HttpRequestMessage message = CreateMessage(new HttpMethod(method), endpoint, accessToken);
            message.AddJsonContent(json);
            HttpResponseMessage result = await client.SendAsync(message);

            if (!result.IsSuccessStatusCode)
                throw await GetExceptionFromResponse(result);
        }

        internal static async Task<HttpStatusCode> SendStreamAsync<T>(this HttpClient client, string method, string endpoint, Stream stream, string accessToken = null, bool throwIf401 = false)
        {
            HttpRequestMessage message = CreateMessage(new HttpMethod(method), endpoint, accessToken);
            message.AddStreamContent(stream);
            HttpResponseMessage result = await client.SendAsync(message);

            if (throwIf401 && result.StatusCode == HttpStatusCode.Unauthorized)
                throw await GetExceptionFromResponse(result);

            return result.StatusCode;
        }

        internal static async Task<T> SendRequestAsync<T>(this HttpClient client, string method, string endpoint, string accessToken = null)
        {
            HttpRequestMessage message = CreateMessage(new HttpMethod(method), endpoint, accessToken);
            HttpResponseMessage result = await client.SendAsync(message);

            if (!result.IsSuccessStatusCode)
                throw await GetExceptionFromResponse(result);

            return await GetJsonFromResponse<T>(result);
        }
        
        internal static async Task<HttpStatusCode> SendRequestForStatusAsync(this HttpClient client, string method, string endpoint, string accessToken = null, bool throwIf401 = false)
        {
            HttpRequestMessage message = CreateMessage(new HttpMethod(method), endpoint, accessToken);
            HttpResponseMessage result = await client.SendAsync(message);

            if (throwIf401 && result.StatusCode == HttpStatusCode.Unauthorized)
                throw await GetExceptionFromResponse(result);

            return result.StatusCode;
        }

        internal static async Task<HttpStatusCode> SendJsonForStatusAsync(this HttpClient client, string method, string endpoint, object json, string accessToken = null, bool throwIf401 = false)
        {
            HttpRequestMessage message = CreateMessage(new HttpMethod(method), endpoint, accessToken);
            AddJsonContent(message, json);
            HttpResponseMessage result = await client.SendAsync(message);

            if (throwIf401 && result.StatusCode == HttpStatusCode.Unauthorized)
                throw await GetExceptionFromResponse(result);

            return result.StatusCode;
        }

        private static async Task<T> GetJsonFromResponse<T>(HttpResponseMessage message)
        {
            string result = await message.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(result);
        }

        private static async Task<GfycatException> GetExceptionFromResponse(HttpResponseMessage message)
        {
            GfycatException exception = await GetJsonFromResponse<GfycatException>(message);
            exception.HttpCode = message.StatusCode;
            return exception;
        }

        private static void AddJsonContent(this HttpRequestMessage message, object json)
        {
            message.Content = new StringContent(JsonConvert.SerializeObject(json), System.Text.Encoding.UTF8, "application/json");
        }

        private static void AddStreamContent(this HttpRequestMessage message, Stream stream)
        {
            message.Content = new StreamContent(stream);
        }

        private static HttpRequestMessage CreateMessage(HttpMethod method, string endpoint, string accessToken)
        {
            HttpRequestMessage message = new HttpRequestMessage(method, endpoint);
            if (!string.IsNullOrWhiteSpace(accessToken))
                message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return message;
        }
    }
}
