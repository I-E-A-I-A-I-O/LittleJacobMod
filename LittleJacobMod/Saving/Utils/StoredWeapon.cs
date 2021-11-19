using System;
using GTA;
using GTA.Native;

namespace LittleJacobMod.Saving.Utils
{
    internal class StoredWeapon
    {
        public WeaponHash WeaponHash { get; }
        public int Ammo { get; set; } = 0;
        public WeaponComponentHash Muzzle { get; set; } = WeaponComponentHash.Invalid;
        public WeaponComponentHash Camo { get; set; } = WeaponComponentHash.Invalid;
        public int Tint { get; set; } = 0;
        public int CamoColor { get; set; } = 0;
        public WeaponComponentHash Grip { get; set; } = WeaponComponentHash.Invalid;
        public WeaponComponentHash Clip { get; set; } = WeaponComponentHash.Invalid;
        public WeaponComponentHash Barrel { get; set; } = WeaponComponentHash.Invalid;
        public WeaponComponentHash Scope { get; set; } = WeaponComponentHash.Invalid;
        public WeaponComponentHash Flashlight { get; set; } = WeaponComponentHash.Invalid;

        public StoredWeapon (WeaponHash hash)
        {
            WeaponHash = hash;
        }

        public int GetTintIndex()
        {
            return Function.Call<int>(Hash.GET_PED_WEAPON_TINT_INDEX, Game.Player.Character.Handle, WeaponHash);
        }

        public bool HasComponent(WeaponComponentHash componentHash)
        {
            return Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON_COMPONENT, Game.Player.Character.Handle, WeaponHash, componentHash);
        }

        public int GetCamoColor()
        {
            return Function.Call<int>(Hash._GET_PED_WEAPON_LIVERY_COLOR, Game.Player.Character.Handle, WeaponHash, Camo);
        }
    }
}
