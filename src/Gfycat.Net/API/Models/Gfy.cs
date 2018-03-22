using Gfycat.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Gfycat.API.Models
{
    internal class Gfy
    {
        [JsonProperty("gfyId")]
        internal string Id { get; set; }
        [JsonProperty("gfyNumber")]
        internal long Number { get; set; }
        [JsonProperty("webmUrl")]
        internal string WebmUrl { get; set; }
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
        [JsonProperty("max1mbGif")]
        internal string Max1MbGif { get; set; }
        [JsonProperty("mjpgUrl")]
        internal string MjpgUrl { get; set; }
        [JsonProperty("width")]
        internal int Width { get; set; }
        [JsonProperty("height")]
        internal int Height { get; set; }
        [JsonProperty("avgColor")]
        internal string AverageColor { get; set; }
        [JsonProperty("frameRate")]
        internal double FrameRate { get; set; }
        [JsonProperty("numFrames")]
        internal double NumberOfFrames { get; set; }
        [JsonProperty("mp4Size")]
        internal int Mp4Size { get; set; }
        [JsonProperty("webmSize")]
        internal int WebmSize { get; set; }
        [JsonProperty("gifSize")]
        internal int GifSize { get; set; }
        [JsonProperty("source")]
        internal string Source { get; set; }
        [JsonProperty("createDate"), JsonConverter(typeof(UnixTimeConverter))]
        internal DateTime CreationDate { get; set; }
        [JsonProperty("nsfw")]
        internal NsfwSetting Nsfw { get; set; }
        [JsonProperty("mp4Url")]
        internal string Mp4Url { get; set; }
        [JsonProperty("likes")]
        internal int Likes { get; set; }
        [JsonProperty("published"), JsonConverter(typeof(NumericalBooleanConverter))]
        internal bool Published { get; set; } = true;
        [JsonProperty("dislikes")]
        internal int Dislikes { get; set; }
        [JsonProperty("extraLemmas")]
        internal string ExtraLemmas { get; set; }
        [JsonProperty("md5")]
        internal string Md5 { get; set; }
        [JsonProperty("views")]
        internal int Views { get; set; }
        [JsonProperty("tags")]
        internal IEnumerable<string> Tags { get; set; }
        [JsonProperty("username")]
        internal string Username { get; set; }
        [JsonProperty("gfyName")]
        internal string Name { get; set; }
        [JsonProperty("title")]
        internal string Title { get; set; }
        [JsonProperty("description")]
        internal string Description { get; set; }
        [JsonProperty("languageText")]
        internal string LanguageText { get; set; }
        [JsonProperty("languageCategories")]
        internal IEnumerable<string> LanguageCategories { get; set; }
        [JsonProperty("subreddit")]
        internal string Subreddit { get; set; }
        [JsonProperty("redditId")]
        internal string RedditId { get; set; }
        [JsonProperty("redditIdText")]
        internal string RedditIdText { get; set; }
        [JsonProperty("domainWhitelist")]
        internal IEnumerable<string> DomainWhitelist { get; set; }
    }
}
