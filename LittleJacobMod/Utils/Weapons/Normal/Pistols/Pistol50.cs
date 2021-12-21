using System;
using System.Collections.Generic;
using GTA;

namespace LittleJacobMod.Utils.Weapons
{
    internal class Pistol50 : Weapon
    {
        public override bool SaveFileWeapon => true;

        public override uint WeaponHash => (uint)GTA.WeaponHash.Pistol50;

        public override int Price => 1500;

        public override string Name => "Pistol 50";

        public override bool HasMuzzleOrSupp => true;

        public override bool HasClip => true;

        public override bool HasBarrel => false;

        public override bool HasGrip => false;

        public override bool HasScope => false;

        public override bool HasCamo => true;

        public override bool HasFlaslight => true;

        public override Dictionary<string, uint> MuzzlesAndSupps => new Dictionary<string, uint>()
        {
            { "None - $500", (uint)WeaponComponentHash.Invalid },
            { "Suppressor - $1000", (uint)WeaponComponentHash.AtArSupp02 }
        };

        public override Dictionary<string, uint> Clips => new Dictionary<string, uint>()
        {
            { "Normal - $100", (uint)WeaponComponentHash.Pistol50Clip01 },
            { "Extended - $500", (uint)WeaponComponentHash.Pistol50Clip02 }
        };

        public override Dictionary<string, uint> Barrels => throw new NotImplementedException();

        public override Dictionary<string, uint> Grips => throw new NotImplementedException();

        public override Dictionary<string, uint> Scopes => throw new NotImplementedException();

        public override Dictionary<string, uint> Camos => new Dictionary<string, uint>()
        {
            { "None", (uint)WeaponComponentHash.Invalid },
            { "Luxury Finish", (uint)WeaponComponentHash.Pistol50VarmodLuxe },
        };

        public override Dictionary<string, uint> FlashLight => new Dictionary<string, uint>()
        {
            { "None - $100", (uint)WeaponComponentHash.Invalid },
            { "Flashlight - $200", (uint)WeaponComponentHash.AtPiFlsh }
        };
    }
}
