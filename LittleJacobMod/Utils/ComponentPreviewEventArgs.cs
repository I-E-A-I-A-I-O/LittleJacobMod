using System;

namespace LittleJacobMod.Utils
{
    public class ComponentPreviewEventArgs : EventArgs
    {
        public uint WeaponHash { get; }
        public uint PreviewComponent { get; }
        public string ComponentIndex { get; }

        public ComponentPreviewEventArgs(uint hash, uint prvw, string componentIndex)
        {
            WeaponHash = hash;
            PreviewComponent = prvw;
            ComponentIndex = componentIndex;
        }
    }
}
