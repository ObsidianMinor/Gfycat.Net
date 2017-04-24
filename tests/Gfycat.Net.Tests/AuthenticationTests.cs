using Gfycat.Net.Tests.RestFakes;
using Xunit;

namespace Gfycat.Net.Tests
{
    [Trait("Category", "Authentication")]
    public class AuthenticationTests
    {
        GfycatClient Client = Utils.MakeClient();

        [Fact(DisplayName = "Client Credentials Grant")]
        public async void ClientAuthAsync()
        {
            await Client.AuthenticateAsync();
            Assert.Equal(Resources.AccessToken, Client.AccessToken);
        }

        [Fact(DisplayName = "Password Grant")]
        public async void PasswordAuthAsync()
        {

        }

        [Fact(DisplayName = "Refreshing access tokens")]
        public async void RefreshingTokensAsync()
        {

        }

        // I bet you're expecting implicit flow here? Nah, implicit flow relies on the guess that you're a good user and are putting in a valid access token
    }
}
