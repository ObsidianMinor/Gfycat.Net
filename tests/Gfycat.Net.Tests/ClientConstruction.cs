using Xunit;

namespace Gfycat.Net.Tests
{
    public class ClientConstruction
    {
        const string clientId = "fakeClientId";
        const string clientSecret = "fakeClientSecret";

        [Fact(DisplayName = "Client info ctor")]
        public void CreateClientWithIdSecretCtor()
        {
            GfycatClient client = new GfycatClient(clientId, clientSecret);

            Assert.Equal(clientId, client.ClientId);
            Assert.Equal(clientSecret, client.ClientSecret);
        }

        [Fact(DisplayName = "Client config ctor")]
        public void CreateClientWithConfigCtor()
        {
            GfycatClientConfig clientConfig = new GfycatClientConfig(clientId, clientSecret);
            GfycatClient client = new GfycatClient(clientConfig);

            Assert.Equal(clientId, client.ClientId);
            Assert.Equal(clientSecret, client.ClientSecret);
        }

        [Fact(DisplayName = "Verify default config values")]
        public void CreateClientWithDefaultConfig()
        {
            GfycatClientConfig clientConfig = new GfycatClientConfig(clientId, clientSecret);
            GfycatClient client = new GfycatClient(clientConfig);

            Assert.Equal(client.ApiClient.Config.DefaultRetryMode, RetryMode.RetryFirst401);
            Assert.Equal(client.ApiClient.Config.DefaultTimeout, -1);
            Assert.Equal(client.ApiClient.Config.RestClient.GetType(), typeof(Rest.DefaultRestClient));
        }

        [Fact(DisplayName = "Verify modified config values")]
        public void CreateClientWithModifiedConfig()
        {
            GfycatClientConfig clientConfig = new GfycatClientConfig(clientId, clientSecret)
            {
                DefaultRetryMode = RetryMode.AlwaysRetry,
                DefaultTimeout = 3000,
                RestClient = new MockRestClient()
            };
            GfycatClient client = new GfycatClient(clientConfig);

            Assert.Equal(client.ApiClient.Config.DefaultRetryMode, RetryMode.AlwaysRetry);
            Assert.Equal(client.ApiClient.Config.DefaultTimeout, 3000);
            Assert.Equal(client.ApiClient.Config.RestClient.GetType(), typeof(MockRestClient));
        }
    }
}
