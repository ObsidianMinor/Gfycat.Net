using System;
using System.Collections.Generic;
using System.Text;

namespace Gfycat.Net.Tests
{
    internal static class Utils
    {
        const string clientId = "fakeClientId";
        const string clientSecret = "fakeClientSecret";

        internal static (GfycatClient, MockRestClient) MakeClient()
        {
            MockRestClient restClient = new MockRestClient();
            GfycatClientConfig config = new GfycatClientConfig(clientId, clientSecret)
            {
                RestClient = restClient
            };
            return (new GfycatClient(config), restClient);
        }
    }
}
