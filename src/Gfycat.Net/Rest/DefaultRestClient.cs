using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Gfycat.Rest
{
    internal sealed class DefaultRestClient : IRestClient, IDisposable
    {
        private readonly HttpClient _client;
        private readonly Uri _baseUri;
        private CancellationTokenSource _tokenSource;
        private CancellationToken _cancelToken, _parentToken;

        internal DefaultRestClient(Uri baseUri)
        {
            _baseUri = baseUri;
            _client = new HttpClient(new HttpClientHandler
            {
                UseProxy = false,
            });
            _tokenSource = new CancellationTokenSource();
            _cancelToken = CancellationToken.None;
            _parentToken = CancellationToken.None;
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        public async Task<RestResponse> SendAsync(string method, string endpoint, CancellationToken token)
        {
            using (HttpRequestMessage request = new HttpRequestMessage(new HttpMethod(method), new Uri(_baseUri, endpoint)))
            {
                return await SendInternalAsync(request, token);
            }
        }

        public async Task<RestResponse> SendAsync(string method, string endpoint, string json, CancellationToken token)
        {
            using (HttpRequestMessage request = new HttpRequestMessage(new HttpMethod(method), new Uri(_baseUri, endpoint)))
            {
                request.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                return await SendInternalAsync(request, token);
            }
        }

        public async Task<RestResponse> SendAsync(string method, string endpoint, Stream stream, CancellationToken token)
        {
            using (HttpRequestMessage request = new HttpRequestMessage(new HttpMethod(method), new Uri(_baseUri, endpoint)))
            {
                request.Content = new StreamContent(stream);
                return await SendInternalAsync(request, token);
            }
        }

        public async Task<RestResponse> SendAsync(string method, string endpoint, IDictionary<string, object> multipart, CancellationToken token)
        {
            using (HttpRequestMessage request = new HttpRequestMessage(new HttpMethod(method), new Uri(_baseUri, endpoint)))
            {
                MultipartFormDataContent content = new MultipartFormDataContent();
                foreach(KeyValuePair<string, object> param in multipart)
                {
                    switch(param.Value)
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
                        case MultipartFile multipartFileParam:
                            content.Add(new StreamContent(multipartFileParam.Stream), param.Key, multipartFileParam.FileName);
                            break;
                        default:
                            throw new InvalidOperationException($"Invalid parameter type: {param.Value.GetType().Name}");
                    }
                }
                request.Content = content;
                return await SendInternalAsync(request, token);
            }
        }

        private async Task<RestResponse> SendInternalAsync(HttpRequestMessage message, CancellationToken token)
        {
            while (true)
            {
                Debug.WriteLine($"{message.Method} {message.RequestUri}");
                token = CancellationTokenSource.CreateLinkedTokenSource(_parentToken, token).Token;
                HttpResponseMessage response = await _client.SendAsync(message, token).ConfigureAwait(false);

                Dictionary<string, string> headers = response.Headers.ToDictionary(k => k.Key, k => k.Value.FirstOrDefault());
                Stream contentStream = (message.Method != HttpMethod.Head) ? await response.Content.ReadAsStreamAsync().ConfigureAwait(false) : null;

                return new RestResponse(response.StatusCode, headers, contentStream, message.Method, message.RequestUri);
            }
        }

        public void SetCancellationToken(CancellationToken token)
        {
            _parentToken = token;
            _cancelToken = CancellationTokenSource.CreateLinkedTokenSource(_parentToken, _tokenSource.Token).Token;
        }

        public void SetHeader(string key, string value)
        {
            _client.DefaultRequestHeaders.Remove(key);
            if (value != null)
                _client.DefaultRequestHeaders.Add(key, value);
        }
    }
}
