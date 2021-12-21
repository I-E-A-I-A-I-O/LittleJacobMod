using System;
using System.Collections.Generic;
using GTA;

namespace LittleJacobMod.Utils.Weapons
{
    internal class HeavySniperMk2 : Weapon
    {
        public override bool SaveFileWeapon => true;

        public override uint WeaponHash => (uint)GTA.WeaponHash.HeavySniperMk2;

        public override int Price => 157000;

        public override string Name => "Heavy Sniper Mk2";

        public override bool HasMuzzleOrSupp => true;

        public override bool HasClip => true;

        public override bool HasBarrel => true;

        public override bool HasGrip => false;

        public override bool HasScope => true;

        public override bool HasCamo => true;

        public override bool HasFlaslight => false;

        public override Dictionary<string, uint> MuzzlesAndSupps => new Dictionary<string, uint>()
        {
            { "None - $199", (uint)WeaponComponentHash.Invalid },
            { "Suppressor - $60000", (uint)WeaponComponentHash.AtSrSupp03 },
            { "Squared Muzzle - $45000", (uint)WeaponComponentHash.AtMuzzle08 },
            { "Bell-End Muzzle - $57000", (uint)WeaponComponentHash.AtMuzzle09 },
        };

        public override Dictionary<string, uint> Clips => new Dictionary<string, uint>()
        {
            { "Normal - $199", (uint)WeaponComponentHash.HeavySniperMk2Clip01 },
            { "Extended - $32000", (uint)WeaponComponentHash.HeavySniperMk2Clip02 },
            { "Armor piercing - $76000", (uint)WeaponComponentHash.HeavySniperMk2ClipArmorPiercing },
            { "Incendiary - $59000", (uint)WeaponComponentHash.HeavySniperMk2ClipIncendiary },
            { "FMJ - $88000", (uint)WeaponComponentHash.HeavySniperMk2ClipFMJ },
            { "Explosive - $115000", (uint)WeaponComponentHash.HeavySniperMk2ClipExplosive }
        };

        public override Dictionary<string, uint> Barrels => new Dictionary<string, uint>()
        {
            { "Standard - $299", (uint)WeaponComponentHash.AtSrBarrel01 },
            { "Heavy - $69000", (uint)WeaponComponentHash.AtSrBarrel02 }
        };
 
        public override Dictionary<string, uint> Grips => throw new NotImplementedException();

        public override Dictionary<string, uint> Scopes => new Dictionary<string, uint>()
        {
            { "Default - $199", (uint)WeaponComponentHash.AtScopeMax },
            { "Zoom - $29000", (uint)WeaponComponentHash.AtScopeLargeMk2 },
            { "Night Vision - $45000", (uint)WeaponComponentHash.AtScopeNV },
            { "Thermal - $69000", (uint)WeaponComponentHash.AtScopeThermal }
        };

        public override Dictionary<string, uint> Camos => new Dictionary<string, uint>()
        {
            { "None", (uint)WeaponComponentHash.Invalid },
            { "Digital", (uint)WeaponComponentHash.HeavySniperMk2Camo },
            { "Brushstroke", (uint)WeaponComponentHash.HeavySniperMk2Camo02 },
            { "Woodland", (uint)WeaponComponentHash.HeavySniperMk2Camo03 },
            { "Skull", (uint)WeaponComponentHash.HeavySniperMk2Camo04 },
            { "Sessanta Nove", (uint)WeaponComponentHash.HeavySniperMk2Camo05 },
            { "Perseus", (uint)WeaponComponentHash.HeavySniperMk2Camo06 },
            { "Leopard", (uint)WeaponComponentHash.HeavySniperMk2Camo07 },
            { "Zebra", (uint)WeaponComponentHash.HeavySniperMk2Camo08 },
            { "Geometric", (uint)WeaponComponentHash.HeavySniperMk2Camo09 },
            { "Boom!", (uint)WeaponComponentHash.HeavySniperMk2Camo10 },
            { "Patriotic", (uint)WeaponComponentHash.HeavySniperMk2CamoIndependence01 },

        };

        public override Dictionary<string, uint> FlashLight => throw new NotImplementedException();
    }
}
