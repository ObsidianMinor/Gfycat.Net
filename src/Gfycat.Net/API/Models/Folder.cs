using Newtonsoft.Json;
using System.Collections.Generic;

namespace Gfycat.API.Models
{
    internal class Folder
    {
        [JsonProperty("id")]
        internal string Id { get; set; }
        [JsonProperty("title")]
        internal string Title { get; set; }
        [JsonProperty("gfyCount")]
        internal int GfyCount { get; set; }
        [JsonProperty("publishedGfys")]
        internal IEnumerable<Gfy> PublishedGfys { get; set; }
    }
}
