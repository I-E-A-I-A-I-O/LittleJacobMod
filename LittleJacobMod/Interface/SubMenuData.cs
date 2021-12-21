using System;
using System.Collections.Generic;
using System.Linq;
using GTA;
using LemonUI.Menus;
using LittleJacobMod.Saving;
using LittleJacobMod.Utils.Weapons;

namespace LittleJacobMod.Interface
{
    class SubMenuData
    {
        public uint Weapon;
        public List<NativeItem> TintItems { get; } = new List<NativeItem>();
        public List<NativeItem> CamoColorItems { get; } = new List<NativeItem>();
        public List<NativeItem> CamoItems { get; } = new List<NativeItem>();
        public Dictionary<string, NativeItem> ClipItems { get; } = new Dictionary<string, NativeItem>();
        public Dictionary<string, NativeItem> MuzzleItems { get; } = new Dictionary<string, NativeItem>();
        public Dictionary<string, NativeItem> BarrelItems { get; } = new Dictionary<string, NativeItem>();
        public Dictionary<string, NativeItem> GripItems { get; } = new Dictionary<string, NativeItem>();
        public Dictionary<string, NativeItem> ScopeItems { get; } = new Dictionary<string, NativeItem>();
        public Dictionary<string, NativeItem> FlashlightItems { get; } = new Dictionary<string, NativeItem>();

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

        void RestartDict(Dictionary<string, NativeItem> items, string text)
        {
            for (int i = 0; i < items.Count; i++)
            {
                KeyValuePair<string, NativeItem> item = items.ElementAt(i);

                if (i == 0)
                {
                    item.Value.Enabled = false;
                    item.Value.Description = $"Current {text}";
                    continue;
                }

                item.Value.Enabled = true;
                item.Value.Description = $"Price: ${item.Key}";
            }
        }

        void RestartList(List<NativeItem> items, string text, int price)
        {
            for (int i = 0; i < items.Count; i++)
            {
                NativeItem item = items.ElementAt(i);

                if (i == 0)
                {
                    item.Enabled = false;
                    item.Description = $"Current {text}";
                    continue;
                }

                item.Enabled = true;
                item.Description = $"Price: ${price}";
            }
        }

        public void SetDictIndex(Dictionary<string, NativeItem> items, string text, int index)
        {
            if (index != -1)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (i != index && !items.Values.ElementAt(i).Enabled)
                    {
                        items.Values.ElementAt(i).Enabled = true;
                        items.Values.ElementAt(i).Description = $"Price: ${items.ElementAt(i).Key}";
                    }
                    else if (i == index)
                    {
                        items.Values.ElementAt(i).Enabled = false;
                        items.Values.ElementAt(i).Description = $"Current {text}";
                    }
                }
            }
        }

        public void SetListIndex(List<NativeItem> items, string text, int price, int index)
        {
            if (index != -1)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (i != index && !items.ElementAt(i).Enabled)
                    {
                        items.ElementAt(i).Enabled = true;
                        items.ElementAt(i).Description = $"Price: ${price}";
                    }
                    else if (i == index)
                    {
                        items.ElementAt(i).Enabled = false;
                        items.ElementAt(i).Description = $"Current {text}";
                    }
                }
            }
        }

        public void RestartLists()
        {
            RestartList(TintItems, "Tint", 5000);
            RestartDict(ClipItems, "Clip");
            RestartDict(MuzzleItems, "Muzzle attachment");
            RestartDict(BarrelItems, "Barrel");
            RestartDict(GripItems, "Grip");
            RestartDict(ScopeItems, "Scope");
            RestartList(CamoItems, "Livery", 60000);
            RestartDict(FlashlightItems, "Flashlight");
            RestartList(CamoColorItems, "Livery color", 10000);
        }

        public void LoadAttachments()
        {
            var weapon = WeaponsList.GetWeapon(Weapon);
            var storeRef = LoadoutSaving.GetStoreReference(Weapon);

            if (storeRef == null)
            {
                return;
            }

            int index;
            index = storeRef.GetTintIndex();
            SetListIndex(TintItems, "Tint", 5000, index);

            if (weapon.HasBarrel)
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
