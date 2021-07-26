using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrueLayer.DataProduct.Pokemon.Shared.DomainModel
{
    /// <summary>
    /// FlavorTextEntries
    /// </summary>
    public class FlavorTextEntries
    {
        [JsonProperty("flavor_text")]
        public string FlavorText { get; set; }

        [JsonProperty("language")]
        public Language Language { get; set; }
    }
}
