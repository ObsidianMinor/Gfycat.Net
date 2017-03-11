using Gfycat.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace Gfycat.API.Models
{
    internal class ApplicationInfo
    {
        [JsonProperty("contact_name")]
        internal string ContactName { get; set; }
        [JsonProperty("web_url")]
        internal string WebUrl { get; set; }
        [JsonProperty("company")]
        internal string Company { get; set; }
        [JsonProperty("createDate", ItemConverterType = typeof(UnixTimeConverter))]
        internal DateTime CreationDate { get; set; }
        [JsonProperty("app_name")]
        internal string AppName { get; set; }
        [JsonProperty("redirect_uris")]
        internal IEnumerable<string> RedirectUris { get; set; }
        [JsonProperty("app_type", ItemConverterType = typeof(StringEnumConverter))]
        internal AppType AppType { get; set; }
        [JsonProperty("username")]
        internal string Username { get; set; }
        [JsonProperty("email")]
        internal string Email { get; set; }
        [JsonProperty("clientId")]
        internal string ClientId { get; set; }
    }
}
