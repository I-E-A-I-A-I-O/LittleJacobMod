namespace LittleJacobMod.Interface;

using System;
using System.Linq;
using Saving;
using Utils;
using LemonUI;
using LemonUI.Menus;
using GTA.Native;

public class GearMenu
{
    public ObjectPool Pool { get; }
    private NativeItem _thermalItem;
    private NativeItem _nightVisionItem1;
    private NativeItem _nightVisionItem2;
    private NativeMenu _colorsMenu;
    private int _propColor;
    private int _propIndex;

    public GearMenu() 
    {
        Pool = new ObjectPool();
        var mainMenu = new NativeMenu("Gear Menu", "Gear Menu");
        Pool.Add(mainMenu);
        _thermalItem = new NativeItem("Thermal Vision Helmet");
        _nightVisionItem1 = new NativeItem("Night Vision Helmet");
        _nightVisionItem2 = new NativeItem("Tactival Night Vision");
        mainMenu.Add(_thermalItem);
        mainMenu.Add(_nightVisionItem1);
        mainMenu.Add(_nightVisionItem2);
        _colorsMenu = new NativeMenu("Helmet Colors", "Helmet Colors");
        _colorsMenu.NoItemsText = "No colors available.";
        mainMenu.AddSubMenu(_colorsMenu);
        Pool.Add(_colorsMenu);
        _colorsMenu.Shown += ColorsMenuShown;

        mainMenu.Opening += (_, _) =>
        {
            _propIndex = Function.Call<int>(Hash.GET_PED_PROP_INDEX, Main.PPID, 0);
            _propColor = Function.Call<int>(Hash.GET_PED_PROP_TEXTURE_INDEX, Main.PPID, 0);
            var index = GetHelmetIndex(_propIndex);

            if (index == -1) return;

            mainMenu.Items[index].Enabled = false;
            mainMenu.Items[index].Description = "Current Helmet";
        };
        
        mainMenu.ItemActivated += (_, args) =>
        {
            var index = mainMenu.SelectedIndex;

            for (int i = 0; i < mainMenu.Items.Count; i++) 
            {
                if (i == index || i == mainMenu.Items.Count - 1) continue;
                mainMenu.Items[i].Enabled = true;
                mainMenu.Items[i].Description = "";
            }

            args.Item.Enabled = false;
            args.Item.Description = "Current Helmet";
            var pedType = Main.IsMPped();
            var helmet = GetHelmetCode(pedType, index);
            _propIndex = helmet;
            _propColor = 0;
            Function.Call(Hash.SET_PED_PROP_INDEX, Main.PPID, 0, _propIndex, _propColor, 1);
        };

        mainMenu.SelectedIndexChanged += (_, args) =>
        {
            if (args.Index == 3) Function.Call(Hash.SET_PED_PROP_INDEX, Main.PPID, 0, _propIndex, _propColor, 1);
            else
            {
                var pedType = Main.IsMPped();
                var drawable = GetHelmetCode(pedType, args.Index);
                Function.Call(Hash.SET_PED_PROP_INDEX, Main.PPID, 0, drawable, drawable == _propIndex ? _propColor : 0, 1);
            }
        };

        mainMenu.Closed += (_, _) => Function.Call(Hash.SET_PED_PROP_INDEX, Main.PPID, 0, _propIndex, _propColor, 1);
    }

    private void ColorsMenuShown(object sender, EventArgs args) 
    {
        _colorsMenu.Clear();
        var pedType = Main.IsMPped();
        var helmet = Function.Call<int>(Hash.GET_PED_PROP_INDEX, Main.PPID, 0);
        var helmetType = Menu.HelmetType(helmet, pedType);

        if (helmetType == -1) return;
        if (HelmetSaving.State == null || HelmetSaving.State.OwnedColors == null) return;
        if (!HelmetSaving.State.OwnedColors.ContainsKey(helmet)) return;
        if (HelmetSaving.State.OwnedColors[helmet] == null || HelmetSaving.State.OwnedColors[helmet].Count == 0) return;
        
        var colorList = helmetType switch
        {
            0 or 1 or 3 or 4 => TintsAndCamos.HelmColors,
            _ => TintsAndCamos.NV2Colors
        };

        if (colorList == null) return;

        for (int i = 0; i < colorList.Count; i++)
        {
            if (!HelmetSaving.State.OwnedColors[helmet].Contains(i)) continue;

            var color = colorList[i];
            var index = i;
            NativeItem item = new(color);
            item.Enabled = _propColor != i;
            item.Description = _propColor == i ? "Current Color" : "";
            _colorsMenu.Add(item);

            item.Activated += (_, _) =>
            {
                _colorsMenu.Items[_propColor].Enabled = true;
                _colorsMenu.Items[_propColor].Description = "";
                _propColor = index;
                GTA.UI.Notification.Show($"~g~{color} equipped!", true);
            };
        }

        _colorsMenu.SelectedIndexChanged += (_, e) => Function.Call(Hash.SET_PED_PROP_INDEX, Main.PPID, 0, helmet, e.Index, 1);
        _colorsMenu.Closed += (_, _) => Function.Call(Hash.SET_PED_PROP_INDEX, Main.PPID, 0, helmet, _propColor, 1);
    }

    public void ReloadOptions()
    {
        var pedType = Main.IsMPped();

        if (HelmetSaving.State == null) return;

        if (pedType == 0) 
        {
            _thermalItem.Enabled = HelmetSaving.State.MpMaleThermalVision;
            _thermalItem.Description = HelmetSaving.State.MpMaleThermalVision ? "Select to equip" : "Item not owned";
            _nightVisionItem1.Enabled = HelmetSaving.State.MpMaleNightVision1;
            _nightVisionItem1.Description = HelmetSaving.State.MpMaleNightVision1 ? "Select to equip" : "Item not owned";
            _nightVisionItem2.Enabled = HelmetSaving.State.MpMaleNightVision2;
            _nightVisionItem2.Description = HelmetSaving.State.MpMaleNightVision2 ? "Select to equip" : "Item not owned";
        }
        else 
        {
            _thermalItem.Enabled = HelmetSaving.State.MpFemaleThermalVision;
            _thermalItem.Description = HelmetSaving.State.MpFemaleThermalVision ? "Select to equip" : "Item not owned";
            _nightVisionItem1.Enabled = HelmetSaving.State.MpFemaleNightVision1;
            _nightVisionItem1.Description = HelmetSaving.State.MpFemaleNightVision1 ? "Select to equip" : "Item not owned";
            _nightVisionItem2.Enabled = HelmetSaving.State.MpFemaleNightVision2;
            _nightVisionItem2.Description = HelmetSaving.State.MpFemaleNightVision2 ? "Select to equip" : "Item not owned";
        }
    }

    private int GetHelmetCode(int pedType, int index)
    {
        return index switch
        {
            0 => pedType == 1 ? 118 : 119,
            1 => pedType == 1 ? 116 : 117,
            2 => pedType == 1 ? 147 : 148,
            _ => -1
        };
    }

    private int GetHelmetIndex(int helmet)
    {
        return helmet switch
        {
            118 or 119 => 0,
            116 or 117 => 1,
            147 or 148 => 2,
            _ => -1
        };
    }
}
