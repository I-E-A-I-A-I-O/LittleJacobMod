using System;
using System.Collections.Generic;
using GTA;

namespace LittleJacobMod.Saving.Utils
{
    internal class StoredWeapon
    {
        public WeaponHash WeaponHash { get; }
        public int Ammo { get; set; } = 0;
        public WeaponComponentHash Muzzle { get; set; } = WeaponComponentHash.Invalid;
        public WeaponComponentHash Camo { get; set; } = WeaponComponentHash.Invalid;
        public int Tint { get; set; } = -1;
        public int CamoColor { get; set; } = -1;
        public WeaponComponentHash Grip { get; set; } = WeaponComponentHash.Invalid;
        public WeaponComponentHash Clip { get; set; } = WeaponComponentHash.Invalid;
        public WeaponComponentHash Barrel { get; set; } = WeaponComponentHash.Invalid;
        public WeaponComponentHash Scope { get; set; } = WeaponComponentHash.Invalid;
        public WeaponComponentHash Flashlight { get; set; } = WeaponComponentHash.Invalid;

        public StoredWeapon (WeaponHash hash)
        {
            WeaponHash = hash;
        }
    }
}
