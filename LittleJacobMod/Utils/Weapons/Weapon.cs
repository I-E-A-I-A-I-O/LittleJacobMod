using System;
using System.Collections.Generic;
using GTA;

namespace LittleJacobMod.Utils.Weapons
{
    internal abstract class Weapon
    {
        public abstract bool SaveFileWeapon { get; }
        public abstract uint WeaponHash { get; }
        public abstract int Price { get; }
        public abstract string Name { get; }
        public abstract bool HasMuzzleOrSupp { get; }
        public abstract bool HasClip { get; }
        public abstract bool HasBarrel { get; }
        public abstract bool HasGrip { get; }
        public abstract bool HasScope { get; }
        public abstract bool HasCamo { get; }
        public abstract bool HasFlaslight { get; }
        public abstract Dictionary<string, uint> MuzzlesAndSupps { get; }
        public abstract Dictionary<string, uint> Clips { get; }
        public abstract Dictionary<string, uint> Barrels { get; }
        public abstract Dictionary<string, uint> Grips { get; }
        public abstract Dictionary<string, uint> Scopes { get; }
        public abstract Dictionary<string, uint> Camos { get; }
        public abstract Dictionary<string, uint> FlashLight { get; }

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
