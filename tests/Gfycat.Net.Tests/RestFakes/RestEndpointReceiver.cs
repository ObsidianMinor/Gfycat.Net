using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Gfycat.Net.Tests.RestFakes
{
    public delegate Task<HttpResponseMessage> RestEndpointReceiver(HttpRequestMessage request);
}
