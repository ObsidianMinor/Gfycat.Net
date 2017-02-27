using System.Threading;

namespace Gfycat
{
    public class RequestOptions
    {
        public int? Timeout { get; set; }
        public CancellationToken CancellationToken { get; set; } = CancellationToken.None;

        internal bool UseAccessToken { get; set; } = true;

        public RetryMode RetryMode { get; set; }
    }
}
