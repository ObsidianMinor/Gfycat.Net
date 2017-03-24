using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Model = Gfycat.API.Models.User;

namespace Gfycat
{
    [DebuggerDisplay("Username: {Username}")]
    public class User : Entity, IUser, IUpdatable
    {
        public string Username { get; internal set; }
        public string Description { get; internal set; }
        /// <summary>
        /// The linked website on a user's profile page
        /// </summary>
        public string ProfileUrl { get; internal set; }
        public string Name { get; internal set; }
        public int Views { get; internal set; }
        /// <summary>
        /// A link to the user's Gfycat profile url
        /// </summary>
        public string Url { get; internal set; }
        public DateTime CreationDate { get; internal set; }
        public string ProfileImageUrl { get; internal set; }
        public bool Verified { get; internal set; }
        public int Followers { get; internal set; }
        public int Following { get; internal set; }
        public bool IframeProfileImageVisible { get; internal set; }
        public int PublishedGfys { get; internal set; }
        public int PublishedAlbums { get; internal set; }

        internal User(GfycatClient client, string id) : base(client, id)
        { }

        internal void Update(Model model)
        {
            CreationDate = model.CreationDate;
            Description = model.Description;
            Followers = model.Followers;
            Following = model.Following;
            IframeProfileImageVisible = model.IframeProfileImageVisible;
            Name = model.Name;
            ProfileImageUrl = model.ProfileImageUrl;
            ProfileUrl = model.ProfileUrl;
            Url = model.Url;
            Username = model.Username;
            Verified = model.Verified;
            Views = model.Views;
            PublishedGfys = model.PublishedGfycats;
            PublishedAlbums = model.PublishedAlbums;
        }

        internal static User Create(GfycatClient client, Model model)
        {
            User user = new User(client, model.Id);
            user.Update(model);
            return user;
        }

        public async Task UpdateAsync(RequestOptions options)
        {
            Update(await Client.ApiClient.GetUserAsync(Id, options));
        }

        public async Task<IAlbumInfo> GetAlbumsAsync(RequestOptions options = null)
        {
            return Utils.CreateAlbum(Client, (await Client.ApiClient.GetAlbumsForUserAsync(Id, options)).FirstOrDefault(), Id);
        }

        public async Task<GfyFeed> GetGfyFeedAsync(RequestOptions options = null)
        {
            return UserGfyFeed.Create(Client, options, Id, await Client.ApiClient.GetUserGfyFeedAsync(Id, null, options));
        }

        public async Task FollowAsync(RequestOptions options = null)
        {
            await Client.ApiClient.FollowUserAsync(Id, options);
        }

        public async Task UnfollowAsync(RequestOptions options = null)
        {
            await Client.ApiClient.UnfollowUserAsync(Id, options);
        }

        public async Task<bool> GetFollowingUser(RequestOptions options = null)
        {
            return (await Client.ApiClient.GetFollowingUserAsync(Id, options)) == System.Net.HttpStatusCode.OK;
        }
    }
}
