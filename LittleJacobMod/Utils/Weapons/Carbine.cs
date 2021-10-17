using System;
using System.Collections.Generic;
using GTA;

namespace LittleJacobMod.Utils.Weapons
{
    internal class Carbine : Weapon
    {
        public override bool SaveFileWeapon => false;

        public override WeaponHash WeaponHash => WeaponHash.CarbineRifle;

        public override int Price => 2100;

        public override string Name => "Carbine Rifle";

        public override bool HasMuzzleOrSupp => true;

        public override bool HasClip => true;

        public override bool HasBarrel => false;

        public override bool HasGrip => true;

        public override bool HasScope => true;

        public override bool HasCamo => false;

        public override bool HasFlaslight => true;

        public override Dictionary<string, WeaponComponentHash> MuzzlesAndSupps => new Dictionary<string, WeaponComponentHash>()
        {
            { "None - $199", WeaponComponentHash.Invalid },
            { "Suppressor - $2000", WeaponComponentHash.AtArSupp }
        };

        public override Dictionary<string, WeaponComponentHash> Clips => new Dictionary<string, WeaponComponentHash>()
        {
            { "Normal - $199", WeaponComponentHash.CarbineRifleClip01 },
            { "Extended - $4000", WeaponComponentHash.CarbineRifleClip02 },
            { "Box - $15000", WeaponComponentHash.CarbineRifleClip03 }
        };

        public override Dictionary<string, WeaponComponentHash> Barrels => throw new NotImplementedException();

        public override Dictionary<string, WeaponComponentHash> Grips => new Dictionary<string, WeaponComponentHash>()
        {
            { "None - $199", WeaponComponentHash.Invalid },
            { "Grip - $4000", WeaponComponentHash.AtArAfGrip }
        };

        public override Dictionary<string, WeaponComponentHash> Scopes => new Dictionary<string, WeaponComponentHash>()
        {
            { "None - $199", WeaponComponentHash.Invalid },
            { "Scope - $5000", WeaponComponentHash.AtScopeMedium }
        };

        public override Dictionary<string, WeaponComponentHash> Camos => throw new NotImplementedException();

        public override Dictionary<string, WeaponComponentHash> FlashLight => new Dictionary<string, WeaponComponentHash>()
        {
            { "None - $199", WeaponComponentHash.Invalid },
            { "Flashlight - $2000", WeaponComponentHash.AtArFlsh }
        };
    }
}
