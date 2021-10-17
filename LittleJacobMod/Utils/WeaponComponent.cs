using System;
using GTA;

namespace LittleJacobMod.Utils
{
    internal class WeaponComponentItem
    {
        public int Index { get; }
        public string DisplayName { get; }
        public WeaponComponentHash WeaponComponentHash { get; }

        public WeaponComponentItem(int index, string displayName, WeaponComponentHash componentHash)
        {
            Index = index;
            DisplayName = displayName;
            WeaponComponentHash = componentHash;
        }
    }
}
