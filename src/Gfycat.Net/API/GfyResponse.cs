using Newtonsoft.Json;

namespace Gfycat.API
{
    internal class GfyResponse
    {
        [JsonProperty("gfyItem")]
        internal Models.Gfy GfyItem { get; set; }
    }
}
