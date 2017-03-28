using Gfycat.Converters;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Gfycat.API.Models
{
    internal class GfyParameters
    {
        [JsonProperty("fetchUrl")]
        internal string FetchUrl { get; set; }
        [JsonProperty("title")]
        internal string Title { get; set; }
        [JsonProperty("description")]
        internal string Description { get; set; }
        [JsonProperty("tags")]
        internal IEnumerable<string> Tags { get; set; }
        [JsonProperty("noMd5")]
        internal bool? NoMd5 { get; set; }
        [JsonProperty("private")]
        internal bool? Private { get; set; }
        [JsonProperty("nsfw")]
        internal NsfwSetting? Nsfw { get; set; }
        [JsonProperty("fetchSeconds")]
        internal float? FetchSeconds { get; set; }
        [JsonProperty("fetchMinutes")]
        internal float? FetchMinutes { get; set; }
        [JsonProperty("fetchHours")]
        internal float? FetchHours { get; set; }
        [JsonProperty("captions")]
        internal IEnumerable<Caption> Captions { get; set; }
        [JsonProperty("cut")]
        internal Cut Cut { get; set; }
        [JsonProperty("crop")]
        internal Crop Crop { get; set; }
    }

    internal class Caption
    {
        [JsonProperty("text", Required = Required.Always)]
        internal string Text { get; set; }
        [JsonProperty("startSeconds")]
        internal int? StartSeconds { get; set; }
        [JsonProperty("duration")]
        internal int? Duration { get; set; }
        [JsonProperty("fontHeight")]
        internal int? FontHeight { get; set; }
        [JsonProperty("x")]
        internal int? X { get; set; }
        [JsonProperty("y")]
        internal int? Y { get; set; }
        [JsonProperty("fontHeightRelative")]
        internal int? RelativeFontHeight { get; set; }
        [JsonProperty("xRelative")]
        internal int? RelativeX { get; set; }
        [JsonProperty("yRelative")]
        internal int? RelativeY { get; set; }
    }

    [JsonArray]
    internal class Cut
    {
        [JsonProperty("duration", Required = Required.Always)]
        internal int Duration { get; set; }
        [JsonProperty("start", Required = Required.Always)]
        internal int Start { get; set; }
    }

    [JsonArray]
    internal class Crop
    {
        [JsonProperty("x", Required = Required.Always)]
        internal int X { get; set; }
        [JsonProperty("y", Required = Required.Always)]
        internal int Y { get; set; }
        [JsonProperty("w", Required = Required.Always)]
        internal int W { get; set; }
        [JsonProperty("h", Required = Required.Always)]
        internal int H { get; set; }
    }
}
