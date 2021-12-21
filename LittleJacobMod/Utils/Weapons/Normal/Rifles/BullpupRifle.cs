using System;
using System.Collections.Generic;
using GTA;

namespace LittleJacobMod.Utils.Weapons
{
    internal class BullpupRifle : Weapon
    {
        public override bool SaveFileWeapon => true;

        public override uint WeaponHash => (uint)GTA.WeaponHash.BullpupRifle;

        public override int Price => 12000;

        public override string Name => "Bullpup Rifle";

        public override bool HasMuzzleOrSupp => true;

        public override bool HasClip => true;

        public override bool HasBarrel => false;

        public override bool HasGrip => true;

        public override bool HasScope => true;

        public override bool HasCamo => true;

        public override bool HasFlaslight => true;

        public override Dictionary<string, uint> MuzzlesAndSupps => new Dictionary<string, uint>()
        {
            { "None - $199", (uint)WeaponComponentHash.Invalid },
            { "Supressor - $10000", (uint)WeaponComponentHash.AtArSupp }
        };

        public override Dictionary<string, uint> Clips => new Dictionary<string, uint>()
        {
            { "Normal - $199", (uint)WeaponComponentHash.BullpupRifleClip01 },
            { "Extended - $12000", (uint)WeaponComponentHash.BullpupRifleClip02 }
        };

        public override Dictionary<string, uint> Barrels => throw new NotImplementedException();

        public override Dictionary<string, uint> Grips => new Dictionary<string, uint>()
        {
            { "None - $199", (uint)WeaponComponentHash.Invalid },
            { "Grip - $5000", (uint)WeaponComponentHash.AtArAfGrip }
        };

        public override Dictionary<string, uint> Scopes => new Dictionary<string, uint>()
        {
            { "None - $199", (uint)WeaponComponentHash.Invalid },
            { "Scope - $7000", (uint)WeaponComponentHash.AtScopeSmall }
        };

        public override Dictionary<string, uint> Camos => new Dictionary<string, uint>()
        {
            { "None", (uint)WeaponComponentHash.Invalid },
            { "Luxury Finish", (uint)WeaponComponentHash.BullpupRifleVarmodLow },
        };

        public override Dictionary<string, uint> FlashLight => new Dictionary<string, uint>()
        {
            { "None - $199", (uint)WeaponComponentHash.Invalid },
            { "Flashlight - $3000", (uint)WeaponComponentHash.AtArFlsh }
        };
    }
}
