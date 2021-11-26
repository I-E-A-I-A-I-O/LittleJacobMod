using System;
using System.Collections.Generic;
using GTA;

namespace LittleJacobMod.Utils.Weapons
{
    internal class SpecialCarbineMK2 : Weapon
    {
        public override bool SaveFileWeapon => true;

        public override WeaponHash WeaponHash => WeaponHash.SpecialCarbineMk2;

        public override int Price => 112000;

        public override string Name => "Special Carbine MK2";

        public override bool HasMuzzleOrSupp => true;

        public override bool HasClip => true;

        public override bool HasBarrel => true;

        public override bool HasGrip => true;

        public override bool HasScope => true;

        public override bool HasCamo => true;

        public override bool HasFlaslight => true;

        public override Dictionary<string, WeaponComponentHash> MuzzlesAndSupps => new Dictionary<string, WeaponComponentHash>()
        {
            { "None - $199", WeaponComponentHash.Invalid },
            { "Suppressor - $40000", WeaponComponentHash.AtArSupp02 },
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
            { "Normal - $199", WeaponComponentHash.SpecialCarbineMk2Clip01 },
            { "Extended - $25000", WeaponComponentHash.SpecialCarbineMk2Clip02 },
            { "Tracer - $44000", WeaponComponentHash.SpecialCarbineMk2ClipTracer },
            { "Incendiary - $51000", WeaponComponentHash.SpecialCarbineMk2ClipIncendiary },
            { "Armor piercing - $66000", WeaponComponentHash.SpecialCarbineMk2ClipArmorPiercing },
            { "FMJ - $76000", WeaponComponentHash.SpecialCarbineMk2ClipFMJ },
        };

        public override Dictionary<string, WeaponComponentHash> Barrels => new Dictionary<string, WeaponComponentHash>()
        {
            { "Standard - $199", WeaponComponentHash.AtScBarrel01 },
            { "Heavy - $49000", WeaponComponentHash.AtScBarrel02 },
        };

        public override Dictionary<string, WeaponComponentHash> Grips => new Dictionary<string, WeaponComponentHash>()
        {
            { "None - $199", WeaponComponentHash.Invalid },
            { "Grip - $14000", WeaponComponentHash.AtArAfGrip02 },
        };

        public override Dictionary<string, WeaponComponentHash> Scopes => new Dictionary<string, WeaponComponentHash>()
        {
            { "None - $199", WeaponComponentHash.Invalid },
            { "Holographic - $19000", WeaponComponentHash.AtSights },
            { "Small - $23000", WeaponComponentHash.AtScopeMacroMk2 },
            { "Large - $34000", WeaponComponentHash.AtScopeMediumMk2 },
        };

        public override Dictionary<string, WeaponComponentHash> Camos => new Dictionary<string, WeaponComponentHash>()
        {
            { "None", WeaponComponentHash.Invalid },
            { "Digital", WeaponComponentHash.SpecialCarbineMk2Camo },
            { "Brushstroke", WeaponComponentHash.SpecialCarbineMk2Camo02 },
            { "Woodland", WeaponComponentHash.SpecialCarbineMk2Camo03 },
            { "Skull", WeaponComponentHash.SpecialCarbineMk2Camo04 },
            { "Sessanta Nove", WeaponComponentHash.SpecialCarbineMk2Camo05 },
            { "Perseus", WeaponComponentHash.SpecialCarbineMk2Camo06 },
            { "Leopard", WeaponComponentHash.SpecialCarbineMk2Camo07 },
            { "Zebra", WeaponComponentHash.SpecialCarbineMk2Camo08 },
            { "Geometric", WeaponComponentHash.SpecialCarbineMk2Camo09 },
            { "Boom!", WeaponComponentHash.SpecialCarbineMk2Camo10 },
            { "Patriotic", WeaponComponentHash.SpecialCarbineMk2CamoIndependence01 },

        };

        public override Dictionary<string, WeaponComponentHash> FlashLight => new Dictionary<string, WeaponComponentHash>()
        {
            { "None - $199", WeaponComponentHash.Invalid },
            { "Flashlight - $10000", WeaponComponentHash.AtArFlsh }
        };
    }
}
