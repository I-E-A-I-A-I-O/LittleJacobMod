using System;
using System.Collections.Generic;
using GTA;

namespace LittleJacobMod.Utils.Weapons
{
    internal class MG : Weapon
    {
        public override bool SaveFileWeapon => true;

        public override uint WeaponHash => (uint)GTA.WeaponHash.MG;

        public override int Price => 10000;

        public override string Name => "MG";

        public override bool HasMuzzleOrSupp => false;

        public override bool HasClip => true;

        public override bool HasBarrel => false;

        public override bool HasGrip => false;

        public override bool HasScope => true;

        public override bool HasCamo => true;

        public override bool HasFlaslight => false;

        public override Dictionary<string, uint> MuzzlesAndSupps => throw new NotImplementedException();

        public override Dictionary<string, uint> Clips => new Dictionary<string, uint>()
        {
            { "Normal - $199", (uint)WeaponComponentHash.MGClip01 },
            { "Extended - $8000", (uint)WeaponComponentHash.MGClip02 }
        };

        public override Dictionary<string, uint> Barrels => throw new NotImplementedException();

        public override Dictionary<string, uint> Grips => throw new NotImplementedException();

        public override Dictionary<string, uint> Scopes => new Dictionary<string, uint>()
        {
            { "None - $199", (uint)WeaponComponentHash.Invalid },
            { "Scope - $8000", (uint)WeaponComponentHash.AtScopeSmall02 }
        };

        public override Dictionary<string, uint> Camos => new Dictionary<string, uint>()
        {
            { "None", (uint)WeaponComponentHash.Invalid },
            { "Luxury Finish", (uint)WeaponComponentHash.MGVarmodLowrider },
        };

        public override Dictionary<string, uint> FlashLight => throw new NotImplementedException();
    }
}
