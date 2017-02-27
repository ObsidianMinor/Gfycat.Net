using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Gfycat.Rest
{
    internal sealed class DefaultRestClient : IRestClient, IDisposable
    {
        private readonly HttpClient _client;
        private readonly Uri _baseUri;
        private readonly CancellationTokenSource _tokenSource;
        private readonly CancellationToken _cancelToken, _parentToken;

        internal DefaultRestClient(Uri baseUri)
        {
            _baseUri = baseUri;
            _client = new HttpClient(new HttpClientHandler
            {
                UseProxy = false,
            },
            true);
            _tokenSource = new CancellationTokenSource();
            _cancelToken = CancellationToken.None;
            _parentToken = CancellationToken.None;
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        public Task<RestResponse> SendAsync(string method, string endpoint, CancellationToken token)
        {
            
        }

        public Task<RestResponse> SendAsync(string method, string endpoint, string json, CancellationToken token)
        {
            
        }

        public async Task<RestResponse> SendAsync(string method, string endpoint, IDictionary<string, object> multipart, CancellationToken token)
        {
            using (HttpRequestMessage request = new HttpRequestMessage(new HttpMethod(method), _baseUri + endpoint))
            {
                MultipartFormDataContent content = new MultipartFormDataContent("Upload----" + DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
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

        private Task<RestResponse> SendInternalAsync(HttpRequestMessage message, CancellationToken token)
        {

        }

        public void SetCancellationToken(CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public void SetHeader(string key, string value)
        {
            _client.DefaultRequestHeaders.Remove(key);
            if (value != null)
                _client.DefaultRequestHeaders.Add(key, value);
        }
    }
}
