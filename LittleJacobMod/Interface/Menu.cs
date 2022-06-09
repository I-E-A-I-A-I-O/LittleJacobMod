using System;
using System.Collections.Generic;
using System.IO;
using LemonUI;
using LemonUI.Menus;
using GTA;
using GTA.Native;
using LittleJacobMod.Utils;
using LittleJacobMod.Saving;
using LittleJacobMod.Utils.Types;

namespace LittleJacobMod.Interface
{
    public class Menu
    {
        public ObjectPool Pool { get; }
        private readonly List<SubMenuData> _subMenus = new List<SubMenuData>();
        private readonly NativeMenu _mainMenu;
        private readonly NativeMenu _helm1;
        private readonly NativeMenu _helm2;
        private readonly NativeMenu _helm3;
        private int _helmColor;
        private bool _move;

        public static event EventHandler<ComponentPreviewEventArgs> ComponentSelected;
        public static event EventHandler<uint> SpawnWeaponObject;
        public static event EventHandler<int> TintChanged;
        public static event EventHandler<CamoColorEventArgs> CamoColorChanged;
        public static event EventHandler<bool> HelmetMenuChanged;

        public Menu()
        {
            var baseDir = $"{Directory.GetCurrentDirectory()}\\scripts\\LittleJacobMod\\Weapons";
            var weapons = FileParser.DesearilizeJsonContents($"{baseDir}\\Json\\Weapons.json");
            Mapper.WeaponData = weapons;
            var weaponGroupMenus = new Dictionary<string, NativeMenu>();
            Pool = new ObjectPool();
            _mainMenu = new NativeMenu("Little Jacob", "Weapon store");
            Pool.Add(_mainMenu);

            foreach (var weapon in weapons)
            {
                if (weaponGroupMenus.ContainsKey(weapon.Group)) continue;
                var groupMenu = new NativeMenu(weapon.Group, weapon.Group);
                weaponGroupMenus.Add(weapon.Group, groupMenu);
                _mainMenu.AddSubMenu(groupMenu);
                Pool.Add(groupMenu);
                groupMenu.SelectedIndexChanged += (_, args) =>
                {
                    var groupedWeapons = weapons.FindAll(it => it.Group == groupMenu.Title.Text);
                    SpawnWeaponObject?.Invoke(this, groupedWeapons[args.Index].Hash);
                };
            }

            var gearMenu = new NativeMenu("Gear", "Gear");
            var armorOption = new NativeItem("Armor", "Buy armor");
            _helm1 = new NativeMenu("Thermal Vision Helmet", "Thermal Vision Helmet", "Price: $60000");
            _helm2 = new NativeMenu("Night Vision Helmet", "Night Vision Helmet", "Price: $40000");
            _helm3 = new NativeMenu("Tactical Night Vision", "Tactical Night Vision", "Price: $20000");
            
            Pool.Add(gearMenu);
            Pool.Add(_helm1);
            Pool.Add(_helm2);
            Pool.Add(_helm3);

            _mainMenu.AddSubMenu(gearMenu);
            gearMenu.Add(armorOption);
            gearMenu.AddSubMenu(_helm1);
            gearMenu.AddSubMenu(_helm2);
            gearMenu.AddSubMenu(_helm3);
            SetupHelmetMenu(_helm1, 1, 118, 60000);
            SetupHelmetMenu(_helm2, 1, 116, 40000);
            SetupHelmetMenu(_helm3, 0, 147, 20000);

            gearMenu.Shown += (_, _) =>
            {
                if (_move) return;
                _move = true;
                HelmetMenuChanged?.Invoke(this, true);
            };

            _mainMenu.Shown += (_, _) =>
            {
                if (!_move) return;
                _move = false;
                HelmetMenuChanged?.Invoke(this, false);
            };

            armorOption.Activated += (_, _) =>
            {
                if (Game.Player.Money < 3000)
                {
                    GTA.UI.Notification.Show("Not enough money to buy armor!");
                    return;
                }
                Game.Player.Money -= 3000;
                Game.Player.Character.Armor = 100;
                GTA.UI.Notification.Show("Armor purchased!");
            };

            foreach (var weapon in weapons)
            {
                AddSubmenu(weapon, weaponGroupMenus[weapon.Group]);
            }
        }

