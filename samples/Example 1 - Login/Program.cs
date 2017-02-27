using System;
using Gfycat;
using System.Threading.Tasks;

namespace LoginSample
{
    class Program
    {
        const string _clientId = "REPLACE_WITH_YOUR_CLIENT_ID";
        const string _clientSecret = "REPLACE_WITH_YOUR_CLIENT_SECRET";

        static void Main(string[] args) => new Program().SimpleClientAuthenticationAsync().GetAwaiter().GetResult();

        async Task SimpleClientAuthenticationAsync()
        {
            /*
             * Welcome to the first example of using Gfycat.Net, 
             * this shows how to use the authentication methods
             * you'll need to authenticate before you can use any endpoints.
             */

            GfycatClient client = new GfycatClient(_clientId, _clientSecret);

            /* 
             * Authentication methods are found in the Authentication property in the client.
             * The simplest client authentication method will let you access all public endpoints, 
             * you can't access user endpoints with this authentication method
             */

            await client.Authentication.AuthenticateClientAsync(); // used to perform a simple client authentication

            /*
             * Authentication methods can be called multiple times.
             * You can run the simple authentication method first and then if the user logs in you can run another authentication method later.
             * 
             * The Authentication property has two events regarding tokens.
             * Authentication.AccessTokenExpired fires when the authentication token expires,
             * when this event fires, the Authentication container with attempt to refresh the token if possible 
             * (auto refresh is not possible for implicit browser auth)
             * If you want to manually refresh the token, you can run Authentication.RefreshTokenAsync()
             */
        }

        async Task PasswordAuthAsync()
        {
            GfycatClient client = new GfycatClient(_clientId, _clientSecret);

            // if you want to login to your client for testing purposes you can with your username and password
            await client.Authentication.AuthenticatePasswordAsync("USERNAME_OF_ACCOUNT_WHICH_OWNS_CLIENT", "PASSWORD_OF_ACCOUNT_WHICH_OWNS_CLIENT");
        }

        void ImplicitAuth()
        {
            GfycatClient client = new GfycatClient(_clientId, _clientSecret);

            string authUrl = client.Authentication.GetBrowserAuthUrl("randomState", "http://example.com/oauth/callback", false);

            // open web browser and navigate to authUrl
            // when the user authenticates, the browser is redirect back to the redirect uri with a query string with the following parameters
            // access_token, token_type, expires_in, scope, state
            // process the query string however you like, run Authenticate with the access_token and expires_in variables
            // if the user doesn't authenticate, you're navigated back using the redirect uri with an "error" parameter
            
            client.Authentication.Authenticate("accessToken", 60);
        }

        async Task AuthCodeAsync()
        {
            GfycatClient client = new GfycatClient(_clientId, _clientSecret);

            string authUrl = client.Authentication.GetBrowserAuthUrl("RANDOMstate", "http://example.com/oauth/callback", true);

            // open web browser and navigate to authUrl
            // when the user authenticates the browser is redirected back to the direct uri with the query string with the following parameters
            // code, state
            // process the query string however you like, run AuthenticateCodeAysnc using the given code
            // if the user doesn't authenticate, you're navigated back using the redirect uri with an "error" parameter

            await client.Authentication.AuthenticateCodeAsync("code", "http://example.com/oauth/callback");
        }

        // I'm sorry but I don't know how facebook works. You'll have to learn how to get an auth code yourself from facebook...
        async Task FacebookAuthAsync()
        {
            GfycatClient client = new GfycatClient(_clientId, _clientSecret);

            await client.Authentication.AuthenticateFacebookCodeAsync("authCode");
        }

        // I'm sorry but I don't know how facebook works. You'll have to learn how to get an auth code from facebook yourself
        async Task FacebookTokenAsync()
        {
            GfycatClient client = new GfycatClient(_clientId, _clientSecret);

            await client.Authentication.AuthenticateFacebookTokenAsync("token!");
        }

        async Task TwitterAsync()
        {
            GfycatClient client = new GfycatClient(_clientId, _clientSecret);

            string requestToken = await client.Authentication.GetTwitterRequestTokenAsync();
            string twitterUrl = $"https://api.twitter.com/oauth/authenticate?oauth_token={requestToken}";

            // navigate to twitter url, you will be redirected back with a verifier

            await client.Authentication.AuthenticateTwitterTokenAsync(requestToken, "verifier");
        }

        // if you're sure you have a valid refresh token, provide it here and the refresh loop can be redone
        async Task AuthenticateRefreshTokenAsync()
        {
            GfycatClient client = new GfycatClient(_clientId, _clientSecret);

            await client.Authentication.AuthenticateWithRefreshTokenAsync("REFRESH");
        }
    }
}