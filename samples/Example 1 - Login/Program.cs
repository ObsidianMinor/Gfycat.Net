using System;
using Gfycat;
using System.Threading.Tasks;

namespace LoginSample
{
    class Program
    {
        static void Main(string[] args) => new Program().SimpleClientAuthentication().GetAwaiter().GetResult();

        async Task SimpleClientAuthentication()
        {
            /*
             * Welcome to the first example of using Gfycat.Net, 
             * this shows Authentication which you'll need to do before you can use any endpoints.
             */

            GfycatClient client = new GfycatClient("REPLACE_WITH_YOUR_CLIENT_ID", "REPLACE_WITH_YOUR_CLIENT_SECRET");

            /* 
             * Authentication methods are found in the Authentication property in the client.
             * The simplest client authentication method will let you access all public endpoints, 
             * you can't access user endpoints with this authentication method
             */

            await client.Authentication.AuthenticateAsync(); // used to perform a simple client authentication

            /*
             * Authentication methods can be called multiple times.
             * You can run the simple authentication method first and then if the user chooses you can run another authentication method later.
             * 
             * The Authentication property has two events regarding tokens.
             * Authentication.AccessTokenExpired fires when the authentication token expires,
             * when this event fires, the Authentication container with attempt to refresh the token if possible
             * If you want to manually refresh the token, you can run Authentication.RefreshTokenAsync()
             */
        }
    }
}