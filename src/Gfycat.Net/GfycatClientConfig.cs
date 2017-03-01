using Gfycat.Rest;
using System;

namespace Gfycat
{
    public class GfycatClientConfig
    {
        public const int ApiVersion = 1;
        public static readonly string BaseUrl = $"https://api.gfycat.com/v{ApiVersion}/";

        /// <summary>
        /// Overrides the rest client for this Gfycat client
        /// </summary>
        public IRestClient RestClient { get; set; }

        public RetryMode DefaultRetryMode { get; set; }
        public int? DefaultTimeout { get; set; }

        public GfycatClientConfig()
        {
            RestClient = new DefaultRestClient(new Uri(BaseUrl));
        }
    }
}
