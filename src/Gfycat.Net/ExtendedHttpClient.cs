using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Gfycat
{
    internal class ExtendedHttpClient : HttpClient
    {
        internal AuthenticationContainer Auth { get; set; }

        internal ExtendedHttpClient() : base() { }

        internal async Task CheckAuthorization(string endpoint)
        {
            if (await SendRequestForStatusAsync("HEAD", endpoint) == HttpStatusCode.Unauthorized)
            {
                await Auth.RefreshTokenAsync();
                if (await SendRequestForStatusAsync("HEAD", endpoint) == HttpStatusCode.Unauthorized)
                    throw new GfycatException()
                    {
                        HttpCode = (HttpStatusCode)401,
                        Code = "Unauthorized",
                        Description = "A valid access token is required to access this resource"
                    };
            }
        }

        internal async Task<T> SendJsonAsync<T>(string method, string endpoint, object json, bool useAccessToken = true)
        {
            Debug.WriteLine($"Sending json to {endpoint}");
            HttpRequestMessage message = await CreateMessageAsync(new HttpMethod(method), endpoint, useAccessToken);
            AddJsonContent(message, json);
            HttpResponseMessage result = await SendAsync(message);

            if (!result.IsSuccessStatusCode)
                throw await GetExceptionFromResponse(result);

            return await GetJsonFromResponse<T>(result);
        }

        internal async Task SendJsonAsync(string method, string endpoint, object json, bool useAccessToken = true)
        {
            Debug.WriteLine($"Sending json to {endpoint}");
            HttpRequestMessage message = await CreateMessageAsync(new HttpMethod(method), endpoint, useAccessToken);
            AddJsonContent(message, json);
            HttpResponseMessage result = await SendAsync(message);

            if (!result.IsSuccessStatusCode)
                throw await GetExceptionFromResponse(result);
        }

        internal async Task<HttpStatusCode> SendStreamAsync(string method, string endpoint, Stream stream, string fileName, bool useAccessToken = true, bool throwIf401 = false, CancellationToken? cancelToken = null)
        {
            Debug.WriteLine($"Sending stream to {endpoint}");
            HttpRequestMessage message = await CreateMessageAsync(new HttpMethod(method), endpoint, useAccessToken);
            AddStreamContent(message, stream, fileName);
             
            HttpResponseMessage result = (cancelToken.HasValue) ? await SendAsync(message) : await SendAsync(message, cancelToken.Value);

            if (throwIf401 && result.StatusCode == HttpStatusCode.Unauthorized)
                throw await GetExceptionFromResponse(result);

            return result.StatusCode;
        }

        internal async Task<T> SendRequestAsync<T>(string method, string endpoint, bool useAccessToken = true)
        {
            Debug.WriteLine($"Sending request to {endpoint}");
            HttpRequestMessage message = await CreateMessageAsync(new HttpMethod(method), endpoint, useAccessToken);
            HttpResponseMessage result = await SendAsync(message);

            if (!result.IsSuccessStatusCode)
                throw await GetExceptionFromResponse(result);

            return await GetJsonFromResponse<T>(result);
        }
        
        internal async Task<HttpStatusCode> SendRequestForStatusAsync(string method, string endpoint, bool useAccessToken = true, bool throwIf401 = false)
        {
            Debug.WriteLine($"Sending request for status code to {endpoint}");
            HttpRequestMessage message = await CreateMessageAsync(new HttpMethod(method), endpoint, useAccessToken);
            HttpResponseMessage result = await SendAsync(message);

            if (throwIf401 && result.StatusCode == HttpStatusCode.Unauthorized)
                throw await GetExceptionFromResponse(result);

            return result.StatusCode;
        }

        internal async Task<HttpStatusCode> SendJsonForStatusAsync(string method, string endpoint, object json, bool useAccessToken = true, bool throwIf401 = false)
        {
            Debug.WriteLine($"Sending json for status code to {endpoint}");
            HttpRequestMessage message = await CreateMessageAsync(new HttpMethod(method), endpoint, useAccessToken);
            AddJsonContent(message, json);
            HttpResponseMessage result = await SendAsync(message);

            if (throwIf401 && result.StatusCode == HttpStatusCode.Unauthorized)
                throw await GetExceptionFromResponse(result);

            return result.StatusCode;
        }

        private static async Task<T> GetJsonFromResponse<T>(HttpResponseMessage message)
        {
            string result = await message.Content.ReadAsStringAsync();
            #if SUPER_DEBUG_MODE
            Debug.WriteLine(result);
            #endif
            return JsonConvert.DeserializeObject<T>(result);
        }

        private static async Task<GfycatException> GetExceptionFromResponse(HttpResponseMessage message)
        {
            GfycatException exception = await GetJsonFromResponse<GfycatException>(message);
            exception.HttpCode = message.StatusCode;
            return exception;
        }

        private static void AddJsonContent(HttpRequestMessage message, object json)
        {
            message.Content = new StringContent(JsonConvert.SerializeObject(json), System.Text.Encoding.UTF8, "application/json");
        }

        private static void AddStreamContent(HttpRequestMessage message, Stream stream, string fileName)
        {
            message.Content = new StreamContent(stream);
            message.Content.Headers.ContentDisposition.FileName = fileName;
        }

        private async Task<HttpRequestMessage> CreateMessageAsync(HttpMethod method, string endpoint, bool useAcccessToken = true)
        {
            if (useAcccessToken)
                await CheckAuthorization(endpoint);
            HttpRequestMessage message = new HttpRequestMessage(method, endpoint);
            if (useAcccessToken)
                message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Auth.AccessToken);
            return message;
        }
    }
}
