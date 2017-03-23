using Newtonsoft.Json;
using System.Collections.Generic;

namespace Gfycat.API.Models
{
    internal class AlbumInfo
    {
        [JsonProperty("id")]
        internal string Id { get; set; }
        [JsonProperty("title")]
        internal string Title { get; set; }
        [JsonProperty("published", ItemConverterType = typeof(Converters.NumericalBooleanConverter))]
        internal bool Published { get; set; }
        [JsonProperty("nodes")]
        internal IEnumerable<AlbumInfo> Nodes { get; set; }
        [JsonProperty("folderSubType")]
        internal string FolderSubType { get; set; }
        [JsonProperty("coverImageUrl")]
        internal string CoverImageUrl { get; set; }
        [JsonProperty("coverImageUrl-mobile")]
        internal string CoverImageUrlMobile { get; set; }
        [JsonProperty("mp4Url")]
        internal string Mp4Url { get; set; }
        [JsonProperty("webmUrl")]
        internal string WebmUrl { get; set; }
        [JsonProperty("webpUrl")]
        internal string WebpUrl { get; set; }
        [JsonProperty("gifUrl")]
        internal string GifUrl { get; set; }
        [JsonProperty("mobileUrl")]
        internal string MobileUrl { get; set; }
        [JsonProperty("mobilePosterUrl")]
        internal string MobilePosterUrl { get; set; }
        [JsonProperty("posterUrl")]
        internal string PosterUrl { get; set; }
        [JsonProperty("thumb360Url")]
        internal string Thumb360Url { get; set; }
        [JsonProperty("thumb360PosterUrl")]
        internal string Thumb360PosterUrl { get; set; }
        [JsonProperty("thumb100PosterUrl")]
        internal string Thumb100PosterUrl { get; set; }
        [JsonProperty("max5mbGif")]
        internal string Max5MbGif { get; set; }
        [JsonProperty("max2mbGif")]
        internal string Max2MbGif { get; set; }
        [JsonProperty("mjpgUrl")]
        internal string MjpgUrl { get; set; }
        [JsonProperty("miniUrl")]
        internal string MiniUrl { get; set; }
        [JsonProperty("miniPosterUrl")]
        internal string MiniPosterUrl { get; set; }
        [JsonProperty("width")]
        internal int Width { get; set; }
        [JsonProperty("height")]
        internal int Height { get; set; }
    }
}
