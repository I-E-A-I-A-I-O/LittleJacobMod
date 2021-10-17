using System;
using System.Collections.Generic;
using GTA;

namespace LittleJacobMod.Utils.Weapons
{
    internal class HeavySniperMk2 : Weapon
    {
        public override bool SaveFileWeapon => true;

        public override WeaponHash WeaponHash => WeaponHash.HeavySniperMk2;

        public override int Price => 165000;

        public override string Name => "Heavy Sniper Mk2";

        public override bool HasMuzzleOrSupp => true;

        public override bool HasClip => true;

        public override bool HasBarrel => true;

        public override bool HasGrip => false;

        public override bool HasScope => true;

        public override bool HasCamo => true;

        public override bool HasFlaslight => false;

        public override Dictionary<string, WeaponComponentHash> MuzzlesAndSupps => new Dictionary<string, WeaponComponentHash>()
        {
            { "None - $199", WeaponComponentHash.Invalid },
            { "Suppressor - $60000", WeaponComponentHash.AtSrSupp03 },
            { "Squared Muzzle - $45000", WeaponComponentHash.AtMuzzle08 },
            { "Bell-End Muzzle - $57000", WeaponComponentHash.AtMuzzle09 },
        };

        public override Dictionary<string, WeaponComponentHash> Clips => new Dictionary<string, WeaponComponentHash>()
        {
            { "Normal - $199", WeaponComponentHash.HeavySniperMk2Clip01 },
            { "Extended - $32000", WeaponComponentHash.HeavySniperMk2Clip02 },
            { "Armor piercing - $76000", WeaponComponentHash.HeavySniperMk2ClipArmorPiercing },
            { "Incendiary - $59000", WeaponComponentHash.HeavySniperMk2ClipIncendiary },
            { "FMJ - $88000", WeaponComponentHash.HeavySniperMk2ClipFMJ },
            { "Explosive - $115000", WeaponComponentHash.HeavySniperMk2ClipExplosive }
        };

        public override Dictionary<string, WeaponComponentHash> Barrels => new Dictionary<string, WeaponComponentHash>()
        {
            { "Standard - $299", WeaponComponentHash.AtSrBarrel01 },
            { "Heavy - $69000", WeaponComponentHash.AtSrBarrel02 }
        };
 
        public override Dictionary<string, WeaponComponentHash> Grips => throw new NotImplementedException();

        public override Dictionary<string, WeaponComponentHash> Scopes => new Dictionary<string, WeaponComponentHash>()
        {
            { "Default - $199", WeaponComponentHash.Invalid },
            { "Zoom - $29000", WeaponComponentHash.AtScopeLargeMk2 },
            { "Night Vision - $45000", WeaponComponentHash.AtScopeNV },
            { "Thermal - $69000", WeaponComponentHash.AtScopeThermal }
        };

        public override Dictionary<string, WeaponComponentHash> Camos => throw new NotImplementedException();

        public override Dictionary<string, WeaponComponentHash> FlashLight => throw new NotImplementedException();
    }
}
