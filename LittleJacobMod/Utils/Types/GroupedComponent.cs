using Newtonsoft.Json;

namespace LittleJacobMod.Utils.Types
{
    internal class GroupedComponent
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Price")]
        public int Price { get; set; }

        [JsonProperty("Hash")]
        public uint Hash { get; set; }

        [JsonProperty("Group")]
        public string Group { get; set; }
    }
}
