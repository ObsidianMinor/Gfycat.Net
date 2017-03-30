using Newtonsoft.Json;
using System.Collections.Generic;

namespace Gfycat.API.Models
{
    internal class Album
    {
        [JsonProperty("coverImageUrl")]
        internal string CoverImageUrl { get; set; }
        [JsonProperty("coverImageUrl-mobile")]
        internal string CoverImageUrlMobile { get; set; }
        [JsonProperty("id")]
        internal string Id { get; set; }
        [JsonProperty("title")]
        internal string Title { get; set; }
        [JsonProperty("linkText")]
        internal string LinkText { get; set; }
        [JsonProperty("nsfw")]
        internal NsfwSetting Nsfw { get; set; }
        [JsonProperty("published"), JsonConverter(typeof(Converters.NumericalBooleanConverter))]
        internal bool Published { get; set; } = true; // published until proven otherwise
        [JsonProperty("order")]
        internal int Order { get; set; }
        [JsonProperty("publishedGfys")]
        internal IEnumerable<Gfy> PublishedGfys { get; set; }
        [JsonProperty("gfyCount")]
        internal int GfyCount { get; set; }
    }
}
