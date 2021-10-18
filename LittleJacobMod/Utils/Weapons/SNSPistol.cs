﻿using System;
using System.Collections.Generic;
using GTA;

namespace LittleJacobMod.Utils.Weapons
{
    internal class SNSPistol : Weapon
    {
        public override bool SaveFileWeapon => false;

        public override WeaponHash WeaponHash => WeaponHash.SNSPistol;

        public override int Price => 2700;

        public override string Name => "SNS Pistol";

        public override bool HasMuzzleOrSupp => false;

        public override bool HasClip => true;

        public override bool HasBarrel => false;

        public override bool HasGrip => false;

        public override bool HasScope => false;

        public override bool HasCamo => false;

        public override bool HasFlaslight => false;

        public override Dictionary<string, WeaponComponentHash> MuzzlesAndSupps => throw new NotImplementedException();

        public override Dictionary<string, WeaponComponentHash> Clips => new Dictionary<string, WeaponComponentHash>()
        {
            { "Normal - $199", WeaponComponentHash.SNSPistolClip01 },
            { "Extended - $12000", WeaponComponentHash.SNSPistolClip02 },
        };

        public override Dictionary<string, WeaponComponentHash> Barrels => throw new NotImplementedException();

        public override Dictionary<string, WeaponComponentHash> Grips => throw new NotImplementedException();

        public override Dictionary<string, WeaponComponentHash> Scopes => throw new NotImplementedException();

        public override Dictionary<string, WeaponComponentHash> Camos => throw new NotImplementedException();

        public override Dictionary<string, WeaponComponentHash> FlashLight => throw new NotImplementedException();
    }
}
