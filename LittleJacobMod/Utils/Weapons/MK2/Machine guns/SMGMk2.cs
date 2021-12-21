using System;
using System.Collections.Generic;
using GTA;

namespace LittleJacobMod.Utils.Weapons
{
    internal class SMGMk2 : Weapon
    {
        public override bool SaveFileWeapon => true;

        public override uint WeaponHash => (uint)GTA.WeaponHash.SMGMk2;

        public override int Price => 62000;

        public override string Name => "SMG Mk2";

        public override bool HasMuzzleOrSupp => true;

        public override bool HasClip => true;

        public override bool HasBarrel => true;

        public override bool HasGrip => false;

        public override bool HasScope => true;

        public override bool HasCamo => true;

        public override bool HasFlaslight => true;

        public override Dictionary<string, uint> MuzzlesAndSupps => new Dictionary<string, uint>()
        {
            { "None - $199", (uint)WeaponComponentHash.Invalid },
            { "Suppressor - $40000", (uint)WeaponComponentHash.AtPiSupp },
            { "Flat Muzzle - $29000", (uint)WeaponComponentHash.AtMuzzle01 },
            { "Tactical Muzzle - $31000", (uint)WeaponComponentHash.AtMuzzle02 },
            { "Fat End Muzzle - $32000", (uint)WeaponComponentHash.AtMuzzle03 },
            { "Precision Muzzle - $34000", (uint)WeaponComponentHash.AtMuzzle04 },
            { "Heavy Duty Muzzle - $35000", (uint)WeaponComponentHash.AtMuzzle05 },
            { "Slanted Muzzle - $37000", (uint)WeaponComponentHash.AtMuzzle06 },
            { "Split End Muzzle - $38000", (uint)WeaponComponentHash.AtMuzzle07 }
        };

        public override Dictionary<string, uint> Clips => new Dictionary<string, uint>()
        {
            { "Normal - $199", (uint)WeaponComponentHash.SMGMk2Clip01 },
            { "Extended - $25000", (uint)WeaponComponentHash.SMGMk2Clip02 },
            { "Tracer - $44000", (uint)WeaponComponentHash.SMGMk2ClipTracer },
            { "Incendiary - $51000", (uint)WeaponComponentHash.SMGMk2ClipIncendiary },
            { "Hollow Point - $66000", (uint)WeaponComponentHash.SMGMk2ClipHollowPoint },
            { "FMJ - $76000", (uint)WeaponComponentHash.SMGMk2ClipFMJ },
        };

        public override Dictionary<string, uint> Barrels => new Dictionary<string, uint>()
        {
            { "Standard - $199", (uint)WeaponComponentHash.AtSbBarrel01 },
            { "Heavy - $49000", (uint)WeaponComponentHash.AtSbBarrel02 },
        };

        public override Dictionary<string, uint> Grips => throw new NotImplementedException();

        public override Dictionary<string, uint> Scopes => new Dictionary<string, uint>()
        {
            { "None - $199", (uint)WeaponComponentHash.Invalid },
            { "Holographic - $19000", (uint)WeaponComponentHash.AtSightsSMG },
            { "Small - $23000", (uint)WeaponComponentHash.AtScopeMacro02SMGMk2 },
            { "Medium - $34000", (uint)WeaponComponentHash.AtScopeSmallSMGMk2 },
        };

        public override Dictionary<string, uint> Camos => new Dictionary<string, uint>()
        {
            { "None", (uint)WeaponComponentHash.Invalid },
            { "Digital", (uint)WeaponComponentHash.SMGMk2Camo },
            { "Brushstroke", (uint)WeaponComponentHash.SMGMk2Camo02 },
            { "Woodland", (uint)WeaponComponentHash.SMGMk2Camo03 },
            { "Skull", (uint)WeaponComponentHash.SMGMk2Camo04 },
            { "Sessanta Nove", (uint)WeaponComponentHash.SMGMk2Camo05 },
            { "Perseus", (uint)WeaponComponentHash.SMGMk2Camo06 },
            { "Leopard", (uint)WeaponComponentHash.SMGMk2Camo07 },
            { "Zebra", (uint)WeaponComponentHash.SMGMk2Camo08 },
            { "Geometric", (uint)WeaponComponentHash.SMGMk2Camo09 },
            { "Boom!", (uint)WeaponComponentHash.SMGMk2Camo10 },
            { "Patriotic", (uint)WeaponComponentHash.SMGMk2CamoIndependence01 },

        };

        public override Dictionary<string, uint> FlashLight => new Dictionary<string, uint>()
        {
            { "None - $199", (uint)WeaponComponentHash.Invalid },
            { "Flashlight - $10000", (uint)WeaponComponentHash.AtArFlsh }
        };
    }
}
