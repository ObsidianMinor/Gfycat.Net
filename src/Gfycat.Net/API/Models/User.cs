using Gfycat.Converters;
using Newtonsoft.Json;
using System;

namespace Gfycat.API.Models
{
    internal class User
    {
        [JsonProperty("userid")]
        internal string Id { get; set; }
        [JsonProperty("username")]
        internal string Username { get; set; }
        [JsonProperty("description")]
        internal string Description { get; set; }
        [JsonProperty("profileUrl")]
        internal string ProfileUrl { get; set; }
        [JsonProperty("name")]
        internal string Name { get; set; }
        [JsonProperty("views")]
        internal int Views { get; set; }
        [JsonProperty("emailVerified")]
        internal bool EmailVerified { get; set; }
        [JsonProperty("url")]
        internal string Url { get; set; }
        [JsonProperty("createDate"), JsonConverter(typeof(UnixTimeConverter))]
        internal DateTime CreationDate { get; set; }
        [JsonProperty("profileImageUrl")]
        internal string ProfileImageUrl { get; set; }
        [JsonProperty("verified")]
        internal bool Verified { get; set; }
        [JsonProperty("followers")]
        internal int Followers { get; set; }
        [JsonProperty("following")]
        internal int Following { get; set; }
        [JsonProperty("iframeProfileImageVisible")]
        internal bool IframeProfileImageVisible { get; set; }
        [JsonProperty("publishedGfycats")]
        internal int PublishedGfycats { get; set; }
        [JsonProperty("publishedAlbums")]
        internal int PublishedAlbums { get; set; }
    }
}
