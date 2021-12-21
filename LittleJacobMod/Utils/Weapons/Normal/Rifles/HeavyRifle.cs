using System;
using System.Collections.Generic;
using GTA;

namespace LittleJacobMod.Utils.Weapons
{
    class HeavyRifle : Weapon
    {
        public override bool SaveFileWeapon => true;

        public override uint WeaponHash => 3347935668;

        public override int Price => 400000;

        public override string Name => "Heavy Rifle";

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
            { "Suppressor - $49000", (uint)WeaponComponentHash.AtArSupp }
        };

        public override Dictionary<string, uint> Clips => new Dictionary<string, uint>()
        {
            { "Normal - $199", 1525977990 },
            { "Extended - $50000", 1824470811 }
        };

        public override Dictionary<string, uint> Barrels => throw new NotImplementedException();

        public override Dictionary<string, uint> Grips => new Dictionary<string, uint>()
        {
            { "None - $199", (uint)WeaponComponentHash.Invalid },
            { "Grip - $8000", (uint)WeaponComponentHash.AtArAfGrip }
        };

        public override Dictionary<string, uint> Scopes => new Dictionary<string, uint>()
        {
            { "Iron sights - $199", 3017917522 },
            { "Scope - $30000", (uint)WeaponComponentHash.AtScopeMedium },
        };

        public override Dictionary<string, uint> Camos => new Dictionary<string, uint>()
        {
            { "None", (uint)WeaponComponentHash.Invalid },
            { "Families Finish", 3969903833 },
        };

        public override Dictionary<string, uint> FlashLight => new Dictionary<string, uint>()
        {
            { "None - $199", (uint)WeaponComponentHash.Invalid },
            { "Flashlight - $10000", (uint)WeaponComponentHash.AtArFlsh }
        };
    }
}
