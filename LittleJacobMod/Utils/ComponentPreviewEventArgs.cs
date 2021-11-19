using System;
using GTA;

namespace LittleJacobMod.Utils
{
    public class ComponentPreviewEventArgs : EventArgs
    {
        public WeaponHash WeaponHash { get; private set; }
        public WeaponComponentHash InstalledComponent { get; private set; }
        public WeaponComponentHash PreviewComponent { get; private set; }

        public ComponentPreviewEventArgs(WeaponHash hash, WeaponComponentHash installed, WeaponComponentHash prvw)
        {
            WeaponHash = hash;
            InstalledComponent = installed;
            PreviewComponent = prvw;
        }
    }
}
