using RichardSzalay.MockHttp;
using Xunit;

namespace Gfycat.Tests
{
    [Trait("Category", "Client construction")]
    public class ClientConstructionTests
    {
        const string ClientId = "clientId";
        const string ClientSecret = "clientSecret";

        [Fact(DisplayName = "Provided ID and secret are correct")]
        public void CreateClientWithIdSecretCtor()
        {
            GfycatClient client = new GfycatClient(ClientId, ClientSecret);

            Assert.Equal(ClientId, client.ClientId);
            Assert.Equal(ClientSecret, client.ClientSecret);
        }

        [Fact(DisplayName = "Provided ID and secret are correct config provided")]
        public void CreateClientWithConfigCtor()
        {
            GfycatClientConfig clientConfig = new GfycatClientConfig(ClientId, ClientSecret);
            GfycatClient client = new GfycatClient(clientConfig);

            Assert.Equal(ClientId, client.ClientId);
            Assert.Equal(ClientSecret, client.ClientSecret);
        }

        [Fact(DisplayName = "Verify default config values")]
        public void CreateClientWithDefaultConfig()
        {
            GfycatClientConfig clientConfig = new GfycatClientConfig(ClientId, ClientId);
            GfycatClient client = new GfycatClient(clientConfig);

            Assert.Equal(RetryMode.RetryFirst401, client.ApiClient.Config.DefaultRetryMode);
            Assert.Equal(-1, client.ApiClient.Config.DefaultTimeout);
            Assert.Equal(typeof(Rest.DefaultRestClient), client.ApiClient.Config.RestClient.GetType());
        }

        [Fact(DisplayName = "Verify modified config values")]
        public void CreateClientWithModifiedConfig()
        {
            GfycatClientConfig clientConfig = new GfycatClientConfig(ClientId, ClientSecret)
            {
                DefaultRetryMode = RetryMode.AlwaysRetry,
                DefaultTimeout = 3000,
                RestClient = new MockMessageClient(new MockHttpMessageHandler())
            };
            GfycatClient client = new GfycatClient(clientConfig);

            Assert.Equal(RetryMode.AlwaysRetry, client.ApiClient.Config.DefaultRetryMode);
            Assert.Equal(3000, client.ApiClient.Config.DefaultTimeout);
            Assert.Equal(typeof(MockMessageClient), client.ApiClient.Config.RestClient.GetType());
        }
    }
}
