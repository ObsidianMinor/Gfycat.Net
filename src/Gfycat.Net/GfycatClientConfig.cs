using Gfycat.Rest;
using Newtonsoft.Json;
using System;

namespace Gfycat
{
    public class GfycatClientConfig
    {
        public const int ApiVersion = 1;
        public static readonly string BaseUrl = $"https://api.gfycat.com/v{ApiVersion}/";
        internal readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings() // for future things, if we need it
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        public string ClientId { get; }
        public string ClientSecret { get; }

        /// <summary>
        /// Overrides the rest client for this Gfycat client
        /// </summary>
        public IRestClient RestClient { get; set; } = new DefaultRestClient(new Uri(BaseUrl));

        /// <summary>
        /// Sets the default retry mode for all requests, the default is <see cref="RetryMode.RetryFirst401"/>
        /// </summary>
        public RetryMode DefaultRetryMode { get; set; } = RetryMode.RetryFirst401;
        /// <summary>
        /// Sets the default timeout for all requests.
        /// </summary>
        public int DefaultTimeout { get; set; } = -1;

        public GfycatClientConfig(string clientId, string clientSecret)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
        }
    }
}
