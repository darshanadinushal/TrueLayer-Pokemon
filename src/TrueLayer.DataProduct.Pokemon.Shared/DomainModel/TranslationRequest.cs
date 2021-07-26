using Newtonsoft.Json;

namespace TrueLayer.DataProduct.Pokemon.Shared.DomainModel
{
    public class TranslationRequest
    {
        [JsonProperty("text")]
        public string Text { get; set; }
    }
}
