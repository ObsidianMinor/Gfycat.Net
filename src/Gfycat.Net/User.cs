using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gfycat
{
    [JsonArray]
    public class User : ConnectedEntity
    {
        internal User(ExtendedHttpClient client) : base(client) { }

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

        public async Task<IEnumerable<GfycatAlbumInfo>> GetAlbums()
        {
            string endpoint = $"users/{Id}/albums";
            return (await Web.SendRequestAsync<GfycatAlbumResponse>("GET", endpoint)).Albums;
        }

        public async Task<GfycatAlbum> GetAlbumContents(string albumId)
        {
            string endpoint = $"users/{Id}/albums/{albumId}";
            return await Web.SendRequestAsync<GfycatAlbum>("GET", endpoint);
        }
    }
}
