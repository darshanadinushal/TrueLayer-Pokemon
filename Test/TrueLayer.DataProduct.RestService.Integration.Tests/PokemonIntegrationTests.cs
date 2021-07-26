using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TrueLayer.DataProduct.Pokemon.RestService;
using TrueLayer.DataProduct.Pokemon.Shared.DomainModel;
using TrueLayer.DataProduct.RestService.Integration.Tests.TestFixtures;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

namespace TrueLayer.DataProduct.RestService.Integration.Tests
{
    [Collection("Integration test collection")]
    public class PokemonIntegrationTests
    {
        private readonly HttpClient client;

        private readonly WireMockServer wireMockServer;

        public PokemonIntegrationTests(IntegrationTestFixture<Startup> testFixture)
        {
            this.client = testFixture.CreateClient();
            this.wireMockServer = testFixture.WireMockServer;
            SetupWireMockResponse();
        }

        private void SetupWireMockResponse()
        {

            wireMockServer.Given(Request.Create()
                .UsingMethod("GET")
                .WithPath("Pokemon/"))
            .RespondWith(Response.Create()
                .WithBodyAsJson(new GenericResponse { })
                .WithStatusCode(HttpStatusCode.OK));

            wireMockServer.Given(Request.Create()
                .UsingMethod("GET")
                .WithPath("Pokemon/translated/"))
            .RespondWith(Response.Create()
                .WithBodyAsJson(new GenericResponse { })
                .WithStatusCode(HttpStatusCode.OK));

        }

        [Fact]
        public async Task PokemonInfoTest_Valid_200OkAsync()
        {
            var response = await client.GetAsync("Pokemon/mewtwo");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            var messageResponse = JsonConvert.DeserializeObject<PokemonResponse>(responseBody);
            Assert.IsType<PokemonResponse>(messageResponse);
            Assert.Equal(messageResponse.Name , "mewtwo");
            Assert.True(messageResponse.IsLegendary);
        }

        [Fact]
        public async Task PokemonInfoTest_IncorrectValue()
        {
            await Assert.ThrowsAsync<HttpRequestException>(() => client.GetAsync("Pokemon/test"));
        }

        [Fact]
        public async Task PokemonTranslatedTest_200OkAsync()
        {
            var traslateDes = "Created by a scientist after years of horrific gene splicing and dna engineering experiments,  it was.";
            var response = await client.GetAsync("Pokemon/translated/mewtwo");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            var messageResponse = JsonConvert.DeserializeObject<PokemonResponse>(responseBody);
            Assert.IsType<PokemonResponse>(messageResponse);
            Assert.Equal(messageResponse.Name, "mewtwo");
            Assert.Equal(messageResponse.Description, traslateDes);
            Assert.True(messageResponse.IsLegendary);
        }


        [Fact]
        public async Task PokemonTranslatedComparePokemonInfoTest_200OkAsync()
        {
            var response = await client.GetAsync("Pokemon/mewtwo");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            var messageResponse = JsonConvert.DeserializeObject<PokemonResponse>(responseBody);
            Assert.IsType<PokemonResponse>(messageResponse);

            var translateResponse = await client.GetAsync("Pokemon/translated/mewtwo");
            Assert.Equal(HttpStatusCode.OK, translateResponse.StatusCode);
            var translateResponseBody = await translateResponse.Content.ReadAsStringAsync();
            var translateMessageResponse = JsonConvert.DeserializeObject<PokemonResponse>(translateResponseBody);

            Assert.Equal(messageResponse.Name, translateMessageResponse.Name);
            Assert.Equal(messageResponse.Habitat, translateMessageResponse.Habitat);
            Assert.Equal(messageResponse.IsLegendary , translateMessageResponse.IsLegendary);
            Assert.NotEqual(messageResponse.Description, translateMessageResponse.Description);
        }


    }
}
