using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model = Gfycat.API.Models.User;

namespace Gfycat
{
    public class User : Entity, IUser, IUpdatable
    {
        public string Username { get; internal set; }
        public string Description { get; internal set; }
        public string ProfileUrl { get; internal set; }
        public string Name { get; internal set; }
        public int Views { get; internal set; }
        public bool EmailVerified { get; internal set; }
        public string Url { get; internal set; }
        public DateTime CreationDate { get; internal set; }
        public string ProfileImageUrl { get; internal set; }
        public bool Verified { get; internal set; }
        public int Followers { get; internal set; }
        public int Following { get; internal set; }
        public bool IframeProfileImageVisible { get; internal set; }

        internal User(GfycatClient client, string id) : base(client, id)
        { }

        internal void Update(Model model)
        {
            CreationDate = model.CreationDate;
            Description = model.Description;
            EmailVerified = model.EmailVerified;
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

        public async Task<GfyFeed> GetGfycatFeedAsync(int count = 10, RequestOptions options = null)
        {
            return UserGfyFeed.Create(Client, count, options, Id, await Client.ApiClient.GetUserGfyFeedAsync(Id, count, null, options));
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
