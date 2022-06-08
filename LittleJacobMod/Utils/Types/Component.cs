using Newtonsoft.Json;

namespace LittleJacobMod.Utils.Types
{
    internal class Component
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Price")]
        public int Price { get; set; }

        [JsonProperty("Hash")]
        public uint Hash { get; set; }
    }
}
