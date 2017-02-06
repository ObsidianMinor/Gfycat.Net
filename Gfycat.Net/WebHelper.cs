using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Gfycat
{
    internal class WebHelper : HttpMessageHandler
    {
        HttpClient _client;

        internal WebHelper()
        {
            _client = new HttpClient(this, false);
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        internal T SendRequestAsync<T>()
        {

        }
    }
}
