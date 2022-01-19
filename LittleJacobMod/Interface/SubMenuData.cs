using System;
using System.Collections.Generic;
using System.Linq;
using GTA;
using LemonUI.Menus;
using LittleJacobMod.Saving;

namespace LittleJacobMod.Interface
{
    internal struct ItemData
    {
        public int Price;
        public NativeItem Item;
        public uint Hash;
    }

    internal class SubMenuData
    {
        private readonly uint _weapon;
        public List<ItemData> TintItems { get; } = new List<ItemData>();
        public List<ItemData> CamoColorItems { get; } = new List<ItemData>();
        public List<ItemData> CamoItems { get; } = new List<ItemData>();
        public List<ItemData> ClipItems { get; } = new List<ItemData>();
        public List<ItemData> MuzzleItems { get; } = new List<ItemData>();
        public List<ItemData> BarrelItems { get; } = new List<ItemData>();
        public List<ItemData> GripItems { get; } = new List<ItemData>();
        public List<ItemData> ScopeItems { get; } = new List<ItemData>();
        public List<ItemData> FlashlightItems { get; } = new List<ItemData>();
        public List<ItemData> VarmodItems { get; } = new List<ItemData>();

        public SubMenuData(uint weapon)
        {
            _weapon = weapon;
        }

        public void ClearLists()
        {
            TintItems.Clear();
            ClipItems.Clear();
            MuzzleItems.Clear();
            BarrelItems.Clear();
            GripItems.Clear();
            ScopeItems.Clear();
            CamoItems.Clear();
            FlashlightItems.Clear();
            CamoItems.Clear();
            VarmodItems.Clear();
        }

        private static void Restart(IReadOnlyCollection<ItemData> items, string text)
        {
            for (int i = 0; i < items.Count; i++)
            {
                ItemData data = items.ElementAt(i);

                if (i == 0)
                {
                    data.Item.Enabled = false;
                    data.Item.Description = $"Current {text}";
                    continue;
                }

                data.Item.Enabled = true;
                data.Item.Description = $"Price: ${data.Price.ToString()}";
            }
        }

        public static void SetIndex(List<ItemData> items, string text, int index)
        {
            if (index == -1) return;
            for (int i = 0; i < items.Count; i++)
            {
                ItemData data = items.ElementAt(i);

                if (i != index && !data.Item.Enabled)
                {
                    data.Item.Enabled = true;
                    data.Item.Description = $"Price: ${data.Price.ToString()}";
                }
                else if (i == index)
                {
                    data.Item.Enabled = false;
                    data.Item.Description = $"Current {text}";
                }
            }
        }

        private void RestartLists()
        {
            Restart(TintItems, "Tint");
            Restart(ClipItems, "Clip");
            Restart(MuzzleItems, "Muzzle attachment");
            Restart(BarrelItems, "Barrel");
            Restart(GripItems, "Grip");
            Restart(ScopeItems, "Scope");
            Restart(CamoItems, "Livery");
            Restart(FlashlightItems, "Flashlight");
            Restart(CamoColorItems, "Livery color");
            Restart(VarmodItems, "Finish");
        }

        public void LoadAttachments()
        {
            RestartLists();
            var storeRef = LoadoutSaving.GetStoreReference(_weapon);

            if (storeRef == null)
            {
                return;
            }

            int index;

            if (TintItems.Count > 0)
            {
                index = storeRef.GetTintIndex();
                SetIndex(TintItems, "Tint", index);
            }

            if (BarrelItems.Count > 0)
            {
                index = BarrelItems.FindIndex((it) => it.Hash == storeRef.Barrel);
                SetIndex(BarrelItems, "Barrel", index);
            }

            if (ClipItems.Count > 0)
            {
                index = ClipItems.FindIndex((it) => it.Hash == storeRef.Clip);
                SetIndex(ClipItems, "Clip", index);
            }

            if (FlashlightItems.Count > 0)
            {
                index = FlashlightItems.FindIndex((it) => it.Hash == storeRef.Flashlight);
                SetIndex(FlashlightItems, "Flashlight", index);
            }
            
            if (GripItems.Count > 0)
            {
                index = GripItems.FindIndex((it) => it.Hash == storeRef.Grip);
                SetIndex(GripItems, "Grip", index);
            }
            
            if (MuzzleItems.Count > 0)
            {
                index = MuzzleItems.FindIndex((it) => it.Hash == storeRef.Muzzle);
                SetIndex(MuzzleItems, "Muzzle attachment", index);
            }

            if (ScopeItems.Count > 0)
            {
                index = ScopeItems.FindIndex((it) => it.Hash == storeRef.Scope);
                SetIndex(ScopeItems, "Scope", index);
            }

            if (CamoItems.Count > 0)
            {
                index = CamoItems.FindIndex((it) => it.Hash == storeRef.Camo);
                SetIndex(CamoItems, "Livery", index);
                index = storeRef.GetCamoColor();
                SetIndex(CamoColorItems, "Livery Color", index);
            }

            if (VarmodItems.Count <= 0) return;
            {
                index = VarmodItems.FindIndex((it) => it.Hash == storeRef.Varmod);
                SetIndex(VarmodItems, "Finish", index);
            }
        }
    }
}
