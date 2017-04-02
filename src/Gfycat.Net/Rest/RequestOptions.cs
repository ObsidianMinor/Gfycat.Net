using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace Gfycat
{
    /// <summary>
    /// Specifies options for this REST request
    /// </summary>
    public class RequestOptions
    {
        /// <summary>
        /// Sets the timeout for this request
        /// </summary>
        public int Timeout { get; set; }
        /// <summary>
        /// Sets the cancellation token for this request
        /// </summary>
        public CancellationToken CancellationToken { get; set; } = CancellationToken.None;
        /// <summary>
        /// Sets the retry mode of this request
        /// </summary>
        public RetryMode RetryMode { get; set; }

        internal bool UseAccessToken { get; set; } = true;
        internal IEnumerable<HttpStatusCode> IgnoreCodes { get; set; }

        internal static RequestOptions CreateFromDefaults(GfycatClientConfig config)
        {
            return new RequestOptions()
            {
                Timeout = config.DefaultTimeout,
                RetryMode = config.DefaultRetryMode,
            };
        }
        
        internal RequestOptions Clone()
        {
            return (RequestOptions)MemberwiseClone();
        }
    }
}
