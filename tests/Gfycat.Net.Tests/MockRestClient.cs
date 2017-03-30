using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Gfycat.Rest;

namespace Gfycat.Net.Tests
{
    internal class MockRestClient : IRestClient
    {
        public MockRestClient()
        {
            CurrentSetHeaders = new Dictionary<string, string>();
        }

        internal Uri BaseUri { get; } = new Uri(GfycatClientConfig.BaseUrl);
        internal Dictionary<string, string> CurrentSetHeaders { get; set; }

        public Task<RestResponse> SendAsync(string method, string endpoint, CancellationToken token)
        {
            return Task.FromResult(Responses.GetResponse(method, endpoint));
        }

        public Task<RestResponse> SendAsync(string method, string endpoint, string json, CancellationToken token)
        {
            return Task.FromResult(Responses.GetResponse(method, endpoint, json));
        }

        public Task<RestResponse> SendAsync(string method, string endpoint, Stream stream, CancellationToken token)
        {
            return Task.FromResult(Responses.GetResponse(method, endpoint, stream));
        }

        public Task<RestResponse> SendAsync(string method, string endpoint, IDictionary<string, object> multipart, CancellationToken token)
        {
            return Task.FromResult(Responses.GetResponse(method, endpoint, multipart));
        }

        public void SetCancellationToken(CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public void SetHeader(string key, string value)
        {
            CurrentSetHeaders.Remove(key);
            if (value != null)
                CurrentSetHeaders.Add(key, value);
        }
    }
}
