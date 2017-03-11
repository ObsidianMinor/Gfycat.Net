using Gfycat.Converters;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Gfycat.API.Models
{
    internal class GfyParameters
    {
        [JsonProperty("fetchUrl", NullValueHandling = NullValueHandling.Ignore)]
        internal string FetchUrl { get; set; }
        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        internal string Title { get; set; }
        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        internal string Description { get; set; }
        [JsonProperty("tags", NullValueHandling = NullValueHandling.Ignore)]
        internal IEnumerable<string> Tags { get; set; }
        [JsonProperty("noMd5", NullValueHandling = NullValueHandling.Ignore)]
        internal bool? NoMd5 { get; set; }
        [JsonProperty("private", NullValueHandling = NullValueHandling.Ignore), JsonConverter(typeof(NumericalBooleanConverter))]
        internal bool? Private { get; set; }
        [JsonProperty("nsfw", NullValueHandling = NullValueHandling.Ignore)]
        internal NsfwSetting? Nsfw { get; set; }
        [JsonProperty("fetchSeconds", NullValueHandling = NullValueHandling.Ignore)]
        internal float? FetchSeconds { get; set; }
        [JsonProperty("fetchMinutes", NullValueHandling = NullValueHandling.Ignore)]
        internal float? FetchMinutes { get; set; }
        [JsonProperty("fetchHours", NullValueHandling = NullValueHandling.Ignore)]
        internal float? FetchHours { get; set; }
        [JsonProperty("captions", NullValueHandling = NullValueHandling.Ignore)]
        internal IEnumerable<Caption> Captions { get; set; }
        [JsonProperty("cut", NullValueHandling = NullValueHandling.Ignore)]
        internal Cut Cut { get; set; }
        [JsonProperty("crop", NullValueHandling = NullValueHandling.Ignore)]
        internal Crop Crop { get; set; }
    }

    internal class Caption
    {
        [JsonProperty("text", Required = Required.Always)]
        internal string Text { get; set; }
        [JsonProperty("startSeconds", NullValueHandling = NullValueHandling.Ignore)]
        internal int? StartSeconds { get; set; }
        [JsonProperty("duration", NullValueHandling = NullValueHandling.Ignore)]
        internal int? Duration { get; set; }
        [JsonProperty("fontHeight", NullValueHandling = NullValueHandling.Ignore)]
        internal int? FontHeight { get; set; }
        [JsonProperty("x", NullValueHandling = NullValueHandling.Ignore)]
        internal int? X { get; set; }
        [JsonProperty("y", NullValueHandling = NullValueHandling.Ignore)]
        internal int? Y { get; set; }
        [JsonProperty("fontHeightRelative", NullValueHandling = NullValueHandling.Ignore)]
        internal int? RelativeFontHeight { get; set; }
        [JsonProperty("xRelative", NullValueHandling = NullValueHandling.Ignore)]
        internal int? RelativeX { get; set; }
        [JsonProperty("yRelative", NullValueHandling = NullValueHandling.Ignore)]
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
