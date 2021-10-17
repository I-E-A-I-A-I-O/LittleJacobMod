using System;

namespace LittleJacobMod.Utils
{
    internal class WeaponComponentItem
    {
        int index;
        string displayName;
        public int Index => index;
        public string DisplayName => displayName;

        public WeaponComponentItem(int index, string displayName)
        {
            this.index = index;
            this.displayName = displayName;
        }
    }
}
