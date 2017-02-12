using System.Collections.Generic;
using Newtonsoft.Json;

namespace Gfycat
{
    public class GfyCreationParameters
    {
        [JsonProperty("fetchUrl", NullValueHandling = NullValueHandling.Ignore)]
        public string FetchUrl { get; internal set; }
        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }
        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }
        [JsonProperty("tags", NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<string> Tags { get; set; }
        /// <summary
        /// Instructs the uploader to skip the file duplication check
        /// </summary
        [JsonProperty("noMd5", NullValueHandling = NullValueHandling.Ignore)]
        public bool? NoMd5 { get; set; }
        [JsonProperty("private", NullValueHandling = NullValueHandling.Ignore), JsonConverter(typeof(NumericalBooleanConverter))]
        public bool? Private { get; set; }
        [JsonProperty("nsfw", NullValueHandling = NullValueHandling.Ignore)]
        public NsfwSetting? Nsfw { get; set; }
        [JsonProperty("fetchSeconds", NullValueHandling = NullValueHandling.Ignore)]
        public float? FetchSeconds { get; set; }
        [JsonProperty("fetchMinutes", NullValueHandling = NullValueHandling.Ignore)]
        public float? FetchMinutes { get; set; }
        [JsonProperty("fetchHours", NullValueHandling = NullValueHandling.Ignore)]
        public float? FetchHours { get; set; }
        [JsonProperty("captions", NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<Caption> Captions { get; set; }
        [JsonProperty("cut", NullValueHandling = NullValueHandling.Ignore)]
        public Cut Cut { get; set; }
        [JsonProperty("crop", NullValueHandling = NullValueHandling.Ignore)]
        public Crop Crop { get; set; }
    }
    
    public class Caption
    {
        [JsonProperty("text", Required = Required.Always)]
        public string Text { get; set; }
        [JsonProperty("startSeconds", NullValueHandling = NullValueHandling.Ignore)]
        public int? StartSeconds { get; set; }
        [JsonProperty("duration", NullValueHandling = NullValueHandling.Ignore)]
        public int? Duration { get; set; }
        [JsonProperty("fontHeight", NullValueHandling = NullValueHandling.Ignore)]
        public int? FontHeight { get; set; }
        [JsonProperty("x", NullValueHandling = NullValueHandling.Ignore)]
        public int? X { get; set; }
        [JsonProperty("y", NullValueHandling = NullValueHandling.Ignore)]
        public int? Y { get; set; }
        [JsonProperty("fontHeightRelative", NullValueHandling = NullValueHandling.Ignore)]
        public int? RelativeFontHeight { get; set; }
        [JsonProperty("xRelative", NullValueHandling = NullValueHandling.Ignore)]
        public int? RelativeX { get; set; }
        [JsonProperty("yRelative", NullValueHandling = NullValueHandling.Ignore)]
        public int? RelativeY { get; set; }
    }

    [JsonArray]
    public class Cut
    {
        [JsonProperty("duration", Required = Required.Always)]
        public int Duration { get; set; }
        [JsonProperty("start", Required = Required.Always)]
        public int Start { get; set; }
    }

    [JsonArray]
    public class Crop
    {
        [JsonProperty("x", Required = Required.Always)]
        public int X { get; set; }
        [JsonProperty("y", Required = Required.Always)]
        public int Y { get; set; }
        [JsonProperty("w", Required = Required.Always)]
        public int W { get; set; }
        [JsonProperty("h", Required = Required.Always)]
        public int H { get; set; }
    }
}
