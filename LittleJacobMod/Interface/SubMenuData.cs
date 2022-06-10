namespace LittleJacobMod.Interface;
using System.Collections.Generic;
using System.Linq;
using LemonUI.Menus;
using Saving;

internal struct ItemData
{
    public readonly int Price;
    public readonly NativeItem Item;
    public readonly uint Hash;

    public ItemData(int price, NativeItem item, uint hash)
    {
        Price = price;
        Item = item;
        Hash = hash;
    }
}

internal class SubMenuData
{
    private readonly uint _weapon;
    public Dictionary<string, List<ItemData>> Attachments = new();
    public List<ItemData> TintItems { get; } = new();
    public List<ItemData> CamoColorItems { get; } = new();
    public List<ItemData> CamoItems { get; } = new();

    public SubMenuData(uint weapon)
    {
        _weapon = weapon;
    }

    public void ClearLists()
    {
        TintItems.Clear();
        CamoItems.Clear();
        CamoItems.Clear();

        foreach (var attachmentsValue in Attachments.Values)
        {
            attachmentsValue.Clear();
        }
    }

    private static void Restart(IReadOnlyCollection<ItemData> items)
    {
        for (var i = 0; i < items.Count; i++)
        {
            var data = items.ElementAt(i);

            if (i == 0)
            {
                data.Item.Enabled = false;
                data.Item.Description = "Current attachment";
                continue;
            }

            data.Item.Enabled = true;
            data.Item.Description = $"Price: ${data.Price.ToString()}";
        }
    }

    public static void SetIndex(IReadOnlyCollection<ItemData> items, string text, int index)
    {
        if (index == -1) return;
        for (var i = 0; i < items.Count; i++)
        {
            var data = items.ElementAt(i);

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
        Restart(TintItems);
        Restart(CamoItems);
        Restart(CamoColorItems);

        foreach (var attachment in Attachments.Values)
        {
            Restart(attachment);
        }
    }

    public void LoadAttachments()
    {
        RestartLists();
        var storeRef = LoadoutSaving.GetStoreReference(_weapon);

        if (storeRef == null)
        {
            return;
        }

        foreach (var group in Attachments)
        {
            if (storeRef.Attachments != null && !storeRef.Attachments.ContainsKey(group.Key)) continue;
                
            var savedGroup = storeRef.Attachments?[group.Key];
            var index = group.Value.FindIndex(it => savedGroup != null && it.Hash == savedGroup.Hash);
            SetIndex(group.Value, group.Key, index);
        }
            
        if (TintItems.Count > 0)
        {
            var index = storeRef.GetTintIndex();
            SetIndex(TintItems, "Tint", index);
        }

        if (CamoItems.Count > 0)
        {
            var index = CamoItems.FindIndex(it => storeRef.Camo != null && it.Hash == storeRef.Camo.Hash);
            SetIndex(CamoItems, "Livery", index);
            index = storeRef.GetCamoColor();
            SetIndex(CamoColorItems, "Livery Color", index);
        }
    }
}