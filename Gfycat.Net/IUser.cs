using Newtonsoft.Json;
using System;

namespace Gfycat
{
    [JsonArray]
    public interface IUser
    {
        [JsonProperty("userid")]
        string Id { get; }
        [JsonProperty("username")]
        string Username { get; }
        [JsonProperty("description")]
        string Description { get; }
        [JsonProperty("profileUrl")]
        string ProfileUrl { get; }
        [JsonProperty("name")]
        string Name { get; }
        [JsonProperty("views")]
        int Views { get; }
        [JsonProperty("emailVerified")]
        bool EmailVerified { get; }
        [JsonProperty("url")]
        string Url { get; }
        [JsonProperty("createDate"), JsonConverter(typeof(UnixTimeConverter))]
        DateTime CreationDate { get; }
        [JsonProperty("profileImageUrl")]
        string ProfileImageUrl { get; }
        [JsonProperty("verified")]
        bool Verified { get; }
        [JsonProperty("followers")]
        int Followers { get; }
        [JsonProperty("following")]
        int Following { get; }
        [JsonProperty("iframeProfileImageVisible")]
        bool IframeProfileImageVisible { get; }
    }
}
