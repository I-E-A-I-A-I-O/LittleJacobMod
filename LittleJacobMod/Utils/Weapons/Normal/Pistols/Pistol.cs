using System;
using System.Collections.Generic;
using GTA;

namespace LittleJacobMod.Utils.Weapons
{
    internal class Pistol : Weapon
    {
        public override bool SaveFileWeapon => true;

        public override bool HasMuzzleOrSupp => true;

        public override bool HasClip => true;

        public override bool HasBarrel => false;

        public override bool HasGrip => false;

        public override bool HasScope => false;

        public override bool HasCamo => false;

        public override bool HasFlaslight => true;

        public override Dictionary<string, uint> MuzzlesAndSupps => new Dictionary<string, uint>
        {
            { "None - $100", (uint)WeaponComponentHash.Invalid },
            { "Supressor - $299", (uint)WeaponComponentHash.AtPiSupp02 }
        };

        public override Dictionary<string, uint> Clips => new Dictionary<string, uint>()
        {
            { "Normal - $150", (uint)WeaponComponentHash.PistolClip01 },
            { "Extended - $300", (uint)WeaponComponentHash.PistolClip02 },
        };

        public override Dictionary<string, uint> Barrels => throw new NotImplementedException();

        public override Dictionary<string, uint> Grips => throw new NotImplementedException();

        public override Dictionary<string, uint> Scopes => throw new NotImplementedException();

        public override Dictionary<string, uint> Camos => throw new NotImplementedException();

        public override Dictionary<string, uint> FlashLight => new Dictionary<string, uint>()
        {
            { "None - $100", (uint)WeaponComponentHash.Invalid },
            { "Flashlight - $299", (uint)WeaponComponentHash.AtPiFlsh }
        };

        public override int Price => 500;

        public override string Name => "Pistol";

        public override uint WeaponHash => (uint)GTA.WeaponHash.Pistol;
    }
}
