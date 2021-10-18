using System;
using System.Collections.Generic;
using GTA;

namespace LittleJacobMod.Utils.Weapons
{
    internal class CombatMGMk2 : Weapon
    {
        public override bool SaveFileWeapon => true;

        public override WeaponHash WeaponHash => WeaponHash.CombatMGMk2;

        public override int Price => 100000;

        public override string Name => "Combat MG Mk2";

        public override bool HasMuzzleOrSupp => true;

        public override bool HasClip => true;

        public override bool HasBarrel => true;

        public override bool HasGrip => true;

        public override bool HasScope => true;

        public override bool HasCamo => true;

        public override bool HasFlaslight => false;

        public override Dictionary<string, WeaponComponentHash> MuzzlesAndSupps => new Dictionary<string, WeaponComponentHash>()
        {
            { "None - $199", WeaponComponentHash.Invalid },
            { "Flat Muzzle - $29000", WeaponComponentHash.AtMuzzle01 },
            { "Tactical Muzzle - $31000", WeaponComponentHash.AtMuzzle02 },
            { "Fat-End Muzzle - $32000", WeaponComponentHash.AtMuzzle03 },
            { "Precision Muzzle - $34000", WeaponComponentHash.AtMuzzle04 },
            { "Heavy Duty Muzzle - $35000", WeaponComponentHash.AtMuzzle05 },
            { "Slanted Muzzle - $37000", WeaponComponentHash.AtMuzzle06 },
            { "Split-End Muzzle - $38000", WeaponComponentHash.AtMuzzle07 }
        };

        public override Dictionary<string, WeaponComponentHash> Clips => new Dictionary<string, WeaponComponentHash>()
        {
            { "Normal - $199", WeaponComponentHash.CombatMGMk2Clip01 },
            { "Extended - $25000", WeaponComponentHash.CombatMGMk2Clip02 },
            { "Tracer - $44000", WeaponComponentHash.CombatMGMk2ClipTracer },
            { "Incendiary - $51000", WeaponComponentHash.CombatMGMk2ClipIncendiary },
            { "Armor piercing - $66000", WeaponComponentHash.CombatMGMk2ClipArmorPiercing },
            { "FMJ - $76000", WeaponComponentHash.CombatMGMk2ClipFMJ },
        };

        public override Dictionary<string, WeaponComponentHash> Barrels => new Dictionary<string, WeaponComponentHash>()
        {
            { "Standard - $199", WeaponComponentHash.AtMGBarrel01 },
            { "Heavy - $49000", WeaponComponentHash.AtMGBarrel02 },
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
            { "Medium - $23000", WeaponComponentHash.AtScopeSmallMk2 },
            { "Large - $34000", WeaponComponentHash.AtScopeMediumMk2 },
        };

        public override Dictionary<string, WeaponComponentHash> Camos => new Dictionary<string, WeaponComponentHash>()
        {
            { "None", WeaponComponentHash.Invalid },
            { "Digital", WeaponComponentHash.CombatMGMk2Camo },
            { "Brushstroke", WeaponComponentHash.CombatMGMk2Camo02 },
            { "Woodland", WeaponComponentHash.CombatMGMk2Camo03 },
            { "Skull", WeaponComponentHash.CombatMGMk2Camo04 },
            { "Sessanta Nove", WeaponComponentHash.CombatMGMk2Camo05 },
            { "Perseus", WeaponComponentHash.CombatMGMk2Camo06 },
            { "Leopard", WeaponComponentHash.CombatMGMk2Camo07 },
            { "Zebra", WeaponComponentHash.CombatMGMk2Camo08 },
            { "Geometric", WeaponComponentHash.CombatMGMk2Camo09 },
            { "Boom!", WeaponComponentHash.CombatMGMk2Camo10 },
            { "Patriotic", WeaponComponentHash.CombatMGMk2CamoIndependence01 },

        };

        public override Dictionary<string, WeaponComponentHash> FlashLight => throw new NotImplementedException();
    }
}
