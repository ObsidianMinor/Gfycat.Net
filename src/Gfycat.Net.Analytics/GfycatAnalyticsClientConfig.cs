using System;
using Gfycat.Rest;
using System.Linq;

namespace Gfycat.Analytics
{
    public class GfycatAnalyticsClientConfig
    {
        public const string BaseImpressionsUrl = "https://px.gfycat.com/px.gif";
        public const string BaseAnalyticsUrl = "https://metrics.gfycat.com";

        public string AppId { get; }
        public string AppName { get; }
        public Version AppVersion { get; }
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

        public GfycatAnalyticsClientConfig(string appName, string appId, Version appVersion) : this(appName, appId, appVersion, GenerateCookie()) { }

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
