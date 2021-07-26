using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TrueLayer.DataProduct.Pokemon.Shared.Contract.Manager;
using TrueLayer.DataProduct.Pokemon.Shared.DomainModel;
using TrueLayer.DataProduct.Pokemon.Shared.Infra;

namespace TrueLayer.DataProduct.Pokemon.Manager.Manager
{
    /// <summary>
    /// Pokemon Manager service <see cref="IPokemonManager"/>
    /// </summary>
    public class PokemonManager: IPokemonManager
    {
        private readonly ILogger<PokemonManager> _logger;

        private readonly IHttpClientFactory _httpClientFactory;

        private ApiConfiguration _apiConfiguration { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PokemonManager"/> services.
        /// </summary>
        /// <param name="logger">DI service for logger</param>
        /// <param name="httpClientFactory">DI service for Http ClientFactory</param>
        /// <param name="apiConfiguration">DI bind for api configuration values</param>
        public PokemonManager(ILogger<PokemonManager> logger, IHttpClientFactory httpClientFactory , IOptions<ApiConfiguration> apiConfiguration)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _apiConfiguration = apiConfiguration.Value;
        }

        /// <summary>
        /// Get the pokemon basic information.
        /// </summary>
        /// <param name="pokemonName">pokemon name.</param>
        /// <param name="cancellationToken">cancellationToken </param>
        /// <returns></returns>
        public async Task<PokemonResponse> GetPokemonInformation(string pokemonName , CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Start GetPokemonInformation pokemonName:{pokemonName}");
                var url = $"/api/v2/pokemon/{pokemonName}";
                var client = _httpClientFactory.CreateClient("pokeapi");
                string result = await client.GetStringAsync(url , cancellationToken);
                var pokemonInfo = JsonConvert.DeserializeObject<PokemonInfo>(result);
                if (pokemonInfo!=null && pokemonInfo.Species != null && !string.IsNullOrEmpty(pokemonInfo.Species.Url))
                {
                    _logger.LogInformation($"Call GetPokemonSpecies");
                    return await GetPokemonSpecies(pokemonInfo.Species , cancellationToken);
                }
                return null;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"Error GetPokemonInformation Error message:{ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error GetPokemonInformation Error message:{ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get the pokemon translated infomation
        /// </summary>
        /// <param name="pokemonName">pokemon name.</param>
        /// <param name="cancellationToken">cancellationToken </param>
        /// <returns></returns>
        public async Task<PokemonResponse> GetTranslatedPokemon(string pokemonName , CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Start GetTranslatedPokemon pokemonName:{pokemonName}");
                var pokemonInfo = await GetPokemonInformation(pokemonName , cancellationToken);
                if (pokemonInfo!=null)
                {
                    _logger.LogInformation($"GetTranslatedPokemon Habitat:{pokemonInfo.Habitat} ,Legendary:{pokemonInfo.IsLegendary}");
                    if (pokemonInfo.Habitat.Equals("cave") || pokemonInfo.IsLegendary)
                    {
                        pokemonInfo.Description = await GetPokemonTranslation(pokemonInfo.Description , "yoda" , cancellationToken);
                    }
                    else
                    {
                        pokemonInfo.Description = await GetPokemonTranslation(pokemonInfo.Description, "shakespeare" , cancellationToken);
                    }
                }
                return pokemonInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error GetTranslatedPokemon Error message:{ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Translation for given description base on the traslateType.
        /// </summary>
        /// <param name="description">pokemon basic info description.</param>
        /// <param name="translateType">pokemon traslateType ,Eg :yoda ,shakespeare</param>
        /// <param name="cancellationToken">cancellationToken </param>
        /// <returns></returns>
        public async Task<string> GetPokemonTranslation(string description , string translateType , CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Start GetPokemonTranslation description:{description} ,translateType:{translateType}");

                var url = $"/translate/{translateType}.json";
                var translateRequest = new TranslationRequest { Text = description };
                HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(translateRequest), 
                    Encoding.UTF8, "application/json");

                var client = _httpClientFactory.CreateClient("translateapi");
                var result = await client.PostAsync(url, httpContent , cancellationToken);
                var responseBody = await result.Content.ReadAsStringAsync();

                var jodaResponse = JObject.Parse(responseBody);
                if (jodaResponse.SelectToken("success") != null && jodaResponse.SelectToken("success").HasValues)
                {
                    var translatedValue = jodaResponse.SelectToken("contents.translated").Value<string>();
                    _logger.LogInformation($"GetPokemonTranslation translated value :{translatedValue}");
                    return translatedValue;
                }
                else if (jodaResponse.SelectToken("error") != null && jodaResponse.SelectToken("error").HasValues)
                {
                    var errorCode = jodaResponse.SelectToken("error.code").Value<string>();
                    var errorMessage = jodaResponse.SelectToken("error.message").Value<string>();
                    _logger.LogInformation($"GetPokemonTranslation translated errorCode :{errorCode} , errorMessage:{errorMessage}");
                }

                return description;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"Error GetPokemonTranslation Error message:{ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error GetPokemonTranslation Error message:{ex.Message}");
                throw;
            }

        }

        /// <summary>
        /// Get the Pokemon Species base on the url.
        /// </summary>
        /// <param name="species">pokemon species object.</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns></returns>
        public async Task<PokemonResponse> GetPokemonSpecies(Species species, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Start GetPokemonSpecies species:{JsonConvert.SerializeObject(species)}");
                var client = _httpClientFactory.CreateClient("pokeapi");
                string result = await client.GetStringAsync(species.Url , cancellationToken);
                var speciesInfo = JsonConvert.DeserializeObject<SpeciesInfo>(result);

                if (speciesInfo!=null)
                {
                    var flavorTextEntity= speciesInfo.FlavorTextEntries.Any() ? 
                        speciesInfo.FlavorTextEntries.Where(x => x.Language.Name == _apiConfiguration.TranslateLanguage.ToString()).FirstOrDefault() : null; 
                    return new PokemonResponse
                    {
                        Name= species.Name,
                        Description = flavorTextEntity!=null ? flavorTextEntity.FlavorText :string.Empty,
                        Habitat = speciesInfo.Habitat.Name,
                        IsLegendary= speciesInfo.IsLegendary
                    };
                }
                return null;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"Error GetPokemonSpecies Error message:{ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error GetPokemonSpecies Error message:{ex.Message}");
                throw;
            }
        }
    }
}
