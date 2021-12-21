using System;
using System.Collections.Generic;
using GTA;

namespace LittleJacobMod.Utils.Weapons
{
    internal class PumpShotgunMk2 : Weapon
    {
        public override bool SaveFileWeapon => true;

        public override uint WeaponHash => (uint)GTA.WeaponHash.PumpShotgunMk2;

        public override int Price => 52000;

        public override string Name => "Pump Shotgun Mk2";

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
            { "Suppressor - $40000", (uint)WeaponComponentHash.AtSrSupp03 },
            { "Muzzle Brake - $29000", (uint)WeaponComponentHash.AtMuzzle08 },
        };

        public override Dictionary<string, uint> Clips => new Dictionary<string, uint>()
        {
            { "Normal - $199", (uint)WeaponComponentHash.PumpShotgunMk2Clip01 },
            { "Incendiary - $51000", (uint)WeaponComponentHash.PumpShotgunMk2ClipIncendiary },
            { "Armor piercing - $66000", (uint)WeaponComponentHash.PumpShotgunMk2ClipArmorPiercing },
            { "Hollow Point - $76000", (uint)WeaponComponentHash.PumpShotgunMk2ClipHollowPoint },
            { "Explosive - $82000", (uint)WeaponComponentHash.PumpShotgunMk2ClipExplosive },
        };

        public override Dictionary<string, uint> Barrels => throw new NotImplementedException();

        public override Dictionary<string, uint> Grips => throw new NotImplementedException();

        public override Dictionary<string, uint> Scopes => new Dictionary<string, uint>()
        {
            { "None - $199", (uint)WeaponComponentHash.Invalid },
            { "Holographic - $19000", (uint)WeaponComponentHash.AtSights },
            { "Small - $23000", (uint)WeaponComponentHash.AtScopeMacroMk2 },
            { "Medium - $34000", (uint)WeaponComponentHash.AtScopeSmallMk2 },
        };

        public override Dictionary<string, uint> Camos => new Dictionary<string, uint>()
        {
            { "None", (uint)WeaponComponentHash.Invalid },
            { "Digital", (uint)WeaponComponentHash.PumpShotgunMk2Camo },
            { "Brushstroke", (uint)WeaponComponentHash.PumpShotgunMk2Camo02 },
            { "Woodland", (uint)WeaponComponentHash.PumpShotgunMk2Camo03 },
            { "Skull", (uint)WeaponComponentHash.PumpShotgunMk2Camo04 },
            { "Sessanta Nove", (uint)WeaponComponentHash.PumpShotgunMk2Camo05 },
            { "Perseus", (uint)WeaponComponentHash.PumpShotgunMk2Camo06 },
            { "Leopard", (uint)WeaponComponentHash.PumpShotgunMk2Camo07 },
            { "Zebra", (uint)WeaponComponentHash.PumpShotgunMk2Camo08 },
            { "Geometric", (uint)WeaponComponentHash.PumpShotgunMk2Camo09 },
            { "Boom!", (uint)WeaponComponentHash.PumpShotgunMk2Camo10 },
            { "Patriotic", (uint)WeaponComponentHash.PumpShotgunMk2CamoIndependence01 },

        };

        public override Dictionary<string, uint> FlashLight => new Dictionary<string, uint>()
        {
            { "None - $199", (uint)WeaponComponentHash.Invalid },
            { "Flashlight - $10000", (uint)WeaponComponentHash.AtArFlsh }
        };
    }
}
