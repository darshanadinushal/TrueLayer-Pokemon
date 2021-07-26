using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrueLayer.DataProduct.Pokemon.Shared.DomainModel
{
    public class SpeciesInfo
    {
        [JsonProperty("flavor_text_entries")]
        public List<FlavorTextEntries> FlavorTextEntries { get; set; }

        [JsonProperty("is_legendary")]
        public bool IsLegendary { get; set; }

        public Habitat Habitat { get; set; }
    }
}
