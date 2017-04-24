namespace Gfycat.Net.Tests.RestFakes
{
    public static class Resources
    {
        public const string ClientId = "fakeClientId";
        public const string ClientSecret = "fakeClientSecret";
        public const string ClientCredentialsRequest = "{\"grant_type\":\"client_credentials\",\"client_id\":\"" + ClientId + "\",\"client_secret\":\"" + ClientSecret + "\"}";
        public const string ClientCredentialsResponse = "{\"token_type\":\"bearer\",\"scope\":\"\",\"expires_in\":3600,\"access_token\":\"" + AccessToken + "\"}";
        public const string StandardAuthenticationResponse = "{\"token_type\":\"bearer\",\"scope\":\"\",\"expires_in\":3600,\"access_token\":\"" + AccessToken + "\",\"refresh_token_expires_in\":\"5184000\",\"refresh_token\":\"" + RefreshToken + "\"}";
        public const string AccessToken = "eyJleHAiOjE0NzA4NTgxNDEsImlzcyI6IjJfWXVVVnZWIiwicm9sZXMiOlsiVXNlciJdLCJzY29wZXMiOiIiLCJzdWIiOiJ1c2VyL2tlbm5ldGhqZXJlbXlhdSJ9.0lR6MW9bFcbRiL3RN-U_xHkOS4S9D2JZB1QuCGab2zJ";
        public const string RefreshToken = "rRxC1nghia8RzJWKWwYMmzWpVcBgMCDY";
        public const string ValidUserName = "validUser";
        public const string InvalidUserName = "invalidUser";
        public const string ValidUserInfo = "{\"userid\":\"validUser\",\"description\":\"Testing Gfycat.Net\",\"profileUrl\":\"https:\\/\\/github.com\\/ObsidianMinor\\/Gfycat.Net\",\"profileImageUrl\":\"https:\\/\\/profiles.gfycat.com\\/9c907b40aa37fbc7e8655bfe80a75d11eb0286e78f5a3cababf93c32a89b723b.png\",\"name\":\"validUser\",\"username\":\"validUser\",\"views\":\"546\",\"verified\":false,\"iframeProfileImageVisible\":false,\"followers\":\"1\",\"following\":\"0\",\"publishedGfycats\":\"15\",\"publishedAlbums\":\"1\",\"createDate\":\"1486347089\",\"url\":\"https:\\/\\/gfycat.com\\/@obsidianminor\"}";
    }
}
