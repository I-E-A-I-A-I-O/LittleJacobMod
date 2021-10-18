using System;
using System.Collections.Generic;
using GTA;

namespace LittleJacobMod.Utils.Weapons
{
    internal class CombatShotgun : Weapon
    {
        public override bool SaveFileWeapon => true;

        public override WeaponHash WeaponHash => WeaponHash.CombatShotgun;

        public override int Price => 120000;

        public override string Name => "Combat Shotgun";

        public override bool HasMuzzleOrSupp => true;

        public override bool HasClip => false;

        public override bool HasBarrel => false;

        public override bool HasGrip => false;

        public override bool HasScope => false;

        public override bool HasCamo => false;

        public override bool HasFlaslight => true;

        public override Dictionary<string, WeaponComponentHash> MuzzlesAndSupps => new Dictionary<string, WeaponComponentHash>()
        {
            { "None - $199", WeaponComponentHash.Invalid },
            { "Suppressor - $19000", WeaponComponentHash.AtArSupp },
        };

        public override Dictionary<string, WeaponComponentHash> Clips => throw new NotImplementedException();

        public override Dictionary<string, WeaponComponentHash> Barrels => throw new NotImplementedException();

        public override Dictionary<string, WeaponComponentHash> Grips => throw new NotImplementedException();

        public override Dictionary<string, WeaponComponentHash> Scopes => throw new NotImplementedException();

        public override Dictionary<string, WeaponComponentHash> Camos => throw new NotImplementedException();

        public override Dictionary<string, WeaponComponentHash> FlashLight => new Dictionary<string, WeaponComponentHash>()
        {
            { "None - $199", WeaponComponentHash.Invalid },
            { "Flashlight - $6000", WeaponComponentHash.AtArFlsh },
        };
    }
}