        private void AddSubmenu(Utils.Types.Weapon weapon, NativeMenu parentMenu)
        {
            NativeMenu weaponMenu = new(weapon.Name, weapon.Name, $"Price: {weapon.Price}");
            GTA.UI.Screen.ShowSubtitle(weapon.Name);
            Pool.Add(weaponMenu);
            parentMenu.AddSubMenu(weaponMenu);
            SubMenuData subMenuData = new(weapon.Hash);

            weaponMenu.Shown += (_, _) =>
            {
                if (!WeaponSelected(weapon.Name, weapon.Hash, weapon.Price)) weaponMenu.Back();
                else SpawnWeaponObject?.Invoke(this, weapon.Hash);
            };

            if (weapon.Group.ToLower() != "melee")
            {
                NativeSliderItem ammoOptionItem = new("Ammo", 250, 1);
                ammoOptionItem.Activated += (_, _) =>
                {
                    AmmoPurchased(weapon.Hash, ammoOptionItem.Value);
                };
                ammoOptionItem.Selected += (_, _) =>
                {
                    var ammoType = Function.Call<uint>(Hash.GET_PED_AMMO_TYPE_FROM_WEAPON, Main.PPID, weapon.Hash);
                    var ini = ScriptSettings.Load("scripts\\LittleJacobMod.ini");
                    var price = ini.GetValue("AmmoPrices", ammoType.ToString(), 1);
                    ammoOptionItem.Description = $"Selected ammo: {ammoOptionItem.Value}\nPrice: ${price * ammoOptionItem.Value}";
                };
                ammoOptionItem.ValueChanged += (_, _) =>
                {
                    var ammoType = Function.Call<uint>(Hash.GET_PED_AMMO_TYPE_FROM_WEAPON, Main.PPID, weapon.Hash);
                    var ini = ScriptSettings.Load("scripts\\LittleJacobMod.ini");
                    var price = ini.GetValue("AmmoPrices", ammoType.ToString(), 1);
                    ammoOptionItem.Description = $"Selected ammo: {ammoOptionItem.Value}\nPrice: ${price * ammoOptionItem.Value}";
                };
                weaponMenu.Add(ammoOptionItem);
            }

            Dictionary<string, NativeMenu> attachmentCategories = new();

            for (var index = 0; index < weapon.Attachments.Count; index++)
            {
                var attachment = weapon.Attachments[index];
                var si = index;
                if (!attachmentCategories.ContainsKey(attachment.Group))
                {
                    var category = new NativeMenu(attachment.Group, attachment.Group);
                    weaponMenu.AddSubMenu(category);
                    Pool.Add(category);
                    attachmentCategories.Add(attachment.Group, category);
                    subMenuData.Attachments.Add(attachment.Group, new List<ItemData>());
                    category.SelectedIndexChanged += (_, args) =>
                    {
                        var groupedAttachments = weapon.Attachments.FindAll(
                            it => it.Group == category.Title.Text);
                        var selectedAttachment = groupedAttachments[args.Index];
                        ComponentSelected?.Invoke(this,
                            new ComponentPreviewEventArgs(weapon.Hash, selectedAttachment.Hash,
                                selectedAttachment.Group));
                    };
                }

                var categoryMenu = attachmentCategories[attachment.Group];
                var componentItemOption = new NativeItem(attachment.Name, $"Price: ${attachment.Price}");
                componentItemOption.Activated += (_, _) =>
                {
                    if (ComponentPurchased(weapon.Hash, attachment.Hash, attachment.Price, attachment.Name, attachment.Group, false))
                    {
                        SubMenuData.SetIndex(subMenuData.Attachments[attachment.Group], "attachment", si);
                    }
                };
                categoryMenu.Add(componentItemOption);
                subMenuData.Attachments[attachment.Group].Add(
                    new ItemData(attachment.Price, componentItemOption, attachment.Hash));
            }

            if (weapon.Tints != null)
            {
                NativeMenu tintCategory = new("Tints", "Tints");
                weaponMenu.AddSubMenu(tintCategory);
                Pool.Add(tintCategory);
                
                for (var i = 0; i < weapon.Tints.Count; i++)
                {
                    var si = i;
                    var tint = weapon.Tints[i];
                    NativeItem tintItem = new(tint.Name, $"Price: ${tint.Price}");
                    tintCategory.Add(tintItem);
                    tintItem.Activated += (_, _) =>
                    {
                        if (TintPurchased(weapon.Hash, si, tint.Price))
                            SubMenuData.SetIndex(subMenuData.TintItems, "tint", si);
                    };
                    subMenuData.TintItems.Add(
                        new ItemData(tint.Price, tintItem, 0));
                }

                tintCategory.SelectedIndexChanged += (_, args) => TintChanged?.Invoke(this, args.Index);
                tintCategory.Closed += (_, _) => SpawnWeaponObject?.Invoke(this, weapon.Hash);
            }

            if (weapon.CamoComponents != null)
            {
                NativeMenu styleCategory = new("Livery", "Livery");
                NativeMenu colorCategory = new("Livery Color", "Livery Color");
                weaponMenu.AddSubMenu(styleCategory);
                weaponMenu.AddSubMenu(colorCategory);
                Pool.Add(styleCategory);
                Pool.Add(colorCategory);

                for (var index = 0; index < weapon.CamoComponents.Components.Count; index++)
                {
                    var style = weapon.CamoComponents.Components[index];
                    var sc = index;
                    NativeItem styleItem = new(style.Name, $"Price: ${style.Price}");
                    styleCategory.Add(styleItem);
                    styleItem.Activated += (_, _) =>
                    {
                        if (ComponentPurchased(weapon.Hash, style.Hash, style.Price, style.Name, "", true))
                        {
                            SubMenuData.SetIndex(subMenuData.CamoItems, "livery", sc);
                        }
                    };

                    subMenuData.CamoItems.Add(new ItemData(style.Price, styleItem, style.Hash));
                }

                for (var i = 0; i < weapon.CamoComponents.Colors.Count; i++)
                {
                    var si = i;
                    var color = weapon.CamoComponents.Colors[i];
                    NativeItem colorItem = new(color.Name, $"Price: ${color.Price}");
                    colorCategory.Add(colorItem);
                    colorItem.Activated += (_, _) =>
                    {
                        if (CamoColorPurchased(weapon.Hash, si, color.Name, color.Price))
                        {
                            SubMenuData.SetIndex(subMenuData.CamoColorItems, "color", si);
                        }
                    };
                        
                    subMenuData.CamoColorItems.Add(new ItemData(color.Price, colorItem, 0));
                }

                colorCategory.SelectedIndexChanged += (_, args) =>
                {
                    var reference = LoadoutSaving.GetStoreReference(weapon.Hash);
                    var camo = reference.GetCamo();

                    if (camo == (uint) WeaponComponentHash.Invalid) return;

                    var ev = new CamoColorEventArgs { Camo = camo, ColorIndex = args.Index };
                    CamoColorChanged?.Invoke(this, ev);
                };
                
                styleCategory.SelectedIndexChanged += (_, args) =>
                {
                    var style = weapon.CamoComponents.Components[args.Index];
                    ComponentSelected?.Invoke(this, new ComponentPreviewEventArgs(weapon.Hash, style.Hash, "Livery"));
                };
                
                styleCategory.Closed += (_, _) => SpawnWeaponObject?.Invoke(this, weapon.Hash);
                colorCategory.Closed += (_, _) => SpawnWeaponObject?.Invoke(this, weapon.Hash);
            }
            _subMenus.Add(subMenuData);
        }

