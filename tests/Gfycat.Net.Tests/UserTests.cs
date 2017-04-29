using System;
using Xunit;
using static Gfycat.Net.Tests.RestFakes.Resources;

namespace Gfycat.Net.Tests
{
    [Trait("Category", "User")]
    public class UserTests
    {
        GfycatClient Client = Utils.MakeClient();

        [Theory(DisplayName = "Invalid user throws")]
        [InlineData(InvalidUserName)]
        public async void InvalidUserAsync(string username)
        {
            await Assert.ThrowsAsync(typeof(GfycatException), async () => await Client.GetUserAsync(username));
        }

        [Theory(DisplayName = "Get valid user exists returns true")]
        [InlineData(ValidUserName)]
        public async void GetUserExistsAsync(string username)
        {
            Assert.True(await Client.GetUserExistsAsync(username));
        }

        [Theory(DisplayName = "Get invalid user exists returns false")]
        [InlineData(InvalidUserName)]
        public async void GetInvalidUserExistsAsync(string username)
        {
            Assert.False(await Client.GetUserExistsAsync(InvalidUserName));
        }

        [Theory(DisplayName = "Try get invalid user returns null")]
        [InlineData(InvalidUserName)]
        public async void TryGetInvalidUserAsync(string username)
        {
            User invalidUser = await Client.TryGetUserAsync(username);
            Assert.Null(invalidUser);
        }

        [Theory(DisplayName = "Try get valid user isn't null")]
        [InlineData(ValidUserName)]
        public async void TryGetValidUserAsync(string username)
        {
            User validUser = await Client.TryGetUserAsync(username);
            Assert.NotNull(validUser);
        }

        [Theory(DisplayName = "Getting a valid user’s public details")]
        [InlineData(ValidUserName)]
        public async void ValidUserInfoAsync(string username)
        {
            User user = await Client.GetUserAsync(username);
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

        [Theory(DisplayName = "Valid user can update")]
        [InlineData(ValidUserName)]
        public async void ValidUserUpdatesAsync(string username)
        {
            User user = await Client.GetUserAsync(username);
            await user.UpdateAsync();
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

        [Theory(DisplayName = "Isn't following valid user")]
        [InlineData(ValidUserName)]
        public async void ValidUserFollowingAsync(string username)
        {
            User user = await Client.GetUserAsync(username);
            Assert.False(await user.GetFollowingUser());
        }

        [Theory(DisplayName = "Follow user")]
        [InlineData(ValidUserName)]
        public async void FollowValidUserAsync(string username)
        {
            User user = await Client.GetUserAsync(username);
            await user.FollowAsync();
            Assert.True(await user.GetFollowingUser());
        }

        [Theory(DisplayName = "Unfollow user")]
        [InlineData(ValidUserName)]
        public async void UnfollowValidUserAsync(string username)
        {
            User user = await Client.GetUserAsync(username);
            await user.UnfollowAsync();
            Assert.False(await user.GetFollowingUser());
        }
    }
}
