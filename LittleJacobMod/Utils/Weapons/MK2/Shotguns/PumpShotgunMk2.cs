using System;
using System.Collections.Generic;
using GTA;

namespace LittleJacobMod.Utils.Weapons
{
    internal class PumpShotgunMk2 : Weapon
    {
        public override bool SaveFileWeapon => true;

        public override WeaponHash WeaponHash => WeaponHash.PumpShotgunMk2;

        public override int Price => 52000;

        public override string Name => "Pump Shotgun Mk2";

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
            { "Suppressor - $40000", WeaponComponentHash.AtSrSupp03 },
            { "Muzzle Brake - $29000", WeaponComponentHash.AtMuzzle08 },
        };

        public override Dictionary<string, WeaponComponentHash> Clips => new Dictionary<string, WeaponComponentHash>()
        {
            { "Normal - $199", WeaponComponentHash.PumpShotgunMk2Clip01 },
            { "Incendiary - $51000", WeaponComponentHash.PumpShotgunMk2ClipIncendiary },
            { "Armor piercing - $66000", WeaponComponentHash.PumpShotgunMk2ClipArmorPiercing },
            { "Hollow Point - $76000", WeaponComponentHash.PumpShotgunMk2ClipHollowPoint },
            { "Explosive - $82000", WeaponComponentHash.PumpShotgunMk2ClipExplosive },
        };

        public override Dictionary<string, WeaponComponentHash> Barrels => throw new NotImplementedException();

        public override Dictionary<string, WeaponComponentHash> Grips => throw new NotImplementedException();

        public override Dictionary<string, WeaponComponentHash> Scopes => new Dictionary<string, WeaponComponentHash>()
        {
            { "None - $199", WeaponComponentHash.Invalid },
            { "Holographic - $19000", WeaponComponentHash.AtSights },
            { "Small - $23000", WeaponComponentHash.AtScopeMacroMk2 },
            { "Medium - $34000", WeaponComponentHash.AtScopeSmallMk2 },
        };

        public override Dictionary<string, WeaponComponentHash> Camos => new Dictionary<string, WeaponComponentHash>()
        {
            { "None", WeaponComponentHash.Invalid },
            { "Digital", WeaponComponentHash.PumpShotgunMk2Camo },
            { "Brushstroke", WeaponComponentHash.PumpShotgunMk2Camo02 },
            { "Woodland", WeaponComponentHash.PumpShotgunMk2Camo03 },
            { "Skull", WeaponComponentHash.PumpShotgunMk2Camo04 },
            { "Sessanta Nove", WeaponComponentHash.PumpShotgunMk2Camo05 },
            { "Perseus", WeaponComponentHash.PumpShotgunMk2Camo06 },
            { "Leopard", WeaponComponentHash.PumpShotgunMk2Camo07 },
            { "Zebra", WeaponComponentHash.PumpShotgunMk2Camo08 },
            { "Geometric", WeaponComponentHash.PumpShotgunMk2Camo09 },
            { "Boom!", WeaponComponentHash.PumpShotgunMk2Camo10 },
            { "Patriotic", WeaponComponentHash.PumpShotgunMk2CamoIndependence01 },

        };

        public override Dictionary<string, WeaponComponentHash> FlashLight => new Dictionary<string, WeaponComponentHash>()
        {
            { "None - $199", WeaponComponentHash.Invalid },
            { "Flashlight - $10000", WeaponComponentHash.AtArFlsh }
        };
    }
}
