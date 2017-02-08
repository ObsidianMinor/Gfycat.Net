using System.Collections.Generic;
using Newtonsoft.Json;

namespace Gfycat
{
    public class GfycatCreationParameters
    {
        [JsonProperty("fetchUrl")]
        public Optional<string> FetchUrl { get; internal set; }
        [JsonProperty("title")]
        public Optional<string> Title { get; set; }
        [JsonProperty("description")]
        public Optional<string> Description { get; set; }
        [JsonProperty("tags")]
        public Optional<IEnumerable<string>> Tags { get; set; }
        /// <summary>
        /// Instructs the uploader to skip the file duplication check
        /// </summary>
        [JsonProperty("noMd5")]
        public Optional<bool> NoMd5 { get; set; }
        [JsonProperty("private")]
        public Optional<bool> Private { get; set; }
        [JsonProperty("nsfw")]
        public Optional<NsfwSetting> Nsfw { get; set; }
        [JsonProperty("fetchSeconds")]
        public Optional<float> FetchSeconds { get; set; }
        [JsonProperty("fetchMinutes")]
        public Optional<float> FetchMinutes { get; set; }
        [JsonProperty("fetchHours")]
        public Optional<float> FetchHours { get; set; }
        [JsonProperty("captions")]
        public Optional<IEnumerable<Caption>> Captions { get; set; }
        [JsonProperty("cut")]
        public Optional<Cut> Cut { get; set; }
        [JsonProperty("crop")]
        public Optional<Crop> Crop { get; set; }
    }
    
    public class Caption
    {
        [JsonProperty("text", Required = Required.Always)]
        public string Text { get; set; }
        [JsonProperty("startSeconds", DefaultValueHandling = DefaultValueHandling.Include)]
        public Optional<int> StartSeconds { get; set; }
        [JsonProperty("duration")]
        public Optional<int> Duration { get; set; }
        [JsonProperty("fontHeight")]
        public Optional<int> FontHeight { get; set; }
        [JsonProperty("x")]
        public Optional<int> X { get; set; }
        [JsonProperty("y")]
        public Optional<int> Y { get; set; }
        [JsonProperty("fontHeightRelative")]
        public Optional<int> RelativeFontHeight { get; set; }
        [JsonProperty("xRelative")]
        public Optional<int> RelativeX { get; set; }
        [JsonProperty("yRelative")]
        public Optional<int> RelativeY { get; set; }
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
