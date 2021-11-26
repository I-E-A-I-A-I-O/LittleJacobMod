using System;
using System.Collections.Generic;
using GTA;

namespace LittleJacobMod.Utils.Weapons
{
    internal class PistolMk2 : Weapon
    {
        public override bool SaveFileWeapon => true;

        public override WeaponHash WeaponHash => WeaponHash.PistolMk2;

        public override int Price => 65000;

        public override string Name => "Pistol Mk2";

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
            { "Compensator - $21000", WeaponComponentHash.AtPiComp }
        };

        public override Dictionary<string, WeaponComponentHash> Clips => new Dictionary<string, WeaponComponentHash>()
        {
            { "Normal - $199" , WeaponComponentHash.PistolMk2Clip01 },
            { "Extended - $15000", WeaponComponentHash.PistolMk2Clip02 },
            { "Tracer - $28000", WeaponComponentHash.PistolMk2ClipTracer },
            { "Incendiary - $34000", WeaponComponentHash.PistolMk2ClipIncendiary },
            { "Hollow Point - $39000", WeaponComponentHash.PistolMk2ClipHollowPoint },
            { "FMJ - $52000", WeaponComponentHash.PistolMk2ClipFMJ }
        };

        public override Dictionary<string, WeaponComponentHash> Barrels => throw new NotImplementedException();

        public override Dictionary<string, WeaponComponentHash> Grips => throw new NotImplementedException();

        public override Dictionary<string, WeaponComponentHash> Scopes => new Dictionary<string, WeaponComponentHash>()
        {
            { "None - $199", WeaponComponentHash.Invalid },
            { "Mounted Scope - $16000", WeaponComponentHash.AtPiRail }
        };

        public override Dictionary<string, WeaponComponentHash> Camos => new Dictionary<string, WeaponComponentHash>()
        {
            { "None", WeaponComponentHash.Invalid },
            { "Digital", WeaponComponentHash.PistolMk2Camo },
            { "Brushstroke", WeaponComponentHash.PistolMk2Camo02 },
            { "Woodland", WeaponComponentHash.PistolMk2Camo03 },
            { "Skull", WeaponComponentHash.PistolMk2Camo04 },
            { "Sessanta Nove", WeaponComponentHash.PistolMk2Camo05 },
            { "Perseus", WeaponComponentHash.PistolMk2Camo06 },
            { "Leopard", WeaponComponentHash.PistolMk2Camo07 },
            { "Zebra", WeaponComponentHash.PistolMk2Camo08 },
            { "Geometric", WeaponComponentHash.PistolMk2Camo09 },
            { "Boom!", WeaponComponentHash.PistolMk2Camo10 },
            { "Patriotic", WeaponComponentHash.PistolMk2CamoIndependence01 },
        };

        public override Dictionary<string, WeaponComponentHash> FlashLight => new Dictionary<string, WeaponComponentHash>()
        {
            { "None - $199", WeaponComponentHash.Invalid },
            { "Flashlight - $7000", WeaponComponentHash.AtPiFlsh02 }
        };
    }
}
