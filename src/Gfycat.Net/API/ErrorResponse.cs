using Newtonsoft.Json;

namespace Gfycat.API
{
    internal class ErrorResponse
    {
        [JsonProperty("errorMessage"), JsonConverter(typeof(Converters.ExceptionMessageConverter))] // WHY MUST IT BE LIKE THIS
        internal ExceptionMessage Error { get; set; }
        
        [JsonProperty("message")]
        internal string Message { get; set; }
    }
}
