using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrueLayer.DataProduct.Pokemon.Shared.Infra
{
    public class ApiConfiguration
    {
        public string PokeapiEndPoint { get; set; }

        public string TranslateapiEndPoint { get; set; }

        public string UserAgent { get; set; }

        public string HeaderAccept { get; set; }

        public string TranslateLanguage { get; set; }
    }
}
