using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Gfycat
{
    internal class WebWrapper : IDisposable
    {
        HttpClient _client;
        const string _startEndpoint = "https://api.gfycat.com/v1/";
        bool _recievedUnauthorized;

        internal WebWrapper()
        {
            _client = new HttpClient() { BaseAddress = new Uri(_startEndpoint) };
        }

        internal async Task<T> SendJsonAsync<T>(string method, string endpoint, object json, string accessToken = null)
        {
            HttpRequestMessage message = CreateMessage(new HttpMethod(method), endpoint, accessToken);
            AddJsonContent(message, json);
            HttpResponseMessage result = await _client.SendAsync(message);

            if (!result.IsSuccessStatusCode)
                throw await GetExceptionFromResponse(result);

            return await GetJsonFromResponse<T>(result);
        }

        internal async Task SendJsonAsync(string method, string endpoint, object json, string accessToken = null)
        {
            HttpRequestMessage message = CreateMessage(new HttpMethod(method), endpoint, accessToken);
            AddJsonContent(message, json);
            HttpResponseMessage result = await _client.SendAsync(message);

            if (!result.IsSuccessStatusCode)
                throw await GetExceptionFromResponse(result);
        }

        internal async Task<T> SendRequestAsync<T>(string method, string endpoint, string accessToken = null)
        {
            HttpRequestMessage message = CreateMessage(new HttpMethod(method), endpoint, accessToken);
            HttpResponseMessage result = await _client.SendAsync(message);

            if (!result.IsSuccessStatusCode)
                throw await GetExceptionFromResponse(result);

            return await GetJsonFromResponse<T>(result);
        }

        internal async Task<HttpStatusCode> SendRequestForStatusAsync(string method, string endpoint, string accessToken = null, bool throwIf401 = false)
        {
            HttpRequestMessage message = CreateMessage(new HttpMethod(method), endpoint, accessToken);
            HttpResponseMessage result = await _client.SendAsync(message);

            if (throwIf401 && result.StatusCode == HttpStatusCode.Unauthorized)
                throw await GetExceptionFromResponse(result);

            return result.StatusCode;
        }

        internal async Task<HttpStatusCode> SendJsonForStatusAsync(string method, string endpoint, object json, string accessToken = null, bool throwIf401 = false)
        {
            HttpRequestMessage message = CreateMessage(new HttpMethod(method), endpoint, accessToken);
            AddJsonContent(message, json);
            HttpResponseMessage result = await _client.SendAsync(message);

            if (throwIf401 && result.StatusCode == HttpStatusCode.Unauthorized)
                throw await GetExceptionFromResponse(result);

            return result.StatusCode;
        }

        private async Task<T> GetJsonFromResponse<T>(HttpResponseMessage message)
        {
            string result = await message.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(result);
        }

        private Task<GfycatException> GetExceptionFromResponse(HttpResponseMessage message) => GetJsonFromResponse<GfycatException>(message);

        private void AddJsonContent(HttpRequestMessage message, object json)
        {
            message.Content = new StringContent(JsonConvert.SerializeObject(json), System.Text.Encoding.UTF8, "application/json");
        }

        private HttpRequestMessage CreateMessage(HttpMethod method, string endpoint, string accessToken)
        {
            HttpRequestMessage message = new HttpRequestMessage(method, _startEndpoint + endpoint);
            if (!string.IsNullOrWhiteSpace(accessToken))
                message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return message;
        }

        public void Dispose()
        {
            ((IDisposable)_client).Dispose();
        }
    }
}
