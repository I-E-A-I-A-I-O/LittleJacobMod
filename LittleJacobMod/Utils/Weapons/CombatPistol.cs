﻿using System;
using System.Collections.Generic;
using GTA;

namespace LittleJacobMod.Utils.Weapons
{
    internal class CombatPistol : Weapon
    {
        public override bool SaveFileWeapon => false;

        public override WeaponHash WeaponHash => WeaponHash.CombatPistol;

        public override int Price => 1500;

        public override string Name => "Combat pistol";

        public override bool HasMuzzleOrSupp => true;

        public override bool HasClip => true;

        public override bool HasBarrel => false;

        public override bool HasGrip => false;

        public override bool HasScope => false;

        public override bool HasCamo => false;

        public override bool HasFlaslight => true;

        public override Dictionary<string, WeaponComponentHash> MuzzlesAndSupps => new Dictionary<string, WeaponComponentHash>()
        {
            { "None - $199", WeaponComponentHash.Invalid },
            { "Suppressor - $6000", WeaponComponentHash.AtPiSupp },
        };

        public override Dictionary<string, WeaponComponentHash> Clips => new Dictionary<string, WeaponComponentHash>()
        {
            { "Normal - $199", WeaponComponentHash.CombatPistolClip01 },
            { "Extended - $6000", WeaponComponentHash.CombatPistolClip02 },
        };

        public override Dictionary<string, WeaponComponentHash> Barrels => throw new NotImplementedException();

        public override Dictionary<string, WeaponComponentHash> Grips => throw new NotImplementedException();

        public override Dictionary<string, WeaponComponentHash> Scopes => throw new NotImplementedException();

        public override Dictionary<string, WeaponComponentHash> Camos => throw new NotImplementedException();

        public override Dictionary<string, WeaponComponentHash> FlashLight => new Dictionary<string, WeaponComponentHash>()
        {
            { "None - $199", WeaponComponentHash.Invalid },
            { "Flashlight - $1200", WeaponComponentHash.AtPiFlsh },
        };
    }
}
