using System;
using System.Collections.Generic;

namespace LittleJacobMod.Utils.Weapons
{
    internal static class WeaponsList
    {
        public static List<Weapon> Melee => new List<Weapon>()
        {
            new Bat(),
            new BattleAxe(),
            new Bottle(),
            new Crowbar(),
            new Dagger(),
            new Flashlight(),
            new Golfclub(),
            new Hammer(),
            new Hatchet(),
            new Knife(),
            new KnuckleDuster(),
            new Nightstick(),
            new StoneHatchet(),
            new Switchblade(),
            new Wrench()
        };

        public static List<Weapon> Pistols => new List<Weapon>()
        {
            new Pistol(),
            new Pistol50(),
            new PistolMk2(),
            new UpNAtomizer(),
            new RevolverMk2(),
            new ApPistol(),
            new CombatPistol(),
            new HeavyPistol(),
            new MarksmanPistol(),
            new PericoPistol(),
            new SNSPistol(),
            new SNSPistolMk2(),
            new HeavyRevolver(),
            new DoubleActionRevolver(),
            new NavyRevolver(),
            new StunGun(),
            new FlareGun(),
            new MachinePistol(),
            new CeramicPistol(),
            new VintagePistol()
        };

        public static List<Weapon> SMGs => new List<Weapon>()
        {
            new SMGMk2(),
            new CombatMGMk2(),
            new UnholyHellbringer(),
            new SMG(),
            new AssaultSMG(),
            new MicroSMG(),
            new MiniSMG(),
            new MG(),
            new CombatMG(),
            new CombatPDW(),
            new Gusenberg()
        };

        public static List<Weapon> Rifles => new List<Weapon>()
        {
            new Carbine(),
            new CarbineRifleMk2(),
            new AdvancedRifle(),
            new AssaultRifle(),
            new BullpupRifle(),
            new BullPupMk2(),
            new MilitaryRifle(),
            new AssaultRifleMk2(),
            new Musket(),
            new SpecialCarbine(),
            new SpecialCarbineMK2(),
            new CompactRifle()
        };

        public static List<Weapon> Shotguns => new List<Weapon>()
        {
            new CombatShotgun(),
            new PumpShotgunMk2(),
            new DoubleBarrelShotgun(),
            new SweeperShotgun(),
            new AssaultShotgun(),
            new BullpupShotgun(),
            new HeavyShotgun(),
            new PumpShotgun(),
            new SawnOffShotgun()
        };

        public static List<Weapon> Snipers => new List<Weapon>()
        {
            new HeavySniperMk2(),
            new MarksmanRifleMk2(),
            new HeavySniper(),
            new SniperRifle(),
            new MarksmanRifle()
        };

        public static List<Weapon> Heavy = new List<Weapon>()
        {
            new Widowmaker(),
            new Firework(),
            new CompactGrenadeLauncher(),
            new RPG(),
            new Minigun(),
            new GrenadeLauncher(),
            new HomingLauncher(),
            new Railgun()
        };

        public static List<Weapon> Explosives => new List<Weapon>()
        {
            new Grenade(),
            new StickyBomb(),
            new ProximityMine(),
            new Snowball(),
            new PipeBomb(),
            new Molotov(),
            new BZGas(),
            new Flare(),
            new SmokeGrenade()
        };
    }
}
