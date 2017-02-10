using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Gfycat
{
    public class Gfy
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("number")]
        public long Number { get; set; }
        [JsonProperty("webmUrl")]
        public string WebmUrl { get; set; }
        [JsonProperty("gifUrl")]
        public string GifUrl { get; set; }
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
        public string Max5MbGif { get; set; }
        [JsonProperty("max2mbGif")]
        public string Max2MbGif { get; set; }
        [JsonProperty("max1mbGif")]
        public string Max1MbGif { get; set; }
        [JsonProperty("mjpgUrl")]
        public string MjpgUrl { get; set; }
        [JsonProperty("width")]
        public int Width { get; set; }
        [JsonProperty("height")]
        public int Height { get; set; }
        [JsonProperty("avgColor")]
        public int AverageColor { get; set; }
        [JsonProperty("frameRate")]
        public int FrameRate { get; set; }
        [JsonProperty("numFrames")]
        public int NumberOfFrames { get; set; }
        [JsonProperty("mp4Size")]
        public int Mp4Size { get; set; }
        [JsonProperty("webmSize")]
        public int WebmSize { get; set; }
        [JsonProperty("gifSize")]
        public int GifSize { get; set; }
        [JsonProperty("createDate", ItemConverterType = typeof(UnixTimeConverter))]
        public DateTime CreationDate { get; set; }
        [JsonProperty("nsfw")]
        public bool Nsfw { get; set; }
        [JsonProperty("mp4Url")]
        public string Mp4Url { get; set; }
        [JsonProperty("likes")]
        public int Likes { get; set; }
        [JsonProperty("published")]
        public bool Published { get; set; }
        [JsonProperty("dislikes")]
        public int Dislikes { get; set; }
        [JsonProperty("extraLemmas")]
        public string ExtraLemmas { get; set; }
        [JsonProperty("md5")]
        public string Md5 { get; set; }
        [JsonProperty("views")]
        public int Views { get; set; }
        [JsonProperty("tags")]
        public List<string> Tags { get; set; }
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("languageText")]
        public string LanguageText { get; set; }
        [JsonProperty("languageCategories")]
        public string LanguageCategories { get; set; }
        [JsonProperty("subreddit")]
        public string Subreddit { get; set; }
        [JsonProperty("redditId")]
        public string RedditId { get; set; }
        [JsonProperty("redditIdText")]
        public string RedditIdText { get; set; }
        [JsonProperty("domainWhitelist")]
        public IEnumerable<string> DomainWhitelist { get; set; }
    }
}
