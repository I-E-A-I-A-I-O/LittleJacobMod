using System;
using GTA;

namespace LittleJacobMod.Utils
{
    public enum ComponentIndex
    {
        Livery,
        Scope,
        Clip,
        Muzzle,
        Flashlight,
        Barrel,
        Grip
    }

    public class ComponentPreviewEventArgs : EventArgs
    {
        public uint WeaponHash { get; private set; }
        public uint PreviewComponent { get; private set; }
        public ComponentIndex ComponentIndex { get; private set; }

        public ComponentPreviewEventArgs(uint hash, uint prvw, ComponentIndex index)
        {
            WeaponHash = hash;
            PreviewComponent = prvw;
            ComponentIndex = index;
        }
    }
}
