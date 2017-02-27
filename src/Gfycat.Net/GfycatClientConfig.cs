using Gfycat.Rest;
using System;

namespace Gfycat
{
    public class GfycatClientConfig
    {
        const string _baseUri = "https://api.gfycat.com/v1/";

        /// <summary>
        /// Overrides the rest client for this Gfycat client
        /// </summary>
        public IRestClient RestClient { get; set; }

        public RetryMode DefaultRetryMode { get; set; }

        public GfycatClientConfig()
        {
            RestClient = new DefaultRestClient(new Uri(_baseUri));
        }
    }
}
