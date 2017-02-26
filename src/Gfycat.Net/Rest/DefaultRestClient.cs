using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Gfycat.Rest
{
    public class DefaultRestClient : IRestClient, IDisposable
    {
        private readonly HttpClient _client;
        private readonly Uri _baseUri;

        public void Dispose()
        {
            _client.Dispose();
        }

        public Task<RestResponse> SendAsync(string method, string endpoint, CancellationToken token)
        {
            
        }

        public Task<RestResponse> SendAsync(string method, string endpoint, string json, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<RestResponse> SendAsync(string method, string endpoint, IDictionary<string, object> multipart, CancellationToken token)
        {
            throw new NotImplementedException();
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
