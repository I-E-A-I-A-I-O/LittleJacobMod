using GTA;
using GTA.Native;

namespace LittleJacobMod.Saving.Utils
{
    internal class StoredWeapon
    {
        public uint WeaponHash { get; }
        public int Ammo { get; set; }
        public uint Muzzle { get; set; } = (uint)WeaponComponentHash.Invalid;
        public uint Camo { get; set; } = (uint)WeaponComponentHash.Invalid;
        public int Tint { get; set; }
        public int CamoColor { get; set; }
        public uint Grip { get; set; } = (uint)WeaponComponentHash.Invalid;
        public uint Clip { get; set; } = (uint)WeaponComponentHash.Invalid;
        public uint Barrel { get; set; } = (uint)WeaponComponentHash.Invalid;
        public uint Scope { get; set; } = (uint)WeaponComponentHash.Invalid;
        public uint Flashlight { get; set; } = (uint)WeaponComponentHash.Invalid;
        public uint Varmod { get; set; } = (uint)WeaponComponentHash.Invalid;

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

        public int GetCamoColor()
        {
            return Function.Call<int>(Hash._GET_PED_WEAPON_LIVERY_COLOR, Main.PPID, WeaponHash, Camo);
        }
    }
}
