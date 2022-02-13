using System;
using System.Collections.Generic;
using GTA;
using GTA.Native;

namespace LittleJacobMod.Saving.Utils
{
    internal class StoredWeapon
    {
        public uint WeaponHash { get; }
        public int Ammo { get; set; }
        public Dictionary<string, uint> Components = new();
        public int Tint { get; set; }
        public int CamoColor { get; set; }

        public StoredWeapon (uint hash)
        {
            WeaponHash = hash;
        }

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
            if (!Components.ContainsKey("Liveries")) return (uint) WeaponComponentHash.Invalid;
            
            return Components["Liveries"];
        }
        
        public int GetCamoColor()
        {
            if (!Components.ContainsKey("Liveries")) return 0;
            
            uint camo = Components["Liveries"];
            return Function.Call<int>(Hash._GET_PED_WEAPON_LIVERY_COLOR, Main.PPID, WeaponHash, camo);
        }
    }
}
