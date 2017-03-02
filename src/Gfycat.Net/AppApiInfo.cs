using Gfycat.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace Gfycat
{
    public class AppApiInfo
    {
        [JsonProperty("contact_name")]
        public string ContactName { get; private set; }

        [JsonProperty("web_url")]
        public string WebUrl { get; private set; }

        [JsonProperty("company")]
        public string Company { get; private set; }

        [JsonProperty("createDate", ItemConverterType = typeof(UnixTimeConverter))]
        public DateTime CreationDate { get; private set; }

        [JsonProperty("app_name")]
        public string AppName { get; private set; }

        [JsonProperty("redirect_uris")]
        public IEnumerable<string> RedirectUris { get; private set; }

        [JsonProperty("app_type", ItemConverterType = typeof(StringEnumConverter))]
        public AppType AppType { get; private set; }

        [JsonProperty("username")]
        public string Username { get; private set; }

        [JsonProperty("email")]
        public string Email { get; private set; }

        [JsonProperty("clientId")]
        public string ClientId { get; private set; }
    }
}