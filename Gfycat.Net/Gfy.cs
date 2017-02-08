using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Gfycat
{
    public class Gfy
    {
        public string Id { get; set; }
        public long Number { get; set; }
        public string WebmUrl { get; set; }
        public string GifUrl { get; set; }
        public string MobileUrl { get; set; }
        public string MobilePosterUrl { get; set; }
        public string PosterUrl { get; set; }
        public string Thumb360Url { get; set; }
        public string Thumb360PosterUrl { get; set; }
        public string Thumb100PosterUrl { get; set; }
        public string Max5MbGif { get; set; }
        public string Max2MbGif { get; set; }
        public string MjpgUrl { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int AverageColor { get; set; }
        public int FrameRate { get; set; }
        public int NumberOfFrames { get; set; }
        public int Mp4Size { get; set; }
        public int WebmSize { get; set; }
        public int GifSize { get; set; }
        [JsonProperty("createDate", ItemConverterType = typeof(UnixTimeConverter))]
        public DateTime CreationDate { get; set; }
        public bool Nsfw { get; set; }
        public string Mp4Url { get; set; }
        public int Likes { get; set; }
        public bool Published { get; set; }
        public int Dislikes { get; set; }
        public string ExtraLemmas { get; set; }
        public string Md5 { get; set; }
        public int Views { get; set; }
        [JsonProperty("tags")]
        public List<string> Tags { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string LanguageText { get; set; }
        public string LanguageCategories { get; set; }
        public string Subreddit { get; set; }
        public string RedditId { get; set; }
        public string RedditIdText { get; set; }
        public IEnumerable<string> DomainWhitelist { get; set; }
    }
}
