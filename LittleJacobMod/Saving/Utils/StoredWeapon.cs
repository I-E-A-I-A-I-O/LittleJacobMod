using System.Collections.Generic;
using GTA;
using GTA.Native;
using Newtonsoft.Json;

namespace LittleJacobMod.Saving.Utils
{
    internal class StoredWeapon
    {
        [JsonProperty("WeaponHash", Required = Required.Always)]
        public uint WeaponHash { get; set; }
        
        [JsonProperty("Ammo", Required = Required.Always)]
        public int Ammo { get; set; }
        
        [JsonProperty("Attachments")]
        public Dictionary<string, LittleJacobMod.Utils.Types.GroupedComponent> Attachments { get; set; }
        
        [JsonProperty("Camo")]
        public LittleJacobMod.Utils.Types.Component Camo { get; set; }

        [JsonProperty("Tint")] public int Tint { get; set; } = -1;

        [JsonProperty("CamoColor")] public int CamoColor { get; set; } = -1;

        [JsonConstructor]
        public StoredWeapon() { }
        
        
        /*public StoredWeapon (uint WeaponHash)
        {
            this.WeaponHash = WeaponHash;
        }*/

        public int GetTintIndex()
        {
            return Function.Call<int>(Hash.GET_PED_WEAPON_TINT_INDEX, Main.PPID, WeaponHash);
        }

        public bool HasComponent(uint componentHash)
        {
            return Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON_COMPONENT, Main.PPID, WeaponHash, componentHash);
        }

        public uint GetCamo()
        {
            if (Camo != null) return Camo.Hash;
            return (uint)WeaponComponentHash.Invalid;
        }
        
        public int GetCamoColor()
        {
            if (Camo == null) return 0;
            return Function.Call<int>(Hash._GET_PED_WEAPON_LIVERY_COLOR, Main.PPID, WeaponHash, Camo.Hash);
        }
    }
}
