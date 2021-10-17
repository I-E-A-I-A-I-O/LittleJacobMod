using System;
using System.Collections.Generic;
using GTA;

namespace LittleJacobMod.Utils.Weapons
{
    internal class Pistol : Weapon
    {
        public override bool SaveFileWeapon => false;

        public override bool HasMuzzleOrSupp => true;

        public override bool HasClip => true;

        public override bool HasBarrel => false;

        public override bool HasGrip => false;

        public override bool HasScope => false;

        public override bool HasCamo => false;

        public override bool HasFlaslight => true;

        public override Dictionary<string, WeaponComponentHash> MuzzlesAndSupps => new Dictionary<string, WeaponComponentHash>
        {
            { "None - $100", WeaponComponentHash.Invalid },
            { "Supressor - $299", WeaponComponentHash.AtPiSupp02 }
        };

        public override Dictionary<string, WeaponComponentHash> Clips => new Dictionary<string, WeaponComponentHash>()
        {
            { "Normal - $150", WeaponComponentHash.PistolClip01 },
            { "Extended - $300", WeaponComponentHash.PistolClip02 },
        };

        public override Dictionary<string, WeaponComponentHash> Barrels => throw new NotImplementedException();

        public override Dictionary<string, WeaponComponentHash> Grips => throw new NotImplementedException();

        public override Dictionary<string, WeaponComponentHash> Scopes => throw new NotImplementedException();

        public override Dictionary<string, WeaponComponentHash> Camos => throw new NotImplementedException();

        public override Dictionary<string, WeaponComponentHash> FlashLight => new Dictionary<string, WeaponComponentHash>()
        {
            { "None - $100", WeaponComponentHash.Invalid },
            { "Flashlight - $299", WeaponComponentHash.AtPiFlsh }
        };

        public override int Price => 500;

        public override string Name => "Pistol";

        public override WeaponHash WeaponHash => WeaponHash.Pistol;
    }
}
