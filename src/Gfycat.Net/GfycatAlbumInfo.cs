using Newtonsoft.Json;
using System.Collections.Generic;

namespace Gfycat
{
    public class GfycatAlbumInfo
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("linkText")]
        public string LinkText { get; set; }
        [JsonProperty("folderSubType")]
        public string FolderSubType { get; set; }
        [JsonProperty("coverImageUrl")]
        public string CoverImageUrl { get; set; }
        [JsonProperty("coverImageUrl-mobile")]
        public string CoverMobileImageUrl { get; set; }
        [JsonProperty("width")]
        public int Width { get; set; }
        [JsonProperty("height")]
        public int Height { get; set; }
        [JsonProperty("mp4Url")]
        public string Mp4Url { get; set; }
        [JsonProperty("webmUrl")]
        public string WebmUrl { get; set; }
        [JsonProperty("webpUrl")]
        public string WebpUrl { get; set; }
        [JsonProperty("mobileUrl")]
        public string MobileUrl { get; set; }
        [JsonProperty("mobilePosterUrl")]
        public string MobilePosterUrl { get; set; }
        [JsonProperty("posterUrl")]
        public string PosterUrl { get; set; }
        [JsonProperty("thumb360Url")]
        public string Thumb360Url { get; set; }
        [JsonProperty("thumb360PosterUrl")]
        public string Thumb360PosterUrl { get; set; }
        [JsonProperty("thumb100PosterUrl")]
        public string Thumb100PosterUrl { get; set; }
        [JsonProperty("max5mbGif")]
        public string Max5mbGif { get; set; }
        [JsonProperty("max2mbGif")]
        public string Max2mbGif { get; set; }
        [JsonProperty("miniUrl")]
        public string MiniUrl { get; set; }
        [JsonProperty("miniPosterUrl")]
        public string MiniPosterUrl { get; set; }
        [JsonProperty("mjpgUrl")]
        public string MjpgUrl { get; set; }
        [JsonProperty("gifUrl")]
        public string GifUrl { get; set; }
        [JsonProperty("published"), JsonConverter(typeof(NumericalBooleanConverter))]
        public bool Published { get; set; }
        [JsonProperty("nodes")]
        public IEnumerable<GfycatAlbumInfo> Subalbums { get; set; }
    }
}
