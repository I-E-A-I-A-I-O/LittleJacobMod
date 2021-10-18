using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleJacobMod.Utils.Weapons
{
    internal static class WeaponsList
    {
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
            new NavyRevolver()
        };

        public static List<Weapon> SMGs => new List<Weapon>()
        {
            new SMGMk2(),
            new CombatMGMk2(),
            new UnholyHellbringer(),
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
        };

        public static List<Weapon> Shotguns => new List<Weapon>()
        {
            new CombatShotgun(),
            new PumpShotgunMk2(),
        };

        public static List<Weapon> Snipers => new List<Weapon>()
        {
            new HeavySniperMk2(),
            new MarksmanRifleMk2()
        };

        public static List<Weapon> Heavy = new List<Weapon>()
        {
            new Widowmaker(),
            new Firework()
        };
    }
}
