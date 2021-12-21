using System;
using System.Collections.Generic;
using GTA;

namespace LittleJacobMod.Utils.Weapons
{
    internal class ApPistol : Weapon
    {
        public override bool SaveFileWeapon => true;

        public override uint WeaponHash => (uint)GTA.WeaponHash.APPistol;

        public override int Price => 1000;

        public override string Name => "AP Pistol";

        public override bool HasMuzzleOrSupp => true;

        public override bool HasClip => true;

        public override bool HasBarrel => false;

        public override bool HasGrip => false;

        public override bool HasScope => false;

        public override bool HasCamo => true;

        public override bool HasFlaslight => true;

        public override Dictionary<string, uint> MuzzlesAndSupps => new Dictionary<string, uint>()
        {
            { "None - $199", (uint)WeaponComponentHash.Invalid },
            { "Suppressor - $6000", (uint)WeaponComponentHash.AtPiSupp }
        };

        public override Dictionary<string, uint> Clips => new Dictionary<string, uint>()
        {
            { "Normal - $199", (uint)WeaponComponentHash.APPistolClip01 },
            { "Extended - $6000", (uint)WeaponComponentHash.APPistolClip02 }
        };

        public override Dictionary<string, uint> Barrels => throw new NotImplementedException();

        public override Dictionary<string, uint> Grips => throw new NotImplementedException();

        public override Dictionary<string, uint> Scopes => throw new NotImplementedException();

        public override Dictionary<string, uint> Camos => new Dictionary<string, uint>()
        {
            { "None", (uint)WeaponComponentHash.Invalid },
            { "Luxury Finish", (uint)WeaponComponentHash.APPistolVarmodLuxe },
        };

        public override Dictionary<string, uint> FlashLight => new Dictionary<string, uint>()
        {
            { "None - $199", (uint)WeaponComponentHash.Invalid },
            { "Flashlight - $1900", (uint)WeaponComponentHash.AtPiFlsh }
        };
    }
}
