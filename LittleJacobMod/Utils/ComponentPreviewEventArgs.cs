using System;
using GTA;

namespace LittleJacobMod.Utils
{
    public enum ComponentIndex
    {
        Camo,
        Scope,
        Clip,
        Muzzle,
        Flashlight,
        Barrel,
        Grip
    }

    public class ComponentPreviewEventArgs : EventArgs
    {
        public WeaponHash WeaponHash { get; private set; }
        public WeaponComponentHash PreviewComponent { get; private set; }
        public ComponentIndex ComponentIndex { get; private set; }

        public ComponentPreviewEventArgs(WeaponHash hash, WeaponComponentHash prvw, ComponentIndex index)
        {
            WeaponHash = hash;
            PreviewComponent = prvw;
            ComponentIndex = index;
        }
    }
}
