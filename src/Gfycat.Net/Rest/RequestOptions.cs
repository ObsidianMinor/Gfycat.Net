using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace Gfycat
{
    public class RequestOptions
    {
        public int? Timeout { get; set; }
        public CancellationToken CancellationToken { get; set; } = CancellationToken.None;
        public RetryMode RetryMode { get; set; }

        internal bool UseAccessToken { get; set; } = true;
        internal IEnumerable<HttpStatusCode> IgnoreCodes { get; set; }

        internal static RequestOptions CreateFromDefaults(GfycatClientConfig config)
        {
            return new RequestOptions()
            {
                Timeout = config.DefaultTimeout,
                RetryMode = config.DefaultRetryMode
            };
        }
    }
}
