using System;
using System.Collections.Generic;
using GTA;

namespace LittleJacobMod.Utils.Weapons
{
    internal abstract class Weapon
    {
        public abstract bool SaveFileWeapon { get; }
        public abstract WeaponHash WeaponHash { get; }
        public abstract int Price { get; }
        public abstract string Name { get; }
        public abstract bool HasMuzzleOrSupp { get; }
        public abstract bool HasClip { get; }
        public abstract bool HasBarrel { get; }
        public abstract bool HasGrip { get; }
        public abstract bool HasScope { get; }
        public abstract bool HasCamo { get; }
        public abstract bool HasFlaslight { get; }
        public abstract Dictionary<string, WeaponComponentHash> MuzzlesAndSupps { get; }
        public abstract Dictionary<string, WeaponComponentHash> Clips { get; }
        public abstract Dictionary<string, WeaponComponentHash> Barrels { get; }
        public abstract Dictionary<string, WeaponComponentHash> Grips { get; }
        public abstract Dictionary<string, WeaponComponentHash> Scopes { get; }
        public abstract Dictionary<string, WeaponComponentHash> Camos { get; }
        public abstract Dictionary<string, WeaponComponentHash> FlashLight { get; }

        public static List<WeaponTint> WeaponTints => new List<WeaponTint>()
        {
            WeaponTint.Normal,
            WeaponTint.Gold,
            WeaponTint.Platinum,
            WeaponTint.LSPD,
            WeaponTint.Army,
            WeaponTint.Green,
            WeaponTint.Pink,
            WeaponTint.Orange
        };
    }
}
