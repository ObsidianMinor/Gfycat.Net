using Gfycat.Rest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Gfycat.Net.Tests.RestFakes
{
    internal partial class MockRestClient : IRestClient
    {
        internal class MockMethod
        {
            internal RestEndpointAttribute EndpointAttribute { get; set; }

            internal RestEndpointReceiver Method { get; set; }
        }
        
        Uri _baseUri;
        IEnumerable<MockMethod> _methods;
        Dictionary<string, string> _headers = new Dictionary<string, string>();

        internal MockRestClient(Uri baseUri)
        {
            _baseUri = baseUri;
            _methods = GetType().GetTypeInfo().DeclaredMethods.Select(m => new MockMethod()
            {
                Method = CreateMethod(m),
                EndpointAttribute = m.GetCustomAttribute<RestEndpointAttribute>()
            })
            .Where(m => m.EndpointAttribute != null && m.Method != null);
        }

        private RestEndpointReceiver CreateMethod(MethodInfo method)
        {
            try
            {
                return method.CreateDelegate(typeof(RestEndpointReceiver), this) as RestEndpointReceiver;
            }
            catch(ArgumentException)
            {
                return null;
            }
        }

        public async Task<RestResponse> SendAsync(string method, string endpoint, CancellationToken token)
        {
            using (HttpRequestMessage request = new HttpRequestMessage())
                return await SendInternalAsync(method, endpoint, request, token).ConfigureAwait(false);
        }

        public async Task<RestResponse> SendAsync(string method, string endpoint, string json, CancellationToken token)
        {
            using (HttpRequestMessage request = new HttpRequestMessage())
            using (StringContent content = new StringContent(json))
            {
                request.Content = content;
                return await SendInternalAsync(method, endpoint, request, token).ConfigureAwait(false);
            }
        }

        public async Task<RestResponse> SendAsync(string method, string endpoint, Stream stream, CancellationToken token)
        {
            using (HttpRequestMessage request = new HttpRequestMessage())
            using (StreamContent content = new StreamContent(stream))
            {
                request.Content = content;
                return await SendInternalAsync(method, endpoint, request, token).ConfigureAwait(false);
            }
        }

        public async Task<RestResponse> SendAsync(string method, string endpoint, IDictionary<string, object> multipart, CancellationToken token)
        {
            using (HttpRequestMessage request = new HttpRequestMessage())
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
                return await SendInternalAsync(method, endpoint, request, token).ConfigureAwait(false);
            }
        }

        private async Task<RestResponse> SendInternalAsync(string method, string endpoint, HttpRequestMessage message, CancellationToken token)
        {
            foreach (var header in _headers)
                message.Headers.Add(header.Key, header.Value);

            message.Method = new HttpMethod(method);
            message.RequestUri = new Uri(_baseUri, endpoint);

            MockMethod mock = _methods.FirstOrDefault(m => m.EndpointAttribute.Endpoint == endpoint && m.EndpointAttribute.Method == method) ?? throw new InvalidOperationException();
            HttpResponseMessage response = await mock.Method(message);
            return new RestResponse(response.StatusCode, response.Headers.ToDictionary(h => h.Key, h => h.Value.FirstOrDefault()), response.Content, message.Method, message.RequestUri);
        }

        public void SetHeader(string key, string value)
        {
            _headers.Remove(key);
            if (value != null)
                _headers.Add(key, value);
        }
    }
}