        public void ReloadOptions()
        {
            foreach (var sub in _subMenus)
            {
                sub.LoadAttachments();
            }

            switch (Main.IsMPped())
            {
                case 0:
                    _helm1.Description = HelmetState.MpmtvOwned ? "Select to equip" : "Price: $60000";
                    _helm2.Description = HelmetState.Mpmnv1Owned ? "Select to equip" : "Price: $40000";
                    _helm3.Description = HelmetState.Mpmnv2Owned ? "Select to equip" : "Price: $20000";
                    break;

                case 1:
                    _helm1.Description = HelmetState.MpftvOwned ? "Select to equip" : "Price: $60000";
                    _helm2.Description = HelmetState.Mpfnv1Owned ? "Select to equip" : "Price: $40000";
                    _helm3.Description = HelmetState.Mpfnv2Owned ? "Select to equip" : "Price: $20000";
                    break;

                default:
                    _helm1.Description = "Not available for this character";
                    _helm2.Description = "Not available for this character";
                    _helm3.Description = "Not available for this character";
                    break;
            }
        }

        private static int HelmetType(int helmet, int pedType)
        {
            if (pedType == 1)
            {
                switch (helmet)
                {
                    case 115 or 116:
                        return 1;
                    case 117 or 118:
                        return 0;
                    case 146 or 147:
                        return 2;
                }
            } else
            {
                switch (helmet)
                {
                    case 116 or 117:
                        return 4;
                    case 118 or 119:
                        return 3;
                    case 147 or 148:
                        return 5;
                }
            }

            return -1;
        }

