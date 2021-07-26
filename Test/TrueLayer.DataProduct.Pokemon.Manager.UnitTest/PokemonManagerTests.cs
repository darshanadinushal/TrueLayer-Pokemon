using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TrueLayer.DataProduct.Pokemon.Manager.Manager;
using TrueLayer.DataProduct.Pokemon.Shared.DomainModel;
using TrueLayer.DataProduct.Pokemon.Shared.Infra;
using Xunit;

namespace TrueLayer.DataProduct.Pokemon.Manager.UnitTest
{
    public class PokemonManagerTests
    {
        [Fact]
        public async Task GetPokemonTranslation_ForSuccessAsync()
        {
            var originalDescription = "It was created by\na scientist after\nyears of horrific\fgene splicing and\nDNA engineering\nexperiments.";
            var translateValue = "Created by a scientist after years of horrific gene splicing and dna engineering experiments,  it was.";
            var mockFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                 {
                     StatusCode = HttpStatusCode.OK,
                     Content = new StringContent(@"{
                                'success': {
                                    'total': 1
                                },
                                'contents': {
                                    'translated': 'Created by a scientist after years of horrific gene splicing and dna engineering experiments,  it was.',
                                    'text': 'It was created by\na scientist after\nyears of horrific\fgene splicing and\nDNA engineering\nexperiments.',
                                    'translation': 'yoda'
                                }}"),
                 });

            var client = new HttpClient(mockHttpMessageHandler.Object);
            client.BaseAddress = new Uri("https://api.funtranslations.com/");
            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var pokemonManager = new PokemonManager(new NullLogger<PokemonManager>(), mockFactory.Object,
                Options.Create(GetApiConfiguration()));

            var result = await pokemonManager.GetPokemonTranslation(originalDescription, "yoda", new CancellationToken());
            Assert.NotNull(result);
            Assert.Equal(result.ToString(), translateValue);
        }


        [Fact]
        public async Task GetPokemonInfo_ForSuccessAsync()
        {
            var originalDescription = "It was created by\na scientist after\nyears of horrific\fgene splicing and\nDNA engineering\nexperiments.";
            var mockFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(@"{
                                'name':'mewtwo',
                                'is_legendary':'true',
                                'habitat': {
                                    'name': 'rare',
                                    'url': 'https://pokeapi.co/api/v2/pokemon-habitat/5/'},
                                'flavor_text_entries': [
                                    {
                                      'flavor_text': 'It was created by\na scientist after\nyears of horrific\fgene splicing and\nDNA engineering\nexperiments.',
                                      'language': {
                                                        'name': 'en'
                                                    }
                                        }
                                ]
                            }"),
                });

            var client = new HttpClient(mockHttpMessageHandler.Object);
            client.BaseAddress = new Uri("https://pokeapi.co/");
            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var pokemonManager = new PokemonManager(new NullLogger<PokemonManager>(), mockFactory.Object,
                Options.Create(GetApiConfiguration()));
            var species = new Species
            {
                Name = "mewtwo",
                Url = "https://pokeapi.co/api/v2/pokemon-species/150/"
            };

            var result = await pokemonManager.GetPokemonSpecies(species, new CancellationToken());
            Assert.NotNull(result);
            Assert.Equal(result.Name, species.Name);
            Assert.Equal(result.Habitat, "rare");
            Assert.Equal(result.Description, originalDescription);
            Assert.True(result.IsLegendary);
        }


        [Fact]
        public async Task GetPokemonInfo_ForDiffrentLanguageAsync()
        {
            var originalDescription = "It was created by\na scientist after\nyears of horrific\fgene splicing and\nDNA engineering\nexperiments.";
            var mockFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(@"{
                                'name':'mewtwo',
                                'is_legendary':'true',
                                'habitat': {
                                    'name': 'rare',
                                    'url': 'https://pokeapi.co/api/v2/pokemon-habitat/5/'},
                                'flavor_text_entries': [
                                    {
                                          'flavor_text': 'ミュウの 遺伝子と ほとんど\n同じ。だが 大きさも 性格も\n恐ろしいほど 違っている。',
                                          'language': {
                                            'name': 'ja',
                                            'url': 'https://pokeapi.co/api/v2/language/11/'
                                          },
                                                        'version': {
                                                    'name': 'lets-go-pikachu',
                                            'url': 'https://pokeapi.co/api/v2/version/31/'
                                                        }
                                            },
                                    {
                                      'flavor_text': 'It was created by\na scientist after\nyears of horrific\fgene splicing and\nDNA engineering\nexperiments.',
                                      'language': {
                                                        'name': 'en',
                                                        'url': 'https://pokeapi.co/api/v2/language/9/'
                                                    },
                                                    'version': {
                                                'name': 'red',
                                        'url': 'https://pokeapi.co/api/v2/version/1/'
                                                    }
                                        },
                                {
                                      'flavor_text': 'Un Pokémon conçu en réorganisant\nles gènes de Mew. On raconte qu’il\ns’agit du Pokémon le plus féroce.',
                                      'language': {
                                        'name': 'fr',
                                        'url': 'https://pokeapi.co/api/v2/language/5/'
                                      },
                                      'version': {
                                        'name': 'black',
                                        'url': 'https://pokeapi.co/api/v2/version/17/'
                                      }
                                    },
                                 {
                                      'flavor_text': 'ひとりの　かがくしゃが　なんねんも\nおそろしい　いでんし　けんきゅうを\nつづけた　けっか　たんじょうした。',
                                      'language': {
                                        'name': 'ja-Hrkt',
                                        'url': 'https://pokeapi.co/api/v2/language/1/'
                                      },
                                      'version': {
                                        'name': 'x',
                                        'url': 'https://pokeapi.co/api/v2/version/23/'
                                      }
                                    }
                                ]
                            }"),
                });

            var client = new HttpClient(mockHttpMessageHandler.Object);
            client.BaseAddress = new Uri("https://pokeapi.co/");
            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var pokemonManager = new PokemonManager(new NullLogger<PokemonManager>(), mockFactory.Object,
                Options.Create(GetApiConfiguration()));
            var species = new Species
            {
                Name = "mewtwo",
                Url = "https://pokeapi.co/api/v2/pokemon-species/150/"
            };

            var result = await pokemonManager.GetPokemonSpecies(species, new CancellationToken());
            Assert.NotNull(result);
            Assert.Equal(result.Name, species.Name);
            Assert.Equal(result.Description, originalDescription);

        }

        private ApiConfiguration GetApiConfiguration()
        {
            return new ApiConfiguration
            {
                PokeapiEndPoint = "https://pokeapi.co/",
                TranslateapiEndPoint = "https://api.funtranslations.com/",
                TranslateLanguage = "en",
                HeaderAccept = "application/json",
                UserAgent= "TrueLayerDataProduct-Pokemon"
            };
        }

    }
}
