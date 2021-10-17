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
            new RevolverMk2()
        };

        public static List<Weapon> Rifles => new List<Weapon>()
        {
            new Carbine(),
            new CarbineRifleMk2()
        };

        public static List<Weapon> Snipers => new List<Weapon>()
        {
            new HeavySniperMk2()
        };
    }
}
