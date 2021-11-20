using System;
using System.Collections.Generic;
using GTA;

namespace LittleJacobMod.Utils.Weapons
{
    internal class SNSPistolMk2 : Weapon
    {
        public override bool SaveFileWeapon => true;

        public override WeaponHash WeaponHash => WeaponHash.SNSPistolMk2;

        public override int Price => 79000;

        public override string Name => "SNS Pistol Mk2";

        public override bool HasMuzzleOrSupp => true;

        public override bool HasClip => true;

        public override bool HasBarrel => false;

        public override bool HasGrip => false;

        public override bool HasScope => true;

        public override bool HasCamo => true;

        public override bool HasFlaslight => true;

        public override Dictionary<string, WeaponComponentHash> MuzzlesAndSupps => new Dictionary<string, WeaponComponentHash>()
        {
            { "None - $199", WeaponComponentHash.Invalid },
            { "Suppressor - $28000", WeaponComponentHash.AtPiSupp02 },
            { "Compensator - $21000", WeaponComponentHash.AtPiComp02 }
        };

        public override Dictionary<string, WeaponComponentHash> Clips => new Dictionary<string, WeaponComponentHash>()
        {
            { "Normal - $199" , WeaponComponentHash.SNSPistolMk2Clip01 },
            { "Extended - $15000", WeaponComponentHash.SNSPistolMk2Clip02 },
            { "Tracer - $28000", WeaponComponentHash.SNSPistolMk2ClipTracer },
            { "Incendiary - $34000", WeaponComponentHash.SNSPistolMk2ClipIncendiary },
            { "Hollow Point - $39000", WeaponComponentHash.SNSPistolMk2ClipHollowPoint },
            { "FMJ - $52000", WeaponComponentHash.SNSPistolMk2ClipFMJ }
        };

        public override Dictionary<string, WeaponComponentHash> Barrels => throw new NotImplementedException();

        public override Dictionary<string, WeaponComponentHash> Grips => throw new NotImplementedException();

        public override Dictionary<string, WeaponComponentHash> Scopes => new Dictionary<string, WeaponComponentHash>()
        {
            { "None - $199", WeaponComponentHash.Invalid },
            { "Mounted Scope - $16000", WeaponComponentHash.AtPiRail02 }
        };

        public override Dictionary<string, WeaponComponentHash> Camos => new Dictionary<string, WeaponComponentHash>()
        {
            { "None", WeaponComponentHash.Invalid },
            { "Digital", WeaponComponentHash.SNSPistolMk2Camo },
            { "Brushstroke", WeaponComponentHash.SNSPistolMk2Camo02 },
            { "Woodland", WeaponComponentHash.SNSPistolMk2Camo03 },
            { "Skull", WeaponComponentHash.SNSPistolMk2Camo04 },
            { "Sessanta Nove", WeaponComponentHash.SNSPistolMk2Camo05 },
            { "Perseus", WeaponComponentHash.SNSPistolMk2Camo06 },
            { "Leopard", WeaponComponentHash.SNSPistolMk2Camo07 },
            { "Zebra", WeaponComponentHash.SNSPistolMk2Camo08 },
            { "Geometric", WeaponComponentHash.SNSPistolMk2Camo09 },
            { "Boom!", WeaponComponentHash.SNSPistolMk2Camo10 },
            { "Patriotic", WeaponComponentHash.SNSPistolMk2CamoIndependence01 },

        };

        public override Dictionary<string, WeaponComponentHash> FlashLight => new Dictionary<string, WeaponComponentHash>()
        {
            { "None - $199", WeaponComponentHash.Invalid },
            { "Flashlight - $7000", WeaponComponentHash.AtPiFlsh03 }
        };
    }
}
