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

        internal async Task<T> SendJsonAsync<T>(HttpMethod method, string endpoint, object json, string accessToken = null)
        {
            HttpRequestMessage message = CreateMessage(method, endpoint, accessToken);
            AddJsonContent(message, json);
            HttpResponseMessage result = await _client.SendAsync(message);
            return await GetJsonFromResponse<T>(result);
        }

        internal async Task<string> SendJsonAsync(HttpMethod method, string endpoint, object json, string accessToken = null)
        {
            HttpRequestMessage message = CreateMessage(method, endpoint, accessToken);
            AddJsonContent(message, json);
            HttpResponseMessage result = await _client.SendAsync(message);
            return await result.Content.ReadAsStringAsync();
        }

        internal async Task<T> SendRequestAsync<T>(HttpMethod method, string endpoint, string accessToken = null)
        {
            HttpRequestMessage message = CreateMessage(method, endpoint, accessToken);
            HttpResponseMessage result = await _client.SendAsync(message);
            return await GetJsonFromResponse<T>(result);
        }

        internal async Task<HttpStatusCode> SendRequestForStatusAsync(HttpMethod method, string endpoint, string accessToken = null)
        {
            throw new NotImplementedException();
        }

        internal async Task<HttpStatusCode> SendJsonForStatusAsync(HttpMethod method, string endpoint, object json, string accessToken = null)
        {
            throw new NotImplementedException();
        }

        private async Task<T> GetJsonFromResponse<T>(HttpResponseMessage message)
        {
            string result = await message.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(result);
        }

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
