using System;
using Xunit;
using Gfycat.Net.Tests.RestFakes;
using static Gfycat.Net.Tests.RestFakes.Resources;

namespace Gfycat.Net.Tests
{
    [Trait("Category", "Client construction")]
    public class ClientConstructionTests
    {
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
            GfycatClientConfig clientConfig = new GfycatClientConfig(ClientId, ClientSecret);
            GfycatClient client = new GfycatClient(clientConfig);

            Assert.Equal(client.ApiClient.Config.DefaultRetryMode, RetryMode.RetryFirst401);
            Assert.Equal(client.ApiClient.Config.DefaultTimeout, -1);
            Assert.Equal(client.ApiClient.Config.RestClient.GetType(), typeof(Rest.DefaultRestClient));
        }

        [Fact(DisplayName = "Verify modified config values")]
        public void CreateClientWithModifiedConfig()
        {
            GfycatClientConfig clientConfig = new GfycatClientConfig(ClientId, ClientSecret)
            {
                DefaultRetryMode = RetryMode.AlwaysRetry,
                DefaultTimeout = 3000,
                RestClient = new MockRestClient(new Uri(GfycatClientConfig.BaseUrl))
            };
            GfycatClient client = new GfycatClient(clientConfig);

            Assert.Equal(client.ApiClient.Config.DefaultRetryMode, RetryMode.AlwaysRetry);
            Assert.Equal(client.ApiClient.Config.DefaultTimeout, 3000);
            Assert.Equal(client.ApiClient.Config.RestClient.GetType(), typeof(MockRestClient));
        }
    }
}
