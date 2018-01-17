using System;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RichardSzalay.MockHttp;
using Xunit;

namespace Gfycat.Tests
{
    [Trait("Category", "Authentication")]
    public class AuthenticationTests
    {
        const string client_id = "clientId";
        const string client_secret = "clientSecret";
        const string access_token = "eyJhbGciOiJIUzIzNiIsfnR5cCI6IkpXVCJ9.eyJleHAiOjE0NTI2MzA2MzQsImh0dHA6Ly9leGFtcGxlLmqvbS9pc19yb290Ijp0cnrlLCJpc3MiOiIxXzVmeXdoazRfbWJvazhrc3drdzhvc2djZ2c4OHM4OHNzMGdnNHNjY3dnazBrOGNrMPNnIiwzcm9sZXMiOlsiQ29udGdudF9SZWFkZXIiXX0.I2z4Wb6M_Yb26ux - K6vMNrNcySxA1TvRYopXuhle6yI";
        const string refresh_token = "rRxC1nghia8RzJWKWwYMmzWpVcBgMCDY";
        const string username = "username";
        const string password = "password";

        [Fact(DisplayName = "Client Credentials Grant")]
        public async void ClientCredentialsGrant()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(HttpMethod.Post, "https://api.gfycat.com/v1/oauth/token")
                .WithJsonContent(
                    new
                    {
                        grant_type = "client_credentials",
                        client_id,
                        client_secret,
                    })
                .Respond(
                    new
                    {
                        token_type = "bearer",
                        scope = "",
                        expires_in = 3600,
                        access_token
                    });

            var config = new GfycatClientConfig(client_id, client_secret)
            {
                RestClient = new MockMessageClient(mockHttp)
            };
            var client = new GfycatClient(config);
            await client.AuthenticateAsync();

            Assert.Equal(access_token, client.AccessToken);
        }

        [Fact(DisplayName = "Password Grant")]
        public async void PasswordGrant()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(HttpMethod.Post, "https://api.gfycat.com/v1/oauth/token")
                .WithJsonContent(
                    new
                    {
                        grant_type = "password",
                        client_id,
                        client_secret,
                        username,
                        password
                    })
                .Respond(
                    new
                    {
                        token_type = "bearer",
                        refresh_token_expires_in = 5184000,
                        refresh_token,
                        scope = "",
                        resource_owner = username,
                        expires_in = 3600,
                        access_token
                    });

            mockHttp.When(HttpMethod.Get, "https://api.gfycat.com/v1/me")
                .WithHeaders("Authorization: Bearer " + access_token)
                .Respond(
                    new
                    {
                        userid = "",
                        username,
                        email = "username@email.com",
                        description = "description",
                        profileUrl = "https://github.com/username",
                        name = "username",
                        views = 9001,
                        uploadNotices = true,
                        emailVerified = true,
                        url = "https://gfycat.com/@username",
                        createDate = 0,
                        profileImageUrl = "http://cdn.edgecast.steamstatic.com/steamcommunity/public/images/avatars/c9/c9c3bbe7ffb21f9b0ae54c1d48c8a42257b7b621_full.jpg",
                        verified = true,
                        followers = 0,
                        following = 1,
                        geoWhitelist = new[] { "us" },
                        domainWhitelist = new[] { "reddit.com" },
                        associatedProviders = new[] { "facebook" },
                        iframeProfileImageVisible = true
                    });

            var config = new GfycatClientConfig(client_id, client_secret)
            {
                RestClient = new MockMessageClient(mockHttp)
            };
            var client = new GfycatClient(config);
            await client.AuthenticateAsync(username, password);

