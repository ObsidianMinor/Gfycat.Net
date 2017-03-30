using System;
using System.Collections.Generic;
using System.Text;

namespace Gfycat.Net.Tests
{
    internal static class Utils
    {
        const string clientId = "fakeClientId";
        const string clientSecret = "fakeClientSecret";

        internal static GfycatClient MakeClient()
        {
            GfycatClientConfig config = new GfycatClientConfig(clientId, clientSecret)
            {
                RestClient = new MockRestClient()
            };
            return new GfycatClient(config);
        }
    }
}
