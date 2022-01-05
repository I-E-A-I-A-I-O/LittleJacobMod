using System;
using System.Collections.Generic;
using GTA;

namespace LittleJacobMod.Utils.Weapons
{
    internal class Weapon
    {
        public bool SaveFileWeapon { get; }
        public uint WeaponHash { get; }
        public int Price { get; }
        public string Name { get; }
        public bool HasMuzzleOrSupp { get; }
        public bool HasClip { get; }
        public bool HasBarrel { get; }
        public bool HasGrip { get; }
        public bool HasScope { get; }
        public bool HasCamo { get; }
        public bool HasFlaslight { get; }
        public bool HasVarmod { get; }
        public Dictionary<string, uint> MuzzlesAndSupps { get; } = new Dictionary<string, uint>();
        public Dictionary<string, uint> Clips { get; } = new Dictionary<string, uint>();
        public Dictionary<string, uint> Barrels { get; } = new Dictionary<string, uint>();
        public Dictionary<string, uint> Grips { get; } = new Dictionary<string, uint>();
        public Dictionary<string, uint> Scopes { get; } = new Dictionary<string, uint>();
        public Dictionary<string, uint> Camos { get; } = new Dictionary<string, uint>();
        public Dictionary<string, uint> FlashLight { get; } = new Dictionary<string, uint>();

        public Weapon(bool sfWeapon, uint weaponHash, int price, string name, bool muzzle, bool clip,
            bool barrel, bool grip, bool scope, bool camo, bool flash, bool varmod)
        {
            SaveFileWeapon = sfWeapon;
            WeaponHash = weaponHash;
            Price = price;
            Name = name;
            HasMuzzleOrSupp = muzzle;
            HasClip = clip;
            HasBarrel = barrel;
            HasGrip = grip;
            HasScope = scope;
            HasCamo = camo;
            HasFlaslight = flash;
            HasVarmod = varmod;
        }
    }
}