            Assert.Equal(access_token, client.AccessToken);
            Assert.Equal(refresh_token, client.RefreshToken);
            Assert.Equal(username, client.CurrentUser.Username);
        }

        [Fact(DisplayName = "Refreshing access tokens")]
        public async void RefreshAccessToken()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(HttpMethod.Post, "https://api.gfycat.com/v1/oauth/token")
                .WithJsonContent(
                    new
                    {
                        grant_type = "password",
                        client_id,
                        client_secret,
                        username,
                        password
                    })
                .Respond(
                    new
                    {
                        token_type = "bearer",
                        refresh_token_expires_in = 5184000,
                        refresh_token,
                        scope = "",
                        resource_owner = username,
                        expires_in = 3600,
                        access_token
                    });

            mockHttp.When(HttpMethod.Post, "https://api.gfycat.com/v1/oauth/token")
                .WithJsonContent(
                    new
                    {
                        grant_type = "refresh",
                        client_id,
                        client_secret,
                        refresh_token
                    })
                .Respond(
                    new
                    {
                        token_type = "bearer",
                        refresh_token_expires_in = 5184000,
                        refresh_token,
                        scope = "",
                        resource_owner = username,
                        expires_in = 3600,
                        access_token
                    });

            mockHttp.When(HttpMethod.Get, "https://api.gfycat.com/v1/me")
                .WithHeaders("Authorization: Bearer " + access_token)
                .Respond(
                    new
                    {
                        userid = "",
                        username,
                        email = "username@email.com",
                        description = "description",
                        profileUrl = "https://github.com/username",
                        name = "username",
                        views = 9001,
                        uploadNotices = true,
                        emailVerified = true,
                        url = "https://gfycat.com/@username",
                        createDate = 0,
                        profileImageUrl = "http://cdn.edgecast.steamstatic.com/steamcommunity/public/images/avatars/c9/c9c3bbe7ffb21f9b0ae54c1d48c8a42257b7b621_full.jpg",
                        verified = true,
                        followers = 0,
                        following = 1,
                        geoWhitelist = new[] { "us" },
                        domainWhitelist = new[] { "reddit.com" },
                        associatedProviders = new[] { "facebook" },
                        iframeProfileImageVisible = true
                    });

            var config = new GfycatClientConfig(client_id, client_secret)
            {
                RestClient = new MockMessageClient(mockHttp)
            };
            var client = new GfycatClient(config);
            await client.AuthenticateAsync(username, password);
            await client.RefreshTokenAsync();

            Assert.Equal(access_token, client.AccessToken);
            Assert.Equal(refresh_token, client.RefreshToken);
            Assert.Equal(username, client.CurrentUser.Username);
        }

        [Fact(DisplayName = "Refreshing access tokens to authenticate")]
        public async void AuthenticateWithRefresh()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(HttpMethod.Post, "https://api.gfycat.com/v1/oauth/token")
                .WithJsonContent(
                    new
                    {
                        grant_type = "refresh",
                        client_id,
                        client_secret,
                        refresh_token
                    })
                .Respond(
                    new
                    {
                        token_type = "bearer",
                        refresh_token_expires_in = 5184000,
                        refresh_token,
                        scope = "",
                        resource_owner = username,
                        expires_in = 3600,
                        access_token
                    });

            mockHttp.When(HttpMethod.Get, "https://api.gfycat.com/v1/me")
                .WithHeaders("Authorization: Bearer " + access_token)
                .Respond(
                    new
                    {
                        userid = "",
                        username,
                        email = "username@email.com",
                        description = "description",
                        profileUrl = "https://github.com/username",
                        name = "username",
                        views = 9001,
                        uploadNotices = true,
                        emailVerified = true,
                        url = "https://gfycat.com/@username",
                        createDate = 0,
                        profileImageUrl = "http://cdn.edgecast.steamstatic.com/steamcommunity/public/images/avatars/c9/c9c3bbe7ffb21f9b0ae54c1d48c8a42257b7b621_full.jpg",
                        verified = true,
                        followers = 0,
                        following = 1,
                        geoWhitelist = new[] { "us" },
                        domainWhitelist = new[] { "reddit.com" },
                        associatedProviders = new[] { "facebook" },
                        iframeProfileImageVisible = true
                    });

            var config = new GfycatClientConfig(client_id, client_secret)
            {
                RestClient = new MockMessageClient(mockHttp)
            };
            var client = new GfycatClient(config);

            Assert.True(await client.RefreshTokenAsync(refresh_token));
            Assert.Equal(access_token, client.AccessToken);
            Assert.Equal(refresh_token, client.RefreshToken);
            Assert.Equal(username, client.CurrentUser.Username);
        }
    }
}
