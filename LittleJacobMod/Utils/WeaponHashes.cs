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
            WeaponHash.Nightstick
        };

        public readonly static List<WeaponHash> mgHashes = new List<WeaponHash>()
        {
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
            WeaponHash.HeavyShotgun,
            WeaponHash.PumpShotgun,
            WeaponHash.SawnOffShotgun,
        };

        public readonly static List<WeaponHash> heavyWeaponHashes = new List<WeaponHash>()
        {
            WeaponHash.GrenadeLauncher,
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
            WeaponHash.BZGas,
        };
    }
}
