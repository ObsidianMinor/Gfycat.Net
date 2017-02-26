using System.Net.Http;

namespace Gfycat
{
    public class GfycatClientConfig
    {
        /// <summary>
        /// Overrides the rest client for this Gfycat client
        /// </summary>
        public IRestClient RestClient { get; set; }
    }
}
