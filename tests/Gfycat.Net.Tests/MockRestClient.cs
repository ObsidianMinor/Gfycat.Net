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
        internal const string ValidUser = "validUser";
        internal const string InvalidUser = "invalidUser";

        internal readonly ServerState State;
        internal readonly Responses Responses;

        public MockRestClient()
        {
            State = new ServerState();
            Responses = new Responses();
            CurrentSetHeaders = new Dictionary<string, string>();
        }

        internal Uri BaseUri { get; } = new Uri(GfycatClientConfig.BaseUrl);
        internal Dictionary<string, string> CurrentSetHeaders { get; set; }

        public Task<RestResponse> SendAsync(string method, string endpoint, CancellationToken token)
        {
            State.ProcessState(method, endpoint);
            return Task.FromResult(Responses.GetResponse(method, endpoint));
        }

        public Task<RestResponse> SendAsync(string method, string endpoint, string json, CancellationToken token)
        {
            State.ProcessState(method, endpoint, json);
            return Task.FromResult(Responses.GetResponse(method, endpoint, json));
        }

        public Task<RestResponse> SendAsync(string method, string endpoint, Stream stream, CancellationToken token)
        {
            State.ProcessState(method, endpoint, stream);
            return Task.FromResult(Responses.GetResponse(method, endpoint, stream));
        }

        public Task<RestResponse> SendAsync(string method, string endpoint, IDictionary<string, object> multipart, CancellationToken token)
        {
            State.ProcessState(method, endpoint, multipart);
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
