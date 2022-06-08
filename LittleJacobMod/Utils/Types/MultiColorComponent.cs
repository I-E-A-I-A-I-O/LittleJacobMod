using System.Collections.Generic;
using Newtonsoft.Json;

namespace LittleJacobMod.Utils.Types
{
    internal class MultiColorComponent
    {
        [JsonProperty("Colors")]
        public List<Color> Colors { get; set; }

        [JsonProperty("Styles")]
        public List<Component> Components { get; set; }
    }
}
