using System;
using System.Collections.Generic;
using GTA;

namespace LittleJacobMod.Utils.Weapons
{
    internal class RevolverMk2 : Weapon
    {
        public override bool SaveFileWeapon => true;

        public override WeaponHash WeaponHash => WeaponHash.RevolverMk2;

        public override int Price => 99000;

        public override string Name => "Heavy Revolver Mk2";

        public override bool HasMuzzleOrSupp => true;

        public override bool HasClip => true;

        public override bool HasBarrel => false;

        public override bool HasGrip => false;

        public override bool HasScope => true;

        public override bool HasCamo => true;

        public override bool HasFlaslight => false;

        public override Dictionary<string, WeaponComponentHash> MuzzlesAndSupps => new Dictionary<string, WeaponComponentHash>()
        {
            { "None - $199", WeaponComponentHash.Invalid },
            { "Compensator - $21000", WeaponComponentHash.AtPiComp03 }
        };

        public override Dictionary<string, WeaponComponentHash> Clips => new Dictionary<string, WeaponComponentHash>()
        {
            { "Normal - $199" , WeaponComponentHash.RevolverMk2Clip01 },
            { "Tracer - $28000", WeaponComponentHash.RevolverMk2ClipTracer },
            { "Incendiary - $34000", WeaponComponentHash.RevolverMk2ClipIncendiary },
            { "Hollow Point - $39000", WeaponComponentHash.RevolverMk2ClipHollowPoint },
            { "FMJ - $52000", WeaponComponentHash.RevolverMk2ClipFMJ }
        };

        public override Dictionary<string, WeaponComponentHash> Barrels => throw new NotImplementedException();

        public override Dictionary<string, WeaponComponentHash> Grips => throw new NotImplementedException();

        public override Dictionary<string, WeaponComponentHash> Scopes => new Dictionary<string, WeaponComponentHash>()
        {
            { "None - $199", WeaponComponentHash.Invalid },
            { "Holographic - $16000", WeaponComponentHash.AtSights },
            { "Small scope - $25000", WeaponComponentHash.AtScopeMacroMk2 }
        };

        public override Dictionary<string, WeaponComponentHash> Camos => throw new NotImplementedException();

        public override Dictionary<string, WeaponComponentHash> FlashLight => throw new NotImplementedException();
    }
}
