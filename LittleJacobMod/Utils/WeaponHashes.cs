using System;
using System.Collections.Generic;
using GTA;

namespace LittleJacobMod.Utils
{
    static internal class WeaponHashes
    {
        public readonly static List<WeaponHash> meeleHashes = new List<WeaponHash>()
        {
            WeaponHash.Knife,
            WeaponHash.KnuckleDuster,
            WeaponHash.Bat,
            WeaponHash.BattleAxe,
            WeaponHash.Dagger,
            WeaponHash.Crowbar,
            WeaponHash.Hatchet,
            WeaponHash.Snowball,
            WeaponHash.Nightstick
        };

        public readonly static List<WeaponHash> mgHashes = new List<WeaponHash>()
        {
            WeaponHash.SMG,
            WeaponHash.AssaultSMG,
            WeaponHash.MicroSMG,
            WeaponHash.MiniSMG,
            WeaponHash.MG,
            WeaponHash.CombatMG,
            WeaponHash.CombatPDW,
        };

        public readonly static List<WeaponHash> shotgunHashes = new List<WeaponHash>()
        {
            WeaponHash.AssaultShotgun,
            WeaponHash.BullpupShotgun,
            WeaponHash.DoubleBarrelShotgun,
            WeaponHash.HeavyShotgun,
            WeaponHash.PumpShotgun,
            WeaponHash.SawnOffShotgun,
            WeaponHash.SweeperShotgun
        };

        public readonly static List<WeaponHash> heavyWeaponHashes = new List<WeaponHash>()
        {
            WeaponHash.RPG,
            WeaponHash.Minigun,
            WeaponHash.GrenadeLauncher,
            WeaponHash.CompactGrenadeLauncher,
            WeaponHash.Railgun,
        };

        public readonly static List<WeaponHash> sniperHashes = new List<WeaponHash>()
        {
            WeaponHash.HeavySniper,
            WeaponHash.SniperRifle,
            WeaponHash.MarksmanRifle,
        };

        public readonly static List<WeaponHash> explosiveHashes = new List<WeaponHash>()
        {
            WeaponHash.Grenade,
            WeaponHash.BZGas,
            WeaponHash.StickyBomb,
            WeaponHash.ProximityMine
        };
    }
}
