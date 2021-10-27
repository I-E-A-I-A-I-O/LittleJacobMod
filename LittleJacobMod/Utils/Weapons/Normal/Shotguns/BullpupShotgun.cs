using System;
using System.Collections.Generic;
using GTA;

namespace LittleJacobMod.Utils.Weapons
{
    internal class BullpupShotgun : Weapon
    {
        public override bool SaveFileWeapon => true;

        public override WeaponHash WeaponHash => WeaponHash.BullpupShotgun;

        public override int Price => 7000;

        public override string Name => "Bullpup Shotgun";

        public override bool HasMuzzleOrSupp => true;

        public override bool HasClip => false;

        public override bool HasBarrel => false;

        public override bool HasGrip => true;

        public override bool HasScope => false;

        public override bool HasCamo => false;

        public override bool HasFlaslight => true;

        public override Dictionary<string, WeaponComponentHash> MuzzlesAndSupps => new Dictionary<string, WeaponComponentHash>()
        {
            { "None - $199", WeaponComponentHash.Invalid },
            { "Suppressor - $28000", WeaponComponentHash.AtArSupp02 }
        };

        public override Dictionary<string, WeaponComponentHash> Clips => throw new NotImplementedException();

        public override Dictionary<string, WeaponComponentHash> Barrels => throw new NotImplementedException();

        public override Dictionary<string, WeaponComponentHash> Grips => new Dictionary<string, WeaponComponentHash>()
        {
            { "None - $199", WeaponComponentHash.Invalid },
            { "Grip - $8000", WeaponComponentHash.AtArAfGrip }
        };

        public override Dictionary<string, WeaponComponentHash> Scopes => throw new NotImplementedException();

        public override Dictionary<string, WeaponComponentHash> Camos => throw new NotImplementedException();

        public override Dictionary<string, WeaponComponentHash> FlashLight => new Dictionary<string, WeaponComponentHash>()
        {
            { "None - $199", WeaponComponentHash.Invalid },
            { "Flashlight - $3500", WeaponComponentHash.AtArFlsh }
        };
    }
}
