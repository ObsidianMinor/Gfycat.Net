using Gfycat.Rest;
using Newtonsoft.Json;
using System;

namespace Gfycat
{
    /// <summary>
    /// Defines a configuration for a <see cref="GfycatClient"/>
    /// </summary>
    public class GfycatClientConfig
    {
        /// <summary>
        /// Gets the Gfycat API version used by the client
        /// </summary>
        public const int ApiVersion = 1;
        /// <summary>
        /// Gets the base url for all requests made by the rest client
        /// </summary>
        public static readonly string BaseUrl = $"https://api.gfycat.com/v{ApiVersion}/";
        internal readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings() // for future things, if we need it
        {
            NullValueHandling = NullValueHandling.Ignore
        };
        /// <summary>
        /// Gets the client Id of this config
        /// </summary>
        public string ClientId { get; }
        /// <summary>
        /// Gets the client secret of this config
        /// </summary>
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
        /// <summary>
        /// Sets the default number of items to fetch in each feed
        /// </summary>
        public int DefaultFeedItemCount { get; set; } = 10;
        /// <summary>
        /// Constructs a <see cref="GfycatClientConfig"/> using the specified client Id and client secret
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        public GfycatClientConfig(string clientId, string clientSecret)
        {
            ClientId = clientId ?? throw new ArgumentNullException(nameof(clientId));
            ClientSecret = clientSecret ?? throw new ArgumentNullException(nameof(clientSecret));
        }
    }
}
