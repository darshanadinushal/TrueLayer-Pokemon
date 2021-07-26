using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TrueLayer.DataProduct.Pokemon.Shared.Contract.Manager;
using TrueLayer.DataProduct.Pokemon.Shared.DomainModel;

namespace TrueLayer.DataProduct.Pokemon.RestService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PokemonController : ControllerBase
    {
        private readonly ILogger<PokemonController> _logger;

        private readonly IPokemonManager _pokemonManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="PokemonController"/> services.
        /// </summary>
        /// <param name="logger">DI Service logger</param>
        /// <param name="pokemonManager">DI Service PokemonManager</param>
        public PokemonController(ILogger<PokemonController> logger , IPokemonManager pokemonManager)
        {
            _logger = logger;
            _pokemonManager = pokemonManager;
        }

        [HttpGet]
        [Route("{pokemonName}")]
        public async Task<PokemonResponse> GetPokemonInfo(string pokemonName, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Start PokemonController pokemonName:{pokemonName}");

                if (string.IsNullOrEmpty(pokemonName))
                    throw new Exception("PokemonController ,pokemonName is null");
               
                return await _pokemonManager.GetPokemonInformation(pokemonName , cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"PokemonController Error message: {ex.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("translated/{pokemonName}")]
        public async Task<PokemonResponse> GetPokemonTranslated(string pokemonName, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Start GetPokemonTranslated pokemonName:{pokemonName}");

                if (string.IsNullOrEmpty(pokemonName))
                    throw new Exception("GetPokemonTranslated ,pokemonName is null");

                return await _pokemonManager.GetTranslatedPokemon(pokemonName, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"TranslatedController Error message: {ex.Message}");
                throw;
            }
        }



    }
}
