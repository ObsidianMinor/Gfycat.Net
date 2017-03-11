using Gfycat.Rest;
using Newtonsoft.Json;
using System;
using System.Reflection;

namespace Gfycat
{
    public class GfycatClientConfig
    {
        public const int ApiVersion = 1;
        public static readonly string BaseUrl = $"https://api.gfycat.com/v{ApiVersion}/";
        public static Version Version => typeof(GfycatClient).GetTypeInfo().Assembly.GetName().Version;
        internal JsonSerializerSettings SerializerSettings = new JsonSerializerSettings()
        {
        };

        /// <summary>
        /// Overrides the rest client for this Gfycat client
        /// </summary>
        public IRestClient RestClient { get; set; } = new DefaultRestClient(new Uri(BaseUrl));
        public OAuth2.Provider AuthenticatorProvider { get; set; } = (clientId, clientSecret) => new OAuth2.DefaultAuthenticator(clientId, clientSecret, );

        /// <summary>
        /// Sets the default retry mode for all requests, the default is <see cref="RetryMode.RetryFirst401"/>
        /// </summary>
        public RetryMode DefaultRetryMode { get; set; } = RetryMode.RetryFirst401;
        /// <summary>
        /// Sets the default timeout for all requests.
        /// </summary>
        public int? DefaultTimeout { get; set; } = 30000;
    }
}
