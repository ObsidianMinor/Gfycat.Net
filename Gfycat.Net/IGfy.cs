using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Gfycat
{
    public interface IGfy
    {
        string Id { get; set; }
        long Number { get; set; }
        string WebmUrl { get; set; }
        string GifUrl { get; set; }
        string MobileUrl { get; set; }
        string MobilePosterUrl { get; set; }
        string PosterUrl { get; set; }
        string Thumb360Url { get; set; }
        string Thumb360PosterUrl { get; set; }
        string Thumb100PosterUrl { get; set; }
        string Max5MbGif { get; set; }
        string Max2MbGif { get; set; }
        string MjpgUrl { get; set; }
        int Width { get; set; }
        int Height { get; set; }
        int AverageColor { get; set; }
        int FrameRate { get; set; }
        int NumberOfFrames { get; set; }
        int Mp4Size { get; set; }
        int WebmSize { get; set; }
        int GifSize { get; set; }
        [JsonProperty("createDate", ItemConverterType = typeof(UnixTimeConverter))]
        DateTime CreationDate { get; set; }
        bool Nsfw { get; set; }
        string Mp4Url { get; set; }
        int Likes { get; set; }
        bool Published { get; set; }
        int Dislikes { get; set; }
        string ExtraLemmas { get; set; }
        string Md5 { get; set; }
        int Views { get; set; }
        [JsonProperty("tags")]
        List<string> Tags { get; set; }
        string Username { get; set; }
        string Name { get; set; }
        string Title { get; set; }
        string Description { get; set; }
        string LanguageText { get; set; }
        string LanguageCategories { get; set; }
        string Subreddit { get; set; }
        string RedditId { get; set; }
        string RedditIdText { get; set; }
        IEnumerable<string> DomainWhitelist { get; set; }
    }
}
