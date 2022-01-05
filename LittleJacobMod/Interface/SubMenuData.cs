using System;
using System.Collections.Generic;
using System.Linq;
using GTA;
using LemonUI.Menus;
using LittleJacobMod.Saving;

namespace LittleJacobMod.Interface
{
    struct ItemData
    {
        public int price;
        public NativeItem item;
        public uint Hash;
    }
    class SubMenuData
    {
        public uint Weapon;
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
            Weapon = weapon;
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
        }

        void Restart(List<ItemData> items, string text)
        {
            for (int i = 0; i < items.Count; i++)
            {
                ItemData data = items.ElementAt(i);

                if (i == 0)
                {
                    data.item.Enabled = false;
                    data.item.Description = $"Current {text}";
                    continue;
                }

                data.item.Enabled = true;
                data.item.Description = $"Price: ${data.price}";
            }
        }

        public void SetIndex(List<ItemData> items, string text, int index)
        {
            if (index != -1)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    ItemData data = items.ElementAt(i);

                    if (i != index && !data.item.Enabled)
                    {
                        data.item.Enabled = true;
                        data.item.Description = $"Price: ${data.price}";
                    }
                    else if (i == index)
                    {
                        data.item.Enabled = false;
                        data.item.Description = $"Current {text}";
                    }
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
        }

        public void LoadAttachments()
        {
            RestartLists();
            Saving.Utils.StoredWeapon storeRef = LoadoutSaving.GetStoreReference(Weapon);

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
                index = weapon.Barrels.Values.ToList().IndexOf(storeRef.Barrel);
                SetDictIndex(BarrelItems, "Barrel", index);
            }

            if (weapon.HasClip)
            {
                index = weapon.Clips.Values.ToList().IndexOf(storeRef.Clip);
                SetDictIndex(ClipItems, "Clip", index);
            }

            if (weapon.HasFlaslight)
            {
                index = weapon.FlashLight.Values.ToList().IndexOf(storeRef.Flashlight);
                SetDictIndex(FlashlightItems, "Flashlight", index);
            }
            
            if (weapon.HasGrip)
            {
                index = weapon.Grips.Values.ToList().IndexOf(storeRef.Grip);
                SetDictIndex(GripItems, "Grip", index);
            }
            
            if (weapon.HasMuzzleOrSupp)
            {
                index = weapon.MuzzlesAndSupps.Values.ToList().IndexOf(storeRef.Muzzle);
                SetDictIndex(MuzzleItems, "Muzzle attachment", index);
            }

            if (weapon.HasScope)
            {
                index = weapon.Scopes.Values.ToList().IndexOf(storeRef.Scope);
                SetDictIndex(ScopeItems, "Scope", index);
            }

            if (weapon.HasCamo)
            {
                index = weapon.Camos.Values.ToList().IndexOf(storeRef.Camo);
                SetListIndex(CamoItems, "Livery", 60000, index);
                index = storeRef.GetCamoColor();
                SetListIndex(CamoColorItems, "Livery Color", 10000, index);
            }
        }
    }
}
