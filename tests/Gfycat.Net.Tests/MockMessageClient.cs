using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Gfycat.Rest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RichardSzalay.MockHttp;

namespace Gfycat.Tests
{
    public class MockMessageClient : IRestClient
    {
        private readonly HttpClient _client;
        private readonly Uri _baseUri = new Uri("https://api.gfycat.com/v1/");

        public MockMessageClient(MockHttpMessageHandler handler)
        {
            _client = handler.ToHttpClient();
        }

        public async Task<RestResponse> SendAsync(string method, string endpoint, CancellationToken token)
        {
            using (HttpRequestMessage request = new HttpRequestMessage(new HttpMethod(method), new Uri(_baseUri, endpoint)))
            {
                return await SendInternalAsync(request, token).ConfigureAwait(false);
            }
        }

        public async Task<RestResponse> SendAsync(string method, string endpoint, string json, CancellationToken token)
        {
            using (HttpRequestMessage request = new HttpRequestMessage(new HttpMethod(method), new Uri(_baseUri, endpoint)))
            using (StringContent content = new StringContent(json, System.Text.Encoding.UTF8, "application/json"))
            {
                request.Content = content;
                return await SendInternalAsync(request, token).ConfigureAwait(false);
            }
        }

        public async Task<RestResponse> SendAsync(string method, string endpoint, Stream stream, CancellationToken token)
        {
            using (HttpRequestMessage request = new HttpRequestMessage(new HttpMethod(method), new Uri(_baseUri, endpoint)))
            using (StreamContent content = new StreamContent(stream))
            {
                request.Content = content;
                return await SendInternalAsync(request, token).ConfigureAwait(false);
            }
        }

        public async Task<RestResponse> SendAsync(string method, string endpoint, IDictionary<string, object> multipart, CancellationToken token)
        {
            using (HttpRequestMessage request = new HttpRequestMessage(new HttpMethod(method), new Uri(_baseUri, endpoint)))
            using (MultipartFormDataContent content = new MultipartFormDataContent())
            {
                foreach (KeyValuePair<string, object> param in multipart)
                {
                    switch (param.Value)
                    {
                        case string stringParam:
                            content.Add(new StringContent(stringParam), param.Key);
                            break;
                        case byte[] byteArrayParam:
                            content.Add(new ByteArrayContent(byteArrayParam), param.Key);
                            break;
                        case Stream streamParam:
                            content.Add(new StreamContent(streamParam), param.Key);
                            break;
                        default:
                            throw new InvalidOperationException($"Invalid parameter type: {param.Value.GetType().Name}");
                    }
                }
                request.Content = content;
                return await SendInternalAsync(request, token).ConfigureAwait(false);
            }
        }

        private async Task<RestResponse> SendInternalAsync(HttpRequestMessage message, CancellationToken token)
        {
            HttpResponseMessage response = await _client.SendAsync(message, token).ConfigureAwait(false);

            Dictionary<string, string> headers = response.Headers.ToDictionary(k => k.Key, k => k.Value.FirstOrDefault());

            return new RestResponse(response.StatusCode, headers, response.Content, message.Method, message.RequestUri);
        }

        public void SetHeader(string key, string value)
        {
            _client.DefaultRequestHeaders.Remove(key);
            if (value != null)
                _client.DefaultRequestHeaders.Add(key, value);
        }
    }

    public static class MockHttpMessageHandlerExtensions
    {
        public static MockedRequest WithJsonContent(this MockedRequest request, object content)
        {
            return MockedRequestExtensions.With(request, m => JToken.DeepEquals(JToken.Parse(m.Content.ReadAsStringAsync().Result), JObject.FromObject(content)));
        }

        public static MockedRequest Respond(this MockedRequest request, object content)
        {
            return request.Respond("application/json", JObject.FromObject(content).ToString(Formatting.None));
        }
    }
}
