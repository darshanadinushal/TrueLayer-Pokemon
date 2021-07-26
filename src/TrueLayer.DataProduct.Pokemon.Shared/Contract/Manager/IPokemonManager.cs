using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TrueLayer.DataProduct.Pokemon.Shared.DomainModel;

namespace TrueLayer.DataProduct.Pokemon.Shared.Contract.Manager
{
    /// <summary>
    /// IPokemonManager
    /// </summary>
    public interface IPokemonManager
    {
        /// <summary>
        /// Get the pokemon basic information.
        /// </summary>
        /// <param name="pokemonName">pokemon name.</param>
        /// <param name="cancellationToken">cancellationToken </param>
        /// <returns></returns>
        Task<PokemonResponse> GetPokemonInformation(string pokemonName , CancellationToken cancellationToken);

        /// <summary>
        /// Get the pokemon translated infomation
        /// </summary>
        /// <param name="pokemonName">pokemon name.</param>
        /// <param name="cancellationToken">cancellationToken </param>
        /// <returns></returns>
        Task<PokemonResponse> GetTranslatedPokemon(string pokemonName, CancellationToken cancellationToken);
    }
}
