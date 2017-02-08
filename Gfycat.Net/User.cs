using Newtonsoft.Json;
using System;

namespace Gfycat
{
    [JsonArray]
    public class User
    {
        [JsonProperty("userid")]
        public string Id { get; set; }
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("profileUrl")]
        public string ProfileUrl { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("views")]
        public int Views { get; set; }
        [JsonProperty("emailVerified")]
        public bool EmailVerified { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("createDate"), JsonConverter(typeof(UnixTimeConverter))]
        public DateTime CreationDate { get; set; }
        [JsonProperty("profileImageUrl")]
        public string ProfileImageUrl { get; set; }
        [JsonProperty("verified")]
        public bool Verified { get; set; }
        [JsonProperty("followers")]
        public int Followers { get; set; }
        [JsonProperty("following")]
        public int Following { get; set; }
        [JsonProperty("iframeProfileImageVisible")]
        public bool IframeProfileImageVisible { get; set; }
    }
}