        private void SetupHelmetMenu(NativeMenu menu, int type, int code, int helmPrice)
        {
            int price;
            List<string> colors;

            if (type == 0)
            {
                price = 5000;
                colors = TintsAndCamos.NV2Colors;
            } else
            {
                price = 10000;
                colors = TintsAndCamos.HelmColors;
            }

            for (var i = 0; i < colors.Count; i++)
            {
                var index = i;
                var item = new NativeItem(colors[i], $"Price: ${price.ToString()}");

                item.Activated += (_, _) =>
                {
                    if (Game.Player.Money < price)
                    {
                        GTA.UI.Notification.Show("~r~Not enough money!");
                        return;
                    }

                    Game.Player.Money -= price;
                    _helmColor = index;
                    GTA.UI.Notification.Show($"~g~{colors[index]} purchased!");
                };

                menu.Add(item);
            }

            menu.Shown += (_, _) =>
            {
                var pedType = Main.IsMPped();

                if (pedType == -1)
                {
                    menu.Back();
                    GTA.UI.Notification.Show("~y~Not available for this character!");
                    return;
                }

                var compIndx = code;

                if (pedType == 0)
                {
                    compIndx += 1;
                }

                var helmIndx = Function.Call<int>(Hash.GET_PED_PROP_INDEX, Main.PPID, 0);
                var purchased = false;

                if (compIndx != helmIndx)
                {
                    switch(HelmetType(compIndx, pedType))
                    {
                        case 0:
                            if (!HelmetState.MpftvOwned)
                            {
                                if (Game.Player.Money < helmPrice)
                                {
                                    GTA.UI.Notification.Show("~r~Not enough money.");
                                    menu.Back();
                                    return;
                                }
                                menu.Description = "Select to equip";
                                HelmetState.MpftvOwned = true;
                                Game.Player.Money -= helmPrice;
                                purchased = true;
                            }
                            break;
                        case 1:
                            if (!HelmetState.Mpfnv1Owned)
                            {
                                if (Game.Player.Money < helmPrice)
                                {
                                    GTA.UI.Notification.Show("~r~Not enough money.");
                                    menu.Back();
                                    return;
                                }
                                menu.Description = "Select to equip";
                                HelmetState.Mpfnv1Owned = true;
                                Game.Player.Money -= helmPrice;
                                purchased = true;
                            }
                            break;
                        case 2:
                            if (!HelmetState.Mpfnv2Owned)
                            {
                                if (Game.Player.Money < helmPrice)
                                {
                                    GTA.UI.Notification.Show("~r~Not enough money.");
                                    menu.Back();
                                    return;
                                }
                                menu.Description = "Select to equip";
                                HelmetState.Mpfnv2Owned = true;
                                Game.Player.Money -= helmPrice;
                                purchased = true;
                            }
                            break;
                        case 3:
                            if (!HelmetState.MpmtvOwned)
                            {
                                if (Game.Player.Money < helmPrice)
                                {
                                    GTA.UI.Notification.Show("~r~Not enough money.");
                                    menu.Back();
                                    return;
                                }
                                menu.Description = "Select to equip";
                                HelmetState.MpmtvOwned = true;
                                Game.Player.Money -= helmPrice;
                                purchased = true;
                            }
                            break;
                        case 4:
                            if (!HelmetState.Mpmnv1Owned)
                            {
                                if (Game.Player.Money < helmPrice)
                                {
                                    GTA.UI.Notification.Show("~r~Not enough money.");
                                    menu.Back();
                                    return;
                                }
                                menu.Description = "Select to equip";
                                HelmetState.Mpmnv1Owned = true;
                                Game.Player.Money -= helmPrice;
                                purchased = true;
                            }
                            break;
                        case 5:
                            if (!HelmetState.Mpmnv2Owned)
                            {
                                if (Game.Player.Money < helmPrice)
                                {
                                    GTA.UI.Notification.Show("~r~Not enough money.");
                                    menu.Back();
                                    return;
                                }
                                menu.Description = "Select to equip";
                                HelmetState.Mpmnv2Owned = true;
                                Game.Player.Money -= helmPrice;
                                purchased = true;
                            }
                            break;
                    }

                    _helmColor = compIndx - 1 == helmIndx ? Function.Call<int>(Hash.GET_PED_PROP_TEXTURE_INDEX, Main.PPID, compIndx - 1) : 0;
                    Function.Call(Hash.SET_PED_PROP_INDEX, Main.PPID, 0, compIndx, _helmColor, 1);
                }

                Game.IsNightVisionActive = false;
                Game.IsThermalVisionActive = false;
                GTA.UI.Notification.Show($"~g~{menu.Title.Text} {(purchased ? "Purchased" : "Equipped")}"); 
            };

            menu.Closed += (_, _) =>
            {
                var helmIndx = Function.Call<int>(Hash.GET_PED_PROP_INDEX, Main.PPID, 0);
                Function.Call(Hash.SET_PED_PROP_INDEX, Main.PPID, 0, helmIndx, _helmColor, 1);
            };

            menu.SelectedIndexChanged += (_, e) =>
            {
                var helmIndx = Function.Call<int>(Hash.GET_PED_PROP_INDEX, Main.PPID, 0);
                Function.Call(Hash.SET_PED_PROP_INDEX, Main.PPID, 0, helmIndx, e.Index, 1);
            };
        }

