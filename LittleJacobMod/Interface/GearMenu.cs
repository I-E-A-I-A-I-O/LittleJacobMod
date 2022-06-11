namespace LittleJacobMod.Interface;

using System;
using Saving;
using Utils;
using LemonUI;
using LemonUI.Menus;
using GTA.Native;

public class GearMenu
{
    public ObjectPool Pool { get; }
    private readonly NativeMenu _mainMenu;
    private readonly NativeItem _thermalItem;
    private readonly NativeItem _nightVisionItem1;
    private readonly NativeItem _nightVisionItem2;
    private readonly NativeMenu _colorsMenu;
    private int _propColor;
    private int _propIndex;
    private bool _goingBack;

    public GearMenu() 
    {
        Pool = new ObjectPool();
        _mainMenu = new NativeMenu("Gear Menu", "Gear Menu");
        Pool.Add(_mainMenu);
        _thermalItem = new NativeItem("Thermal Vision Helmet");
        _nightVisionItem1 = new NativeItem("Night Vision Helmet");
        _nightVisionItem2 = new NativeItem("Tactical Night Vision");
        _mainMenu.Add(_thermalItem);
        _mainMenu.Add(_nightVisionItem1);
        _mainMenu.Add(_nightVisionItem2);
        _colorsMenu = new("Helmet Colors", "Helmet Colors") { NoItemsText = "No colors available." };
        _mainMenu.AddSubMenu(_colorsMenu);
        Pool.Add(_colorsMenu);
        _colorsMenu.Shown += ColorsMenuShown;

        _mainMenu.Opening += (_, _) =>
        {
            if (_goingBack)
            {
                _goingBack = false;
                return;
            }
            
            _propIndex = Function.Call<int>(Hash.GET_PED_PROP_INDEX, Main.PPID, 0);
            _propColor = Function.Call<int>(Hash.GET_PED_PROP_TEXTURE_INDEX, Main.PPID, 0);
            var index = GetHelmetIndex(_propIndex);
            var pedType = Main.IsMPped();

            for (int i = 0; i < _mainMenu.Items.Count - 1; i++)
            {
                if (i == index)
                {
                    _mainMenu.Items[index].Enabled = false;
                    _mainMenu.Items[index].Description = "Current Helmet";
                }
                else
                {
                    bool exp;
                    
                    switch (i)
                    {
                        case 0:
                            exp = (pedType == 0
                                ? HelmetSaving.State?.MpMaleThermalVision
                                : HelmetSaving.State?.MpFemaleThermalVision) ?? false;
                            break;
                        case 1:
                            exp = (pedType == 0
                                ? HelmetSaving.State?.MpMaleNightVision1
                                : HelmetSaving.State?.MpFemaleNightVision1) ?? false;
                            break;
                        case 2:
                            exp = (pedType == 0
                                ? HelmetSaving.State?.MpMaleNightVision2
                                : HelmetSaving.State?.MpFemaleNightVision2) ?? false;
                            break;
                        default:
                            continue;
                    }

                    _mainMenu.Items[i].Enabled = exp;
                    _mainMenu.Items[i].Description = exp ? "" : "Item not owned.";
                }
            }
        };
        
        _mainMenu.ItemActivated += (_, args) =>
        {
            var index = _mainMenu.SelectedIndex;
            if (index == 3) return;
            var pedType = Main.IsMPped();

            for (int i = 0; i < _mainMenu.Items.Count; i++) 
            {
                if (i == index || i == _mainMenu.Items.Count - 1) continue;
                bool exp;
                    
                switch (i)
                {
                    case 0:
                        exp = (pedType == 0
                            ? HelmetSaving.State?.MpMaleThermalVision
                            : HelmetSaving.State?.MpFemaleThermalVision) ?? false;
                        break;
                    case 1:
                        exp = (pedType == 0
                            ? HelmetSaving.State?.MpMaleNightVision1
                            : HelmetSaving.State?.MpFemaleNightVision1) ?? false;
                        break;
                    case 2:
                        exp = (pedType == 0
                            ? HelmetSaving.State?.MpMaleNightVision2
                            : HelmetSaving.State?.MpFemaleNightVision2) ?? false;
                        break;
                    default:
                        continue;
                }

                _mainMenu.Items[i].Enabled = exp;
                _mainMenu.Items[i].Description = exp ? "" : "Item not owned.";
            }

            args.Item.Enabled = false;
            args.Item.Description = "Current Helmet";
            var helmet = GetHelmetCode(pedType, index);
            _propIndex = helmet;
            _propColor = 0;
            Function.Call(Hash.SET_PED_PROP_INDEX, Main.PPID, 0, _propIndex, _propColor, 1);
        };

        _mainMenu.SelectedIndexChanged += (_, args) =>
        {
            if (args.Index == 3 || !_mainMenu.Items[args.Index].Enabled)
            {
                if (_propIndex == -1)
                    Function.Call(Hash.KNOCK_OFF_PED_PROP, Main.PPID, 0, 1, 0, 0);
                else
                    Function.Call(Hash.SET_PED_PROP_INDEX, Main.PPID, 0, _propIndex, _propColor, 1);
                
                return;
            }
            
            var pedType = Main.IsMPped();
            var drawable = GetHelmetCode(pedType, args.Index);
            Function.Call(Hash.SET_PED_PROP_INDEX, Main.PPID, 0, drawable, drawable == _propIndex ? _propColor : 0, 1);
        };

        _mainMenu.Closed += (_, _) =>
        {
            if (_propIndex == -1)
                Function.Call(Hash.KNOCK_OFF_PED_PROP, Main.PPID, 0, 1, 0, 0);
            else
                Function.Call(Hash.SET_PED_PROP_INDEX, Main.PPID, 0, _propIndex, _propColor, 1);
        };
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

        for (int i = 0; i < colorList.Count; i++)
        {
            if (!HelmetSaving.State.OwnedColors[helmet].Contains(i)) continue;

            var index = i;
            var color = colorList[i];
            NativeItem item = new(color);
            item.Enabled = _propColor != i;
            item.Description = _propColor == i ? "Current Color" : "";
            _colorsMenu.Add(item);

            item.Activated += (_, _) =>
            {
                if (_propColor != -1)
                {
                    var colorIndex = GetMenuIndex(colorList[_propColor]);

                    if (colorIndex != -1)
                    {
                        _colorsMenu.Items[colorIndex].Enabled = true;
                        _colorsMenu.Items[colorIndex].Description = "";   
                    }
                }
                
                _propColor = index;
                item.Enabled = false;
                item.Description = "Current Color";
                GTA.UI.Notification.Show($"~g~{color} equipped!", true);
            };
        }

        _colorsMenu.SelectedIndexChanged += (_, e) =>
        {
            var item = _colorsMenu.Items[e.Index];
            var color = colorList.FindIndex(it => it == item.Title);

            if (color == -1) return;
            
            Function.Call(Hash.SET_PED_PROP_INDEX, Main.PPID, 0, helmet, color, 1);
        };
        
        _colorsMenu.Closing += (_, _) =>
        {
            _goingBack = true;
            Function.Call(Hash.SET_PED_PROP_INDEX, Main.PPID, 0, helmet, _propColor, 1);
        };
    }

    public void Open() 
    {
        _mainMenu.Visible = true;
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

    private int GetMenuIndex(string colorName)
    {
        for (var i = 0; i < _colorsMenu.Items.Count; i++)
        {
            if (_colorsMenu.Items[i].Title == colorName) return i;
        }

        return -1;
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
