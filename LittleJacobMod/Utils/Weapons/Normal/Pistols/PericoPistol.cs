using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleJacobMod.Utils.Weapons
{
    internal class PericoPistol : Weapon
    {
        public override bool SaveFileWeapon => true;

        public override GTA.WeaponHash WeaponHash => GTA.WeaponHash.PericoPistol;

        public override int Price => 50000;

        public override string Name => "Perico pistol";

        public override bool HasMuzzleOrSupp => false;

        public override bool HasClip => false;

        public override bool HasBarrel => false;

        public override bool HasGrip => false;

        public override bool HasScope => false;

        public override bool HasCamo => false;

        public override bool HasFlaslight => false;

        public override Dictionary<string, GTA.WeaponComponentHash> MuzzlesAndSupps => throw new NotImplementedException();

        public override Dictionary<string, GTA.WeaponComponentHash> Clips => throw new NotImplementedException();

        public override Dictionary<string, GTA.WeaponComponentHash> Barrels => throw new NotImplementedException();

        public override Dictionary<string, GTA.WeaponComponentHash> Grips => throw new NotImplementedException();

        public override Dictionary<string, GTA.WeaponComponentHash> Scopes => throw new NotImplementedException();

        public override Dictionary<string, GTA.WeaponComponentHash> Camos => throw new NotImplementedException();

        public override Dictionary<string, GTA.WeaponComponentHash> FlashLight => throw new NotImplementedException();
    }
}
