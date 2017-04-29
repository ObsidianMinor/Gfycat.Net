using Gfycat.API.Models;
using Newtonsoft.Json;

namespace Gfycat.API
{
    internal class FullGfyResponse
    {
        [JsonProperty("gfyItem")]
        internal FullGfy Gfy { get; set; }
    }
}