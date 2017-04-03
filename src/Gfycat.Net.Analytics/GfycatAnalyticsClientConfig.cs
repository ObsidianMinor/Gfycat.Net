using System;
using Gfycat.Rest;
using System.Linq;

namespace Gfycat.Analytics
{
    /// <summary>
    /// Represents a configuration for a <see cref="GfycatAnalyticsClient"/>
    /// </summary>
    public class GfycatAnalyticsClientConfig
    {
        /// <summary>
        /// Gets the base impressions url
        /// </summary>
        public const string BaseImpressionsUrl = "https://px.gfycat.com/px.gif";
        /// <summary>
        /// Gets the base analytics url
        /// </summary>
        public const string BaseAnalyticsUrl = "https://metrics.gfycat.com";

        /// <summary>
        /// Gets the app Id for this config
        /// </summary>
        public string AppId { get; }
        /// <summary>
        /// Gets the app name for this config
        /// </summary>
        public string AppName { get; }
        /// <summary>
        /// Gets the app version for this config
        /// </summary>
        public Version AppVersion { get; }
        /// <summary>
        /// Gets the session Id for this config
        /// </summary>
        public string SessionId { get; }
        
        /// <summary>
        /// Overrides the rest client for this Gfycat client
        /// </summary>
        public IRestClient RestClient { get; set; } = new DefaultRestClient(new Uri("https://gfycat.com/"));

        /// <summary>
        /// Sets the default retry mode for all requests, the default is <see cref="RetryMode.Retry502"/>
        /// </summary>
        public RetryMode DefaultRetryMode { get; set; } = RetryMode.Retry502;

        /// <summary>
        /// Sets the default timeout for all requests.
        /// </summary>
        public int DefaultTimeout { get; set; } = -1;

        /// <summary>
        /// Creates a new config using the specified app name, id, and version
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="appId"></param>
        /// <param name="appVersion"></param>
        public GfycatAnalyticsClientConfig(string appName, string appId, Version appVersion) : this(appName, appId, appVersion, GenerateCookie()) { }

        /// <summary>
        /// Creates a new config using the specified app name, id, version, and session Id
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="appId"></param>
        /// <param name="appVersion"></param>
        /// <param name="sessionId"></param>
        public GfycatAnalyticsClientConfig(string appName, string appId, Version appVersion, string sessionId)
        {
            AppId = appId;
            AppName = appName;
            AppVersion = appVersion;
            SessionId = sessionId;
        }

        /// <summary>
        /// Generates a random tracking cookie
        /// </summary>
        /// <returns></returns>
        public static string GenerateCookie()
        {
            Random random = new Random();
            string GenerateString(int length)
            {
                const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                return new string(Enumerable.Repeat(chars, length)
                  .Select(s => s[random.Next(s.Length)]).ToArray());
            }

            return $"{GenerateString(8)}-{GenerateString(4)}-{GenerateString(4)}-{GenerateString(4)}-{GenerateString(12)}";
        }
    }
}
