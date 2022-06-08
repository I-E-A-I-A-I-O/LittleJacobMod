using System.Collections.Generic;
using Newtonsoft.Json;

namespace LittleJacobMod.Utils.Types
{
    internal class Weapon
    {
        [JsonProperty("Name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty("Group", Required = Required.Always)]
        public string Group { get; set; }

        [JsonProperty("Price", Required = Required.Always)]
        public int Price { get; set; }

        [JsonProperty("Hash", Required = Required.Always)]
        public uint Hash { get; set; }

        [JsonProperty("Tints", Required = Required.Default)]
        public List<Color> Tints { get; set; }

        [JsonProperty("Camo", Required = Required.Default)]
        public MultiColorComponent CamoComponents { get; set; }

        [JsonProperty("Attachments", Required = Required.Default)]
        public List<GroupedComponent> Attachments { get; set; }
    }
}
