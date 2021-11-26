using System;
using System.Collections.Generic;
using GTA;

namespace LittleJacobMod.Utils.Weapons
{
    internal class SMGMk2 : Weapon
    {
        public override bool SaveFileWeapon => true;

        public override WeaponHash WeaponHash => WeaponHash.SMGMk2;

        public override int Price => 62000;

        public override string Name => "SMG Mk2";

        public override bool HasMuzzleOrSupp => true;

        public override bool HasClip => true;

        public override bool HasBarrel => true;

        public override bool HasGrip => false;

        public override bool HasScope => true;

        public override bool HasCamo => true;

        public override bool HasFlaslight => true;

        public override Dictionary<string, WeaponComponentHash> MuzzlesAndSupps => new Dictionary<string, WeaponComponentHash>()
        {
            { "None - $199", WeaponComponentHash.Invalid },
            { "Suppressor - $40000", WeaponComponentHash.AtPiSupp },
            { "Flat Muzzle - $29000", WeaponComponentHash.AtMuzzle01 },
            { "Tactical Muzzle - $31000", WeaponComponentHash.AtMuzzle02 },
            { "Fat End Muzzle - $32000", WeaponComponentHash.AtMuzzle03 },
            { "Precision Muzzle - $34000", WeaponComponentHash.AtMuzzle04 },
            { "Heavy Duty Muzzle - $35000", WeaponComponentHash.AtMuzzle05 },
            { "Slanted Muzzle - $37000", WeaponComponentHash.AtMuzzle06 },
            { "Split End Muzzle - $38000", WeaponComponentHash.AtMuzzle07 }
        };

        public override Dictionary<string, WeaponComponentHash> Clips => new Dictionary<string, WeaponComponentHash>()
        {
            { "Normal - $199", WeaponComponentHash.SMGMk2Clip01 },
            { "Extended - $25000", WeaponComponentHash.SMGMk2Clip02 },
            { "Tracer - $44000", WeaponComponentHash.SMGMk2ClipTracer },
            { "Incendiary - $51000", WeaponComponentHash.SMGMk2ClipIncendiary },
            { "Hollow Point - $66000", WeaponComponentHash.SMGMk2ClipHollowPoint },
            { "FMJ - $76000", WeaponComponentHash.SMGMk2ClipFMJ },
        };

        public override Dictionary<string, WeaponComponentHash> Barrels => new Dictionary<string, WeaponComponentHash>()
        {
            { "Standard - $199", WeaponComponentHash.AtSbBarrel01 },
            { "Heavy - $49000", WeaponComponentHash.AtSbBarrel02 },
        };

        public override Dictionary<string, WeaponComponentHash> Grips => throw new NotImplementedException();

        public override Dictionary<string, WeaponComponentHash> Scopes => new Dictionary<string, WeaponComponentHash>()
        {
            { "None - $199", WeaponComponentHash.Invalid },
            { "Holographic - $19000", WeaponComponentHash.AtSightsSMG },
            { "Small - $23000", WeaponComponentHash.AtScopeMacro02SMGMk2 },
            { "Medium - $34000", WeaponComponentHash.AtScopeSmallSMGMk2 },
        };

        public override Dictionary<string, WeaponComponentHash> Camos => new Dictionary<string, WeaponComponentHash>()
        {
            { "None", WeaponComponentHash.Invalid },
            { "Digital", WeaponComponentHash.SMGMk2Camo },
            { "Brushstroke", WeaponComponentHash.SMGMk2Camo02 },
            { "Woodland", WeaponComponentHash.SMGMk2Camo03 },
            { "Skull", WeaponComponentHash.SMGMk2Camo04 },
            { "Sessanta Nove", WeaponComponentHash.SMGMk2Camo05 },
            { "Perseus", WeaponComponentHash.SMGMk2Camo06 },
            { "Leopard", WeaponComponentHash.SMGMk2Camo07 },
            { "Zebra", WeaponComponentHash.SMGMk2Camo08 },
            { "Geometric", WeaponComponentHash.SMGMk2Camo09 },
            { "Boom!", WeaponComponentHash.SMGMk2Camo10 },
            { "Patriotic", WeaponComponentHash.SMGMk2CamoIndependence01 },

        };

        public override Dictionary<string, WeaponComponentHash> FlashLight => new Dictionary<string, WeaponComponentHash>()
        {
            { "None - $199", WeaponComponentHash.Invalid },
            { "Flashlight - $10000", WeaponComponentHash.AtArFlsh }
        };
    }
}
