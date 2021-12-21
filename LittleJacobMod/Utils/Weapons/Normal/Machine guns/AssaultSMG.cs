using System;
using System.Collections.Generic;
using GTA;

namespace LittleJacobMod.Utils.Weapons
{
    internal class AssaultSMG : Weapon
    {
        public override bool SaveFileWeapon => true;

        public override uint WeaponHash => (uint)GTA.WeaponHash.AssaultSMG;

        public override int Price => 8000;

        public override string Name => "Assault SMG";

        public override bool HasMuzzleOrSupp => true;

        public override bool HasClip => true;

        public override bool HasBarrel => false;

        public override bool HasGrip => false;

        public override bool HasScope => true;

        public override bool HasCamo => true;

        public override bool HasFlaslight => true;

        public override Dictionary<string, uint> MuzzlesAndSupps => new Dictionary<string, uint>()
        {
            { "None - $199", (uint)WeaponComponentHash.Invalid },
            { "Suppressor - $2000", (uint)WeaponComponentHash.AtArSupp02 }
        };

        public override Dictionary<string, uint> Clips => new Dictionary<string, uint>()
        {
            { "Normal - $199", (uint)WeaponComponentHash.AssaultSMGClip01 },
            { "Extended - $6000", (uint)WeaponComponentHash.AssaultSMGClip02 },
        };

        public override Dictionary<string, uint> Barrels => throw new NotImplementedException();

        public override Dictionary<string, uint> Grips => throw new NotImplementedException();

        public override Dictionary<string, uint> Scopes => new Dictionary<string, uint>()
        {
            { "None - $199", (uint)WeaponComponentHash.Invalid },
            { "Scope - $8000", (uint)WeaponComponentHash.AtScopeMacro }
        };

        public override Dictionary<string, uint> Camos => new Dictionary<string, uint>()
        {
            { "None", (uint)WeaponComponentHash.Invalid },
            { "Luxury Finish", (uint)WeaponComponentHash.AssaultSMGVarmodLowrider },
        };

        public override Dictionary<string, uint> FlashLight => new Dictionary<string, uint>()
        {
            { "None - $199", (uint)WeaponComponentHash.Invalid },
            { "Flashlight - $2000", (uint)WeaponComponentHash.AtArFlsh }
        };
    }
}
