using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gfycat
{
    [JsonArray]
    public class User : ConnectedEntity, IUser
    {
        [JsonProperty("userid")]
        public string Id { get; private set; }
        [JsonProperty("username")]
        public string Username { get; private set; }
        [JsonProperty("description")]
        public string Description { get; private set; }
        [JsonProperty("profileUrl")]
        public string ProfileUrl { get; private set; }
        [JsonProperty("name")]
        public string Name { get; private set; }
        [JsonProperty("views")]
        public int Views { get; private set; }
        [JsonProperty("emailVerified")]
        public bool EmailVerified { get; private set; }
        [JsonProperty("url")]
        public string Url { get; private set; }
        [JsonProperty("createDate"), JsonConverter(typeof(UnixTimeConverter))]
        public DateTime CreationDate { get; private set; }
        [JsonProperty("profileImageUrl")]
        public string ProfileImageUrl { get; private set; }
        [JsonProperty("verified")]
        public bool Verified { get; private set; }
        [JsonProperty("followers")]
        public int Followers { get; private set; }
        [JsonProperty("following")]
        public int Following { get; private set; }
        [JsonProperty("iframeProfileImageVisible")]
        public bool IframeProfileImageVisible { get; private set; }

        public async Task<IEnumerable<GfycatAlbumInfo>> GetAlbumsAsync()
        {
            string endpoint = $"users/{Id}/albums";
            IEnumerable<GfycatAlbumInfo> albums = (await Web.SendRequestAsync<GfycatAlbumResponse>("GET", endpoint)).Albums;
            RecursiveSetOwners(albums);
            return albums;
        }

        private void RecursiveSetOwners(IEnumerable<GfycatAlbumInfo> albums)
        {
            foreach(GfycatAlbumInfo album in albums)
            {
                album.Owner = this;
                RecursiveSetOwners(album.Subalbums);
            }
        }

        public async Task<GfycatAlbumInfo> GetAlbumContentsByLinkTextAsync(string albumLinkText)
        {
            string endpoint = $"users/{Id}/album_links/{albumLinkText}";
            return await Web.SendRequestAsync<GfycatAlbumInfo>("GET", endpoint);
        }

        public Task<GfycatFeed> GetGfycatFeedAsync(int? count = null, string cursor = null)
        {
            string queryString = InternalClient.CreateQueryString(new Dictionary<string, object>()
            {
                { "count", count },
                { "cursor", cursor }
            });
            return Web.SendRequestAsync<GfycatFeed>("GET", $"users/{Id}/gfycats{queryString}");
        }
    }
}
