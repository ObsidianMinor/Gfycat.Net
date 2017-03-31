using System;
using Xunit;

namespace Gfycat.Net.Tests
{
    public class UserTests
    {
        [Fact(DisplayName = "Invalid user throws")]
        public async void InvalidUserAsync()
        {
            (GfycatClient client, MockRestClient rest) = Utils.MakeClient();
            await Assert.ThrowsAsync(typeof(GfycatException), async () => await client.GetUserAsync("invalidUser"));
        }

        [Fact(DisplayName = "Valid user isn't null")]
        public async void ValidUserAsync()
        {
            (GfycatClient client, MockRestClient rest) = Utils.MakeClient();
            User user = await client.GetUserAsync("validUser");
            Assert.NotNull(user);
        }

        [Fact(DisplayName = "Valid user has correct info")]
        public async void ValidUserInfoAsync()
        {
            (GfycatClient client, MockRestClient rest) = Utils.MakeClient();
            User user = await client.GetUserAsync("validUser");
            Assert.Equal("validUser", user.Username);
            Assert.Equal("validUser", user.Id);
            Assert.Equal("validUser", user.Name);
            Assert.Equal("Testing Gfycat.Net", user.Description);
            Assert.Equal("https://github.com/ObsidianMinor/Gfycat.Net", user.ProfileUrl);
            Assert.Equal("https://profiles.gfycat.com/9c907b40aa37fbc7e8655bfe80a75d11eb0286e78f5a3cababf93c32a89b723b.png", user.ProfileImageUrl);
            Assert.Equal(false, user.IframeProfileImageVisible);
            Assert.Equal(546, user.Views);
            Assert.Equal(false, user.Verified);
            Assert.Equal(1, user.Followers);
            Assert.Equal(0, user.Following);
            Assert.Equal(15, user.PublishedGfys);
            Assert.Equal(1, user.PublishedAlbums);
            Assert.Equal(new DateTime(2017, 2, 6, 2, 11, 29, DateTimeKind.Utc), user.CreationDate);
            Assert.Equal("https://gfycat.com/@obsidianminor", user.Url);
        }

        [Fact(DisplayName = "Isn't following valid user")]
        public async void ValidUserFollowingAsync()
        {
            (GfycatClient client, MockRestClient rest) = Utils.MakeClient();
            User user = await client.GetUserAsync(MockRestClient.ValidUser);
            Assert.Equal(false, await user.GetFollowingUser());
        }

        [Fact(DisplayName = "Follow user")]
        public async void FollowValidUserAsync()
        {
            (GfycatClient client, MockRestClient rest) = Utils.MakeClient();
            User user = await client.GetUserAsync(MockRestClient.ValidUser);
            await user.FollowAsync();
            Assert.True(rest.State.FollowUserSuccess);
        }

        [Fact(DisplayName = "Unfollow user")]
        public async void UnfollowValidUserAsync()
        {
            (GfycatClient client, MockRestClient rest) = Utils.MakeClient();
            User user = await client.GetUserAsync(MockRestClient.ValidUser);
            await user.UnfollowAsync();
            Assert.True(rest.State.UnfollowUserSuccess);
        }

        public async void UserFeedValidAsync()
        {
            (GfycatClient client, MockRestClient rest) = Utils.MakeClient();
            User user = await client.GetUserAsync(MockRestClient.ValidUser);
            GfyFeed feed = await user.GetGfyFeedAsync();
        }
    }
}
