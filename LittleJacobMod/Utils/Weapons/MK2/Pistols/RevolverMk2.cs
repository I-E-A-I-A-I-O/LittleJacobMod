using System;
using System.Collections.Generic;
using GTA;

namespace LittleJacobMod.Utils.Weapons
{
    internal class RevolverMk2 : Weapon
    {
        public override bool SaveFileWeapon => true;

        public override uint WeaponHash => (uint)GTA.WeaponHash.RevolverMk2;

        public override int Price => 91000;

        public override string Name => "Heavy Revolver Mk2";

        public override bool HasMuzzleOrSupp => true;

        public override bool HasClip => true;

        public override bool HasBarrel => false;

        public override bool HasGrip => false;

        public override bool HasScope => true;

        public override bool HasCamo => true;

        public override bool HasFlaslight => false;

        public override Dictionary<string, uint> MuzzlesAndSupps => new Dictionary<string, uint>()
        {
            { "None - $199", (uint)WeaponComponentHash.Invalid },
            { "Compensator - $21000", (uint)WeaponComponentHash.AtPiComp03 }
        };

        public override Dictionary<string, uint> Clips => new Dictionary<string, uint>()
        {
            { "Normal - $199" , (uint)WeaponComponentHash.RevolverMk2Clip01 },
            { "Tracer - $28000", (uint)WeaponComponentHash.RevolverMk2ClipTracer },
            { "Incendiary - $34000", (uint)WeaponComponentHash.RevolverMk2ClipIncendiary },
            { "Hollow Point - $39000", (uint)WeaponComponentHash.RevolverMk2ClipHollowPoint },
            { "FMJ - $52000", (uint)WeaponComponentHash.RevolverMk2ClipFMJ }
        };

        public override Dictionary<string, uint> Barrels => throw new NotImplementedException();

        public override Dictionary<string, uint> Grips => throw new NotImplementedException();

        public override Dictionary<string, uint> Scopes => new Dictionary<string, uint>()
        {
            { "None - $199", (uint)WeaponComponentHash.Invalid },
            { "Holographic - $16000", (uint)WeaponComponentHash.AtSights },
            { "Small scope - $25000", (uint)WeaponComponentHash.AtScopeMacroMk2 }
        };

        public override Dictionary<string, uint> Camos => new Dictionary<string, uint>()
        {
            { "None", (uint)WeaponComponentHash.Invalid },
            { "Digital", (uint)WeaponComponentHash.RevolverMk2Camo },
            { "Brushstroke", (uint)WeaponComponentHash.RevolverMk2Camo02 },
            { "Woodland", (uint)WeaponComponentHash.RevolverMk2Camo03 },
            { "Skull", (uint)WeaponComponentHash.RevolverMk2Camo04 },
            { "Sessanta Nove", (uint)WeaponComponentHash.RevolverMk2Camo05 },
            { "Perseus", (uint)WeaponComponentHash.RevolverMk2Camo06 },
            { "Leopard", (uint)WeaponComponentHash.RevolverMk2Camo07 },
            { "Zebra", (uint)WeaponComponentHash.RevolverMk2Camo08 },
            { "Geometric", (uint)WeaponComponentHash.RevolverMk2Camo09 },
            { "Boom!", (uint)WeaponComponentHash.RevolverMk2Camo10 },
            { "Patriotic", (uint)WeaponComponentHash.RevolverMk2CamoIndependence01 },

        };

        public override Dictionary<string, uint> FlashLight => throw new NotImplementedException();
    }
}
