using Newtonsoft.Json;
using System.Collections.Generic;

namespace Gfycat
{
    public class GfycatAlbum
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("linkText")]
        public string LinkText { get; set; }
        [JsonProperty("nsfw")]
        public NsfwSetting Nsfw { get; set; }
        [JsonProperty("published")]
        public bool Published { get; set; }
        [JsonProperty("order")]
        public int Order { get; set; }
        [JsonProperty("coverImageUrl")]
        public string CoverImageUrl { get; set; }
        [JsonProperty("coverImageUrl-mobile")]
        public string CoverImageMobileUrl { get; set; }
        [JsonProperty("publishedGfys")]
        public IEnumerable<Gfy> PublishedGfys { get; set; }
        [JsonProperty("gfyCount")]
        public int GfyCount { get; set; }
    }
}
