using Newtonsoft.Json;

namespace Gfycat
{
    public class GfycatOperation
    {
        public Operation Operation { get; set; } // I couldn't use the string enum converter with an array of constructor args because CLS compliance
        [JsonProperty("op")]
        private string _operation { get => Operation.ToString().ToLowerInvariant(); }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public enum Operation
    {
        Add,
        Remove,
        Replace
    }
}
