using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Model = Gfycat.API.Models.User;

namespace Gfycat
{
    public class User : Entity, IUser
    {
        public string Username { get; private set; }
        public string Description { get; private set; }
        public string ProfileUrl { get; private set; }
        public string Name { get; private set; }
        public int Views { get; private set; }
        public bool EmailVerified { get; private set; }
        public string Url { get; private set; }
        public DateTime CreationDate { get; private set; }
        public string ProfileImageUrl { get; private set; }
        public bool Verified { get; private set; }
        public int Followers { get; private set; }
        public int Following { get; private set; }
        public bool IframeProfileImageVisible { get; private set; }

        internal User(GfycatClient client, string id) : base(client, id)
        { }

        internal static User Create(GfycatClient client, Model model)
        {
            return new User(client, model.Id)
            {
                CreationDate = model.CreationDate,
                Description = model.Description,
                EmailVerified = model.EmailVerified,
                Followers = model.Followers,
                Following = model.Following,
                IframeProfileImageVisible = model.IframeProfileImageVisible,
                Name = model.Name,
                ProfileImageUrl = model.ProfileImageUrl,
                ProfileUrl = model.ProfileUrl,
                Url = model.Url,
                Username = model.Username,
                Verified = model.Verified,
                Views = model.Views
            };
        }

        public async Task<IEnumerable<GfycatAlbumInfo>> GetAlbumsAsync(RequestOptions options = null)
        {
            string endpoint = $"users/{Id}/albums";
            IEnumerable<GfycatAlbumInfo> albums = await Client.SendAsync<IEnumerable<GfycatAlbumInfo>>("GET", endpoint, options);
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

        public async Task<GfycatAlbum> GetAlbumContentsByLinkTextAsync(string albumLinkText, RequestOptions options = null)
        {
            string endpoint = $"users/{Id}/album_links/{albumLinkText}";
            return await Client.SendAsync<GfycatAlbum>("GET", endpoint, options);
        }

        public Task<GfycatFeed> GetGfycatFeedAsync(int? count = null, string cursor = null, RequestOptions options = null)
        {
            string queryString = Utils.CreateQueryString(new Dictionary<string, object>()
            {
                { "count", count },
                { "cursor", cursor }
            });
            return Client.SendAsync<GfycatFeed>("GET", $"users/{Id}/gfycats{queryString}", options);
        }
    }
}
