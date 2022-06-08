using Newtonsoft.Json;

namespace LittleJacobMod.Utils.Types
{
    internal class Color
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Price")]
        public int Price { get; set; }
    }
}
