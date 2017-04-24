using System;
using Gfycat.Net.Tests.RestFakes;

namespace Gfycat.Net.Tests
{
    internal static class Utils
    {
        const string clientId = "fakeClientId";
        const string clientSecret = "fakeClientSecret";

        internal static GfycatClient MakeClient()
        {
            MockRestClient restClient = new MockRestClient(new Uri(GfycatClientConfig.BaseUrl));
            GfycatClientConfig config = new GfycatClientConfig(clientId, clientSecret)
            {
                RestClient = restClient
            };
            return new GfycatClient(config);
        }
    }
}
