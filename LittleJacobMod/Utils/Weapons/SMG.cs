using System;
using System.Collections.Generic;
using GTA;

namespace LittleJacobMod.Utils.Weapons
{
    internal class SMG : Weapon
    {
        public override bool SaveFileWeapon => false;

        public override WeaponHash WeaponHash => WeaponHash.SMG;

        public override int Price => 5000;

        public override string Name => "SMG";

        public override bool HasMuzzleOrSupp => true;

        public override bool HasClip => true;

        public override bool HasBarrel => false;

        public override bool HasGrip => false;

        public override bool HasScope => true;

        public override bool HasCamo => false;

        public override bool HasFlaslight => true;

        public override Dictionary<string, WeaponComponentHash> MuzzlesAndSupps => new Dictionary<string, WeaponComponentHash>()
        {
            { "None - $199", WeaponComponentHash.Invalid },
            { "Suppressor - $2000", WeaponComponentHash.AtArSupp02 }
        };

        public override Dictionary<string, WeaponComponentHash> Clips => new Dictionary<string, WeaponComponentHash>()
        {
            { "Normal - $199", WeaponComponentHash.SMGClip01 },
            { "Extended - $6000", WeaponComponentHash.SMGClip02 },
            { "Drum - $16000", WeaponComponentHash.SMGClip03 }
        };

        public override Dictionary<string, WeaponComponentHash> Barrels => throw new NotImplementedException();

        public override Dictionary<string, WeaponComponentHash> Grips => throw new NotImplementedException();

        public override Dictionary<string, WeaponComponentHash> Scopes => new Dictionary<string, WeaponComponentHash>()
        {
            { "None - $199", WeaponComponentHash.Invalid },
            { "Scope - $8000", WeaponComponentHash.AtScopeSmall02 }
        };

        public override Dictionary<string, WeaponComponentHash> Camos => throw new NotImplementedException();

        public override Dictionary<string, WeaponComponentHash> FlashLight => new Dictionary<string, WeaponComponentHash>()
        {
            { "None - $199", WeaponComponentHash.Invalid },
            { "Flashlight - $2000", WeaponComponentHash.AtArFlsh }
        };
    }
}
