using System;
using System.Collections.Generic;
using GTA;

namespace LittleJacobMod.Utils.Weapons
{
    internal class SNSPistolMk2 : Weapon
    {
        public override bool SaveFileWeapon => true;

        public override uint WeaponHash => (uint)GTA.WeaponHash.SNSPistolMk2;

        public override int Price => 71000;

        public override string Name => "SNS Pistol Mk2";

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
            { "Suppressor - $28000", (uint)WeaponComponentHash.AtPiSupp02 },
            { "Compensator - $21000", (uint)WeaponComponentHash.AtPiComp02 }
        };

        public override Dictionary<string, uint> Clips => new Dictionary<string, uint>()
        {
            { "Normal - $199" , (uint)WeaponComponentHash.SNSPistolMk2Clip01 },
            { "Extended - $15000", (uint)WeaponComponentHash.SNSPistolMk2Clip02 },
            { "Tracer - $28000", (uint)WeaponComponentHash.SNSPistolMk2ClipTracer },
            { "Incendiary - $34000", (uint)WeaponComponentHash.SNSPistolMk2ClipIncendiary },
            { "Hollow Point - $39000", (uint)WeaponComponentHash.SNSPistolMk2ClipHollowPoint },
            { "FMJ - $52000", (uint)WeaponComponentHash.SNSPistolMk2ClipFMJ }
        };

        public override Dictionary<string, uint> Barrels => throw new NotImplementedException();

        public override Dictionary<string, uint> Grips => throw new NotImplementedException();

        public override Dictionary<string, uint> Scopes => new Dictionary<string, uint>()
        {
            { "None - $199", (uint)WeaponComponentHash.Invalid },
            { "Mounted Scope - $16000", (uint)WeaponComponentHash.AtPiRail02 }
        };

        public override Dictionary<string, uint> Camos => new Dictionary<string, uint>()
        {
            { "None", (uint)WeaponComponentHash.Invalid },
            { "Digital", (uint)WeaponComponentHash.SNSPistolMk2Camo },
            { "Brushstroke", (uint)WeaponComponentHash.SNSPistolMk2Camo02 },
            { "Woodland", (uint)WeaponComponentHash.SNSPistolMk2Camo03 },
            { "Skull", (uint)WeaponComponentHash.SNSPistolMk2Camo04 },
            { "Sessanta Nove", (uint)WeaponComponentHash.SNSPistolMk2Camo05 },
            { "Perseus", (uint)WeaponComponentHash.SNSPistolMk2Camo06 },
            { "Leopard", (uint)WeaponComponentHash.SNSPistolMk2Camo07 },
            { "Zebra", (uint)WeaponComponentHash.SNSPistolMk2Camo08 },
            { "Geometric", (uint)WeaponComponentHash.SNSPistolMk2Camo09 },
            { "Boom!", (uint)WeaponComponentHash.SNSPistolMk2Camo10 },
            { "Patriotic", (uint)WeaponComponentHash.SNSPistolMk2CamoIndependence01 },

        };

        public override Dictionary<string, uint> FlashLight => new Dictionary<string, uint>()
        {
            { "None - $199", (uint)WeaponComponentHash.Invalid },
            { "Flashlight - $7000", (uint)WeaponComponentHash.AtPiFlsh03 }
        };
    }
}
