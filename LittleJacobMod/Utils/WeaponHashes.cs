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

        public readonly static List<WeaponHash> pistolHashes = new List<WeaponHash>()
        {
            WeaponHash.APPistol,
            WeaponHash.CombatPistol,
            WeaponHash.HeavyPistol,
            WeaponHash.MachinePistol,
            WeaponHash.MarksmanPistol,
            WeaponHash.PericoPistol,
            WeaponHash.SNSPistol,
            WeaponHash.SNSPistolMk2,
            WeaponHash.VintagePistol,
            WeaponHash.Revolver,
            WeaponHash.DoubleActionRevolver,
            WeaponHash.NavyRevolver,
        };

        public readonly static List<WeaponHash> mgHashes = new List<WeaponHash>()
        {
            WeaponHash.SMG,
            WeaponHash.SMGMk2,
            WeaponHash.AssaultSMG,
            WeaponHash.MicroSMG,
            WeaponHash.MiniSMG,
            WeaponHash.MG,
            WeaponHash.CombatMG,
            WeaponHash.CombatMGMk2,
            WeaponHash.CombatPDW,
            WeaponHash.UnholyHellbringer
        };

        public readonly static List<WeaponHash> shotgunHashes = new List<WeaponHash>()
        {
            WeaponHash.AssaultShotgun,
            WeaponHash.BullpupShotgun,
            WeaponHash.CombatShotgun,
            WeaponHash.DoubleBarrelShotgun,
            WeaponHash.HeavyShotgun,
            WeaponHash.PumpShotgun,
            WeaponHash.PumpShotgunMk2,
            WeaponHash.SawnOffShotgun,
            WeaponHash.SweeperShotgun
        };

        public readonly static List<WeaponHash> heavyWeaponHashes = new List<WeaponHash>()
        {
            WeaponHash.RPG,
            WeaponHash.Minigun,
            WeaponHash.GrenadeLauncher,
            WeaponHash.CompactGrenadeLauncher,
            WeaponHash.Firework,
            WeaponHash.Railgun,
            WeaponHash.Widowmaker
        };

        public readonly static List<WeaponHash> rifleHashes = new List<WeaponHash>()
        {
            WeaponHash.AdvancedRifle,
            WeaponHash.AssaultRifle,
            WeaponHash.BullpupRifle,
            WeaponHash.BullpupRifleMk2,
            WeaponHash.CompactRifle,
            WeaponHash.MilitaryRifle,
            WeaponHash.AssaultrifleMk2
        };

        public readonly static List<WeaponHash> sniperHashes = new List<WeaponHash>()
        {
            WeaponHash.HeavySniper,
            WeaponHash.SniperRifle,
            WeaponHash.MarksmanRifle,
            WeaponHash.MarksmanRifleMk2
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
