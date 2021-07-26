namespace TrueLayer.DataProduct.Pokemon.Shared.DomainModel
{
    public class PokemonInfo
    {
        public Species Species { get; set; }
    }

    public class Species
    {
        public string Name { get; set; }

        public string Url { get; set; }
    }
}