        public void ShowMainMenu()
        {
            _mainMenu.Visible = true;
        }

        private static bool WeaponSelected(string name, uint weapon, int price)
        {
            var hasWeapon = Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON, Main.PPID, weapon, 0);

            if (!hasWeapon)
            {
                if (Game.Player.Money < price)
                {
                    GTA.UI.Notification.Show($"Can't buy {name}. Not enough money!");
                    return false;
                }

                GTA.UI.Notification.Show($"{name} purchased!");
                //Main.LittleJacob.ProcessVoice(true, true);
                Game.Player.Money -= price;
                Function.Call<bool>(Hash.GIVE_WEAPON_TO_PED, Main.PPID, weapon, 1, true, true);
                Main.ShowScaleform("~g~purchased", name, 1, weapon);
                Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "Weapon_Upgrade", "DLC_GR_Weapon_Upgrade_Soundset", true);
            } else
            {
                Function.Call(Hash.SET_CURRENT_PED_WEAPON, Main.PPID, weapon, true);
            }

            LoadoutSaving.AddWeapon(weapon);

            return true;
        }

        private void AmmoPurchased(uint weapon, int value)
        {
            int maxAmmo;
            unsafe
            {
                Function.Call(Hash.GET_MAX_AMMO, Main.PPID, weapon, &maxAmmo);
            }
            var purchasableAmmo = maxAmmo - Function.Call<int>(Hash.GET_AMMO_IN_PED_WEAPON, Main.PPID, weapon);

            if (purchasableAmmo == 0)
            {
                GTA.UI.Notification.Show("Max ammo capacity reached!");
                return;
            }

            var ammoToPurchase = value >= purchasableAmmo ? value - purchasableAmmo : value;
            var ammoType = Function.Call<uint>(Hash.GET_PED_AMMO_TYPE_FROM_WEAPON, Main.PPID, weapon);
            var ini = ScriptSettings.Load("scripts\\LittleJacobMod.ini");
            var price = ini.GetValue("AmmoPrices", ammoType.ToString(), 1);

            if (price == 0)
                price = 50000;

            var ammoPrice = ammoToPurchase * price;

            if (Game.Player.Money < ammoPrice)
            {
                GTA.UI.Notification.Show("Can't purchase ammo. Not enough money!");
                return;
            }

            Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "WEAPON_AMMO_PURCHASE", "HUD_AMMO_SHOP_SOUNDSET", true);
            Game.Player.Money -= ammoPrice;
            Function.Call(Hash._ADD_AMMO_TO_PED_BY_TYPE, Main.PPID, ammoType, ammoToPurchase);
            LoadoutSaving.SetAmmo(weapon, Function.Call<int>(Hash.GET_AMMO_IN_PED_WEAPON, Main.PPID, weapon));
        }

        private bool TintPurchased(uint weapon, int index, int price)
        {
            price = ApplyDiscount(price);

            if (Game.Player.Money < price)
            {
                GTA.UI.Notification.Show("Couldn't purchase this tint. Not enough money!");
                return false;
            }
            
            Game.Player.Money -= price;
            TintChanged?.Invoke(this, index);
            Function.Call(Hash.SET_PED_WEAPON_TINT_INDEX, Main.PPID, weapon, index);
            GTA.UI.Notification.Show("Tint purchased!");
            Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "WEAPON_PURCHASE", "HUD_AMMO_SHOP_SOUNDSET", true);
            LoadoutSaving.SetTint(weapon, index);
            return true;
        }

        private bool ComponentPurchased(uint weapon, uint component, int price, string name, string group, bool camo)
        {
            price = ApplyDiscount(price);

            if (Game.Player.Money < price)
            {
                GTA.UI.Notification.Show($"Couldn't purchase {name}. Not enough money!");
                return false;
            }

            Game.Player.Money -= price;
            Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, Main.PPID, weapon, component);
            var slide = TintsAndCamos.ReturnSlide(component);

            if (slide != (uint)WeaponComponentHash.Invalid) 
            { 
                Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, Main.PPID, weapon, slide);
            }
                
            GTA.UI.Notification.Show($"{name} purchased!");
            Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "WEAPON_PURCHASE", "HUD_AMMO_SHOP_SOUNDSET", true);
            
            if (camo) LoadoutSaving.SetCamo(weapon, new Component
            {
                Hash = component,
                Name = name,
                Price = price
            });
            else LoadoutSaving.SetAttachment(weapon, new GroupedComponent
            {
                Group = group,
                Hash = component,
                Name = name,
                Price = price
            });
            
            return true;
        }

        private bool CamoColorPurchased(uint weapon, int index, string name, int price)
        {
            var storedWeapon = LoadoutSaving.GetStoreReference(weapon);

            if (storedWeapon == null) return false;

            var camo = storedWeapon.GetCamo();
            
            if (camo == (uint)WeaponComponentHash.Invalid)
            {
                GTA.UI.Notification.Show("Buy a camo first!");
                return false;
            }

            price = ApplyDiscount(price);

            if (Game.Player.Money < price)
            {
                GTA.UI.Notification.Show("Couldn't purchase camo color. Not enough money!");
                return false;
            }
            
            Game.Player.Money -= price;
            Function.Call(Hash._SET_PED_WEAPON_LIVERY_COLOR, Main.PPID, weapon, camo, index);
            var slide = TintsAndCamos.ReturnSlide(camo);
            
            if (slide != (uint)WeaponComponentHash.Invalid)
            {
                Function.Call(Hash._SET_PED_WEAPON_LIVERY_COLOR, Main.PPID, weapon, slide, index);
            }

            GTA.UI.Notification.Show($"Camo color {name} purchased!");
            Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "WEAPON_PURCHASE", "HUD_AMMO_SHOP_SOUNDSET", true);
            LoadoutSaving.SetCamoColor(weapon, index);
            return true;
        }

        private int ApplyDiscount(int price)
        {
            var canApply = false;

            if (Function.Call<bool>(Hash.IS_PED_MODEL, Main.PPID, (uint)PedHash.Michael) && MissionSaving.MProgress >= 4)
                canApply = true;
            else if (Function.Call<bool>(Hash.IS_PED_MODEL, Main.PPID, (uint)PedHash.Franklin) && MissionSaving.FProgress >= 4)
                canApply = true;
            else if (Function.Call<bool>(Hash.IS_PED_MODEL, Main.PPID, (uint)PedHash.Trevor) && MissionSaving.TUnlocked)
                canApply = true;

            if (!canApply) return price;
            var disc = price * 0.20f;
            return (int)(price - disc);

        }
    }
}
