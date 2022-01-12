using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using LemonUI;
using LemonUI.Menus;
using LemonUI.Scaleform;
using GTA;
using GTA.Native;
using LittleJacobMod.Utils;
using LittleJacobMod.Saving;

namespace LittleJacobMod.Interface
{
    public class Menu
    {
        public ObjectPool Pool { get; private set; }
        public bool DrawScaleform;
        List<SubMenuData> _subMenus = new List<SubMenuData>();
        NativeMenu _mainMenu;
        NativeMenu _helm1;
        NativeMenu _helm2;
        NativeMenu _helm3;
        int _helmColor;
        bool _move;
        BigMessage _bigMessage;

        public static event EventHandler<ComponentPreviewEventArgs> ComponentSelected;
        public static event EventHandler<uint> SpawnWeaponObject;
        public static event EventHandler<int> TintChanged;
        public static event EventHandler<CamoColorEventArgs> CamoColorChanged;
        public static event EventHandler<bool> HelmetMenuChanged;

        public Menu()
        {
            Pool = new ObjectPool();
            _mainMenu = new NativeMenu("Little Jacob", "Weapon store");
            NativeMenu _melee = new NativeMenu("Melee", "Melee weapons");
            NativeMenu _pistols = new NativeMenu("Pistols", "Pistols");
            NativeMenu _rifles = new NativeMenu("Rifles", "Rifles");
            NativeMenu _mg = new NativeMenu("Machine guns", "Machine guns");
            NativeMenu _snipers = new NativeMenu("Sniper rifles", "Sniper rifles");
            NativeMenu _shotguns = new NativeMenu("Shotguns", "Shotguns");
            NativeMenu _explosives = new NativeMenu("Explosives", "Explosives");
            NativeMenu _heavy = new NativeMenu("Heavy Weapons", "Heavy Weapons");
            NativeMenu gearMenu = new NativeMenu("Gear", "Gear");
            NativeItem armorOption = new NativeItem("Armor", "Buy armor");
            _helm1 = new NativeMenu("Thermal Vision Helmet", "Thermal Vision Helmet", "Price: $60000");
            _helm2 = new NativeMenu("Night Vision Helmet", "Night Vision Helmet", "Price: $40000");
            _helm3 = new NativeMenu("Tactical Night Vision", "Tactical Night Vision", "Price: $20000");

            Pool.Add(_mainMenu);
            Pool.Add(_melee);
            Pool.Add(_pistols);
            Pool.Add(_rifles);
            Pool.Add(_mg);
            Pool.Add(_snipers);
            Pool.Add(_explosives);
            Pool.Add(_shotguns);
            Pool.Add(_heavy);
            Pool.Add(gearMenu);
            Pool.Add(_helm1);
            Pool.Add(_helm2);
            Pool.Add(_helm3);

            _mainMenu.AddSubMenu(_melee);
            _mainMenu.AddSubMenu(_pistols);
            _mainMenu.AddSubMenu(_rifles);
            _mainMenu.AddSubMenu(_shotguns);
            _mainMenu.AddSubMenu(_mg);
            _mainMenu.AddSubMenu(_snipers);
            _mainMenu.AddSubMenu(_heavy);
            _mainMenu.AddSubMenu(_explosives);
            _mainMenu.AddSubMenu(gearMenu);
            gearMenu.Add(armorOption);
            gearMenu.AddSubMenu(_helm1);
            gearMenu.AddSubMenu(_helm2);
            gearMenu.AddSubMenu(_helm3);
            SetupHelmetMenu(_helm1, 1, 118, 60000);
            SetupHelmetMenu(_helm2, 1, 116, 40000);
            SetupHelmetMenu(_helm3, 0, 147, 20000);

            gearMenu.Shown += (o, e) =>
            {
                if (!_move)
                {
                    _move = true;
                    HelmetMenuChanged?.Invoke(this, true);
                }
            };

            _mainMenu.Shown += (o, e) =>
            {
                if (_move)
                {
                    _move = false;
                    HelmetMenuChanged?.Invoke(this, false);
                }
            };

            armorOption.Activated += (o, e) =>
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

            string baseDir = $"{Directory.GetCurrentDirectory()}\\scripts\\LittleJacobMod\\Weapons";
            List<WeaponData> melee = AddSubmenu($"{baseDir}\\Normal\\Melee", _melee, true);
            List<WeaponData> pistols = AddSubmenu($"{baseDir}\\Normal\\Pistols", _pistols);
            List<WeaponData> pistols2 = AddSubmenu($"{baseDir}\\MK2\\Pistols", _pistols);
            List<WeaponData> mg = AddSubmenu($"{baseDir}\\Normal\\Machine Guns", _mg);
            List<WeaponData> mg2 = AddSubmenu($"{baseDir}\\MK2\\Machine Guns", _mg);
            List<WeaponData> rifles = AddSubmenu($"{baseDir}\\Normal\\Rifles", _rifles);
            List<WeaponData> rifles2 = AddSubmenu($"{baseDir}\\MK2\\Rifles", _rifles);
            List<WeaponData> shotguns = AddSubmenu($"{baseDir}\\Normal\\Shotguns", _shotguns);
            List<WeaponData> shotguns2 = AddSubmenu($"{baseDir}\\MK2\\Shotguns", _shotguns);
            List<WeaponData> snipers = AddSubmenu($"{baseDir}\\Normal\\Snipers", _snipers);
            List<WeaponData> snipers2 = AddSubmenu($"{baseDir}\\MK2\\Snipers", _snipers);
            List<WeaponData> heavy = AddSubmenu($"{baseDir}\\Normal\\Heavy", _heavy);
            List<WeaponData> explosives = AddSubmenu($"{baseDir}\\Normal\\Explosives", _explosives);

            pistols.AddRange(pistols2);
            mg.AddRange(mg2);
            rifles.AddRange(rifles2);
            shotguns.AddRange(shotguns2);
            snipers.AddRange(snipers2);

            Mapper.WeaponData.AddRange(melee);
            Mapper.WeaponData.AddRange(pistols);
            Mapper.WeaponData.AddRange(mg);
            Mapper.WeaponData.AddRange(rifles);
            Mapper.WeaponData.AddRange(shotguns);
            Mapper.WeaponData.AddRange(snipers);
            Mapper.WeaponData.AddRange(heavy);
            Mapper.WeaponData.AddRange(explosives);

            _melee.SelectedIndexChanged += (o, e) => { SelectedIndexChanged(melee[e.Index].weaponHash); };
            _pistols.SelectedIndexChanged += (o, e) => { SelectedIndexChanged(pistols[e.Index].weaponHash); };
            _rifles.SelectedIndexChanged += (o, e) => { SelectedIndexChanged(rifles[e.Index].weaponHash); };
            _mg.SelectedIndexChanged += (o, e) => { SelectedIndexChanged(mg[e.Index].weaponHash); };
            _shotguns.SelectedIndexChanged += (o, e) => { SelectedIndexChanged(shotguns[e.Index].weaponHash); };
            _snipers.SelectedIndexChanged += (o, e) => { SelectedIndexChanged(snipers[e.Index].weaponHash); };
            _heavy.SelectedIndexChanged += (o, e) => { SelectedIndexChanged(heavy[e.Index].weaponHash); };
            _explosives.SelectedIndexChanged += (o, e) => { SelectedIndexChanged(explosives[e.Index].weaponHash); };
        }

        private List<WeaponData> AddSubmenu(string path, NativeMenu parentMenu, bool melee = false)
        {
            string[] files = Directory.GetFiles(path);
            List<WeaponData> weaponData = new List<WeaponData>();

            for (int i = 0; i < files.Length; i++)
            {
                XElement document = XElement.Load(files[i]);
                string weaponName = (string)document.Element("Name");
                int weaponPrice = (int)document.Element("Price");
                NativeMenu weaponMenu = new NativeMenu(
                    weaponName, weaponName, $"Price: ${weaponPrice}"
                );
                uint weaponHash = (uint)document.Element("Hash");
                WeaponData data = new WeaponData
                {
                    weaponHash = weaponHash,
                    flags = new List<bool>()
                };

                if (!melee)
                {
                    NativeSliderItem ammoOptionItem = new NativeSliderItem("Ammo", 250, 1);

                    ammoOptionItem.Activated += (o, e) =>
                    {
                        AmmoPurchased(weaponHash, ammoOptionItem.Value);
                    };

                    ammoOptionItem.Selected += (o, e) =>
                    {
                        uint ammoType = Function.Call<uint>(Hash.GET_PED_AMMO_TYPE_FROM_WEAPON, Main.PPID, weaponHash);
                        ScriptSettings ini = ScriptSettings.Load("scripts\\LittleJacobMod.ini");
                        int price = ini.GetValue("AmmoPrices", ammoType.ToString(), 1);
                        ammoOptionItem.Description = $"Selected ammo: {ammoOptionItem.Value}\nPrice: ${price * ammoOptionItem.Value}";
                    };

                    ammoOptionItem.ValueChanged += (o, e) =>
                    {
                        uint ammoType = Function.Call<uint>(Hash.GET_PED_AMMO_TYPE_FROM_WEAPON, Main.PPID, weaponHash);
                        ScriptSettings ini = ScriptSettings.Load("scripts\\LittleJacobMod.ini");
                        int price = ini.GetValue("AmmoPrices", ammoType.ToString(), 1);
                        ammoOptionItem.Description = $"Selected ammo: {ammoOptionItem.Value}\nPrice: ${price * ammoOptionItem.Value}";
                    };

                    weaponMenu.Add(ammoOptionItem);
                }
                
                SubMenuData subMenuData = new SubMenuData(weaponHash);

                if ((bool)document.Element("Flags").Element("Tint"))
                {
                    XElement att = document.Element("Attachments");
                    IEnumerable<XElement> tints = att.Element("Tints").Descendants();
                    NativeMenu tintMenu = new NativeMenu("Weapon Tints", "Tints");
                    int size = tints.Count();

                    for (int n = 0; n < size; n++)
                    {
                        int ix = n;
                        XElement tint = tints.ElementAt(n);
                        NativeItem tintItem = new NativeItem(
                            (string)tint, $"Price: ${tint.Attribute("Price")}"
                            );
                        ItemData itemData = new ItemData();
                        itemData.item = tintItem;
                        itemData.price = (int)tint.Attribute("Price");
                        subMenuData.TintItems.Add(itemData);

                        tintItem.Activated += (o, e) =>
                        {
                            subMenuData.SetIndex(subMenuData.TintItems, "Tint", ix);
                            TintPurchased(weaponHash, ix);
                            //Main.LittleJacob.ProcessVoice(true);
                        };

                        tintMenu.Add(tintItem);
                    }

                    tintMenu.SelectedIndexChanged += (o, e) =>
                    {
                        TintChanged?.Invoke(this, e.Index);
                    };
    
                    tintMenu.Closed += (o, e) =>
                    {
                        SpawnWeaponObject?.Invoke(this, weaponHash);
                    };

                    weaponMenu.AddSubMenu(tintMenu);
                    Pool.Add(tintMenu);
                }

                if ((bool)document.Element("Flags").Element("Muzzle"))
                {
                    XElement att = document.Element("Attachments");
                    IEnumerable<XElement> muzzles = att.Element("Muzzle").Descendants();
                    NativeMenu attMenu = AddOption("Muzzle Attachments", "Muzzle", weaponHash, muzzles,
                    ComponentIndex.Muzzle, LoadoutSaving.SetMuzzle, subMenuData);
                    weaponMenu.AddSubMenu(attMenu);
                    Pool.Add(attMenu);
                    data.flags.Add(true);
                    data.muzzles = new List<uint>();
                    for (int c = 0; c < muzzles.Count(); c++)
                        data.muzzles.Add((uint)muzzles.ElementAt(c));
                } else
                    data.flags.Add(false);

                if ((bool)document.Element("Flags").Element("Clip"))
                {
                    XElement att = document.Element("Attachments");
                    IEnumerable<XElement> clips = att.Element("Clip").Descendants();
                    NativeMenu attMenu = AddOption("Weapon Clips", "Clip", weaponHash, clips,
                    ComponentIndex.Clip, LoadoutSaving.SetClip, subMenuData);
                    weaponMenu.AddSubMenu(attMenu);
                    Pool.Add(attMenu);
                    data.flags.Add(true);
                    data.clips = new List<uint>();
                    for (int c = 0; c < clips.Count(); c++)
                        data.clips.Add((uint)clips.ElementAt(c));
                } else
                    data.flags.Add(false);

                if ((bool)document.Element("Flags").Element("Barrel"))
                {
                    XElement att = document.Element("Attachments");
                    IEnumerable<XElement> barrels = att.Element("Barrel").Descendants();
                    NativeMenu attMenu = AddOption("Weapon Barrels", "Barrel", weaponHash, barrels,
                    ComponentIndex.Barrel, LoadoutSaving.SetBarrel, subMenuData);
                    weaponMenu.AddSubMenu(attMenu);
                    Pool.Add(attMenu);
                    data.flags.Add(true);
                    data.barrels = new List<uint>();
                    for (int c = 0; c < barrels.Count(); c++)
                        data.barrels.Add((uint)barrels.ElementAt(c));
                } else
                    data.flags.Add(false);

                if ((bool)document.Element("Flags").Element("Grip"))
                {
                    XElement att = document.Element("Attachments");
                    IEnumerable<XElement> grips = att.Element("Grip").Descendants();
                    NativeMenu attMenu = AddOption("Grip Attachments", "Grip", weaponHash, grips,
                    ComponentIndex.Grip, LoadoutSaving.SetGrip, subMenuData);
                    weaponMenu.AddSubMenu(attMenu);
                    Pool.Add(attMenu);
                    data.flags.Add(true);
                    data.grips = new List<uint>();
                    for (int c = 0; c < grips.Count(); c++)
                        data.grips.Add((uint)grips.ElementAt(c));
                } else
                    data.flags.Add(false);

                if ((bool)document.Element("Flags").Element("Scope"))
                {
                    XElement att = document.Element("Attachments");
                    IEnumerable<XElement> scopes = att.Element("Scope").Descendants();
                    NativeMenu attMenu = AddOption("Scope Attachments", "Scope", weaponHash, scopes,
                    ComponentIndex.Scope, LoadoutSaving.SetScope, subMenuData);
                    weaponMenu.AddSubMenu(attMenu);
                    Pool.Add(attMenu);
                    data.flags.Add(true);
                    data.scopes = new List<uint>();
                    for (int c = 0; c < scopes.Count(); c++)
                        data.scopes.Add((uint)scopes.ElementAt(c));
                } else
                    data.flags.Add(false);

                if ((bool)document.Element("Flags").Element("Camo"))
                {
                    XElement att = document.Element("Attachments");
                    IEnumerable<XElement> camos = att.Element("Camos").Element("Styles").Descendants();
                    NativeMenu attMenu = AddOption("Weapon Liveries", "Livery", weaponHash, camos,
                    ComponentIndex.Livery, LoadoutSaving.SetCamo, subMenuData);
                    weaponMenu.AddSubMenu(attMenu);
                    Pool.Add(attMenu);
                    data.camos = new List<uint>();
                    for (int c = 0; c < camos.Count(); c++)
                        data.camos.Add((uint)camos.ElementAt(c));

                    IEnumerable<XElement> camoColors = att.Element("Camos").Element("Colors").Descendants();
                    NativeMenu camoColorMenu = new NativeMenu("Livery colors", "Livery color");
                    int size = camoColors.Count();

                    for (int n = 0; n < size; n++)
                    {
                        int ix = n;
                        XElement camoColor = camoColors.ElementAt(n);
                        int price = (int)camoColor.Attribute("Price");
                        string colorName = (string)camoColor;
                        NativeItem camoColorItem = new NativeItem(colorName)
                        {
                            Description = $"Price: ${price}"
                        };
                        ItemData itemData = new ItemData();
                        itemData.price = price;
                        itemData.item = camoColorItem;
                        subMenuData.CamoColorItems.Add(itemData);

                        camoColorItem.Activated += (o, e) =>
                        {
                            subMenuData.SetIndex(subMenuData.CamoColorItems, colorName, ix);
                            CamoColorPurchased(weaponHash, ix, colorName, price);
                            //Main.LittleJacob.ProcessVoice(true);
                        };

                        camoColorMenu.Add(camoColorItem);
                    }

                    camoColorMenu.SelectedIndexChanged += (o, e) =>
                    {
                        var reference = LoadoutSaving.GetStoreReference(weaponHash);
                        int colorIndex = reference.GetCamoColor();

                        CamoColorEventArgs ev = new CamoColorEventArgs
                        {
                            Camo = reference.Camo,
                            ColorIndex = e.Index
                        };

                        CamoColorChanged?.Invoke(this, ev);
                    };

                    camoColorMenu.Closed += (o, e) =>
                    {
                        SpawnWeaponObject?.Invoke(this, weaponHash);
                    };

                    Pool.Add(camoColorMenu);
                    weaponMenu.AddSubMenu(camoColorMenu);
                    data.flags.Add(true);
                } else
                    data.flags.Add(false);

                if ((bool)document.Element("Flags").Element("Flashlight"))
                {
                    XElement att = document.Element("Attachments");
                    IEnumerable<XElement> flash = att.Element("Flashlight").Descendants();
                    NativeMenu attMenu = AddOption("Flashlights", "Flashlight", weaponHash, flash,
                    ComponentIndex.Flashlight, LoadoutSaving.SetFlashlight, subMenuData);
                    weaponMenu.AddSubMenu(attMenu);
                    Pool.Add(attMenu);
                    data.flags.Add(true);
                    data.flashlights = new List<uint>();
                    for (int c = 0; c < flash.Count(); c++)
                        data.flashlights.Add((uint)flash.ElementAt(c));
                } else
                    data.flags.Add(false);

                if ((bool)document.Element("Flags").Element("Varmod"))
                {
                    XElement att = document.Element("Attachments");
                    IEnumerable<XElement> varmods = att.Element("Varmod").Descendants();
                    NativeMenu attMenu = AddOption("Weapon Finishes", "Finish", weaponHash, varmods,
                    ComponentIndex.Varmod, LoadoutSaving.SetVarmod, subMenuData);
                    weaponMenu.AddSubMenu(attMenu);
                    Pool.Add(attMenu);
                    data.flags.Add(true);
                    data.varmods = new List<uint>();
                    for (int c = 0; c < varmods.Count(); c++)
                        data.varmods.Add((uint)varmods.ElementAt(c));
                } else
                    data.flags.Add(false);

                weaponMenu.Shown += (o, e) =>
                {
                    SpawnWeaponObject?.Invoke(this, weaponHash);
                    bool result = WeaponSelected((string)document.Element("Name"), weaponHash, 
                    (int)document.Element("Price"));

                    if (!result)
                        weaponMenu.Back();
                };

                _subMenus.Add(subMenuData);
                parentMenu.AddSubMenu(weaponMenu);
                Pool.Add(weaponMenu);
                weaponData.Add(data);
            }

            return weaponData;
        }

        public void ReloadOptions()
        {
            foreach (SubMenuData sub in _subMenus)
            {
                sub.LoadAttachments();
            }

            switch (Main.IsMPPed())
            {
                case 0:
                    _helm1.Description = HelmetState.MPMTVOwned ? "Select to equip" : "Price: $60000";
                    _helm2.Description = HelmetState.MPMNV1Owned ? "Select to equip" : "Price: $40000";
                    _helm3.Description = HelmetState.MPMNV2Owned ? "Select to equip" : "Price: $20000";
                    break;

                case 1:
                    _helm1.Description = HelmetState.MPFTVOwned ? "Select to equip" : "Price: $60000";
                    _helm2.Description = HelmetState.MPFNV1Owned ? "Select to equip" : "Price: $40000";
                    _helm3.Description = HelmetState.MPFNV2Owned ? "Select to equip" : "Price: $20000";
                    break;

                default:
                    _helm1.Description = "Not available for this character";
                    _helm2.Description = "Not available for this character";
                    _helm3.Description = "Not available for this character";
                    break;
            }
        }

        int HelmetType(int helmet, int pedType)
        {
            if (pedType == 1)
            {
                if (helmet == 115 || helmet == 116)
                    return 1;
                if (helmet == 117 || helmet == 118)
                    return 0;
                if (helmet == 146 || helmet == 147)
                    return 2;
            } else
            {
                if (helmet == 116 || helmet == 117)
                    return 4;
                if (helmet == 118 || helmet == 119)
                    return 3;
                if (helmet == 147 || helmet == 148)
                    return 5;
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

            for (int i = 0; i < colors.Count; i++)
            {
                int index = i;
                NativeItem item = new NativeItem(colors[i], $"Price: ${price}");

                item.Activated += (o, e) =>
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

            menu.Shown += (o, e) =>
            {
                int pedType = Main.IsMPPed();

                if (pedType == -1)
                {
                    menu.Back();
                    GTA.UI.Notification.Show("~y~Not available for this character!");
                    return;
                }

                int compIndx = code;

                if (pedType == 0)
                {
                    compIndx += 1;
                }

                int helmIndx = Function.Call<int>(Hash.GET_PED_PROP_INDEX, Main.PPID, 0);
                bool purchased = false;

                if (compIndx != helmIndx)
                {
                    switch(HelmetType(compIndx, pedType))
                    {
                        case 0:
                            if (!HelmetState.MPFTVOwned)
                            {
                                if (Game.Player.Money < helmPrice)
                                {
                                    GTA.UI.Notification.Show("~r~Not enought money.");
                                    menu.Back();
                                    return;
                                }
                                menu.Description = "Select to equip";
                                HelmetState.MPFTVOwned = true;
                                Game.Player.Money -= helmPrice;
                                purchased = true;
                            }
                            break;
                        case 1:
                            if (!HelmetState.MPFNV1Owned)
                            {
                                if (Game.Player.Money < helmPrice)
                                {
                                    GTA.UI.Notification.Show("~r~Not enought money.");
                                    menu.Back();
                                    return;
                                }
                                menu.Description = "Select to equip";
                                HelmetState.MPFNV1Owned = true;
                                Game.Player.Money -= helmPrice;
                                purchased = true;
                            }
                            break;
                        case 2:
                            if (!HelmetState.MPFNV2Owned)
                            {
                                if (Game.Player.Money < helmPrice)
                                {
                                    GTA.UI.Notification.Show("~r~Not enought money.");
                                    menu.Back();
                                    return;
                                }
                                menu.Description = "Select to equip";
                                HelmetState.MPFNV2Owned = true;
                                Game.Player.Money -= helmPrice;
                                purchased = true;
                            }
                            break;
                        case 3:
                            if (!HelmetState.MPMTVOwned)
                            {
                                if (Game.Player.Money < helmPrice)
                                {
                                    GTA.UI.Notification.Show("~r~Not enought money.");
                                    menu.Back();
                                    return;
                                }
                                menu.Description = "Select to equip";
                                HelmetState.MPMTVOwned = true;
                                Game.Player.Money -= helmPrice;
                                purchased = true;
                            }
                            break;
                        case 4:
                            if (!HelmetState.MPMNV1Owned)
                            {
                                if (Game.Player.Money < helmPrice)
                                {
                                    GTA.UI.Notification.Show("~r~Not enought money.");
                                    menu.Back();
                                    return;
                                }
                                menu.Description = "Select to equip";
                                HelmetState.MPMNV1Owned = true;
                                Game.Player.Money -= helmPrice;
                                purchased = true;
                            }
                            break;
                        case 5:
                            if (!HelmetState.MPMNV2Owned)
                            {
                                if (Game.Player.Money < helmPrice)
                                {
                                    GTA.UI.Notification.Show("~r~Not enought money.");
                                    menu.Back();
                                    return;
                                }
                                menu.Description = "Select to equip";
                                HelmetState.MPMNV2Owned = true;
                                Game.Player.Money -= helmPrice;
                                purchased = true;
                            }
                            break;
                        default:
                            break;
                    }

                    if (compIndx - 1 == helmIndx)
                    {
                        _helmColor = Function.Call<int>(Hash.GET_PED_PROP_TEXTURE_INDEX, Main.PPID, compIndx - 1);
                    } 
                    else
                    {
                        _helmColor = 0;
                    }

                    Function.Call(Hash.SET_PED_PROP_INDEX, Main.PPID, 0, compIndx, _helmColor, 1);
                }

                Game.IsNightVisionActive = false;
                Game.IsThermalVisionActive = false;
                GTA.UI.Notification.Show($"~g~{menu.Title.Text} {(purchased ? "Purchased" : "Equipped")}"); 
            };

            menu.Closed += (o, e) =>
            {
                int helmIndx = Function.Call<int>(Hash.GET_PED_PROP_INDEX, Main.PPID, 0);
                Function.Call(Hash.SET_PED_PROP_INDEX, Main.PPID, 0, helmIndx, _helmColor, 1);
            };

            menu.SelectedIndexChanged += (o, e) =>
            {
                int helmIndx = Function.Call<int>(Hash.GET_PED_PROP_INDEX, Main.PPID, 0);
                Function.Call(Hash.SET_PED_PROP_INDEX, Main.PPID, 0, helmIndx, e.Index, 1);
            };
        }

        private void SelectedIndexChanged(uint weapon)
        {
            SpawnWeaponObject?.Invoke(this, weapon);
        }

        public void ShowMainMenu()
        {
            _mainMenu.Visible = true;
        }

        bool WeaponSelected(string name, uint weapon, int price)
        {
            bool hasWeapon = Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON, Main.PPID, weapon, 0);

            if (!hasWeapon)
            {
                if (Game.Player.Money < price)
                {
                    GTA.UI.Notification.Show("You don't have enough money!");
                    return false;
                }

                GTA.UI.Notification.Show($"{name} purchased!");
                //Main.LittleJacob.ProcessVoice(true, true);
                Game.Player.Money -= price;
                Function.Call<bool>(Hash.GIVE_WEAPON_TO_PED, Main.PPID, weapon, 1, true, true);
            } else
            {
                Function.Call(Hash.SET_CURRENT_PED_WEAPON, Main.PPID, weapon, true);
            }

            LoadoutSaving.AddWeapon(weapon);

            return true;
        }

        NativeMenu AddOption(string title, string subtitle, uint weaponHash, IEnumerable<XElement> elements, 
        ComponentIndex compIndex, Action<uint, uint> OnSuccess, SubMenuData subMenuData)
        {
            NativeMenu atcmntMenu = new NativeMenu(title, subtitle);
            int size = elements.Count();

            for (int i = 0; i < size; i++)
            {
                int ix = i;
                XElement att = elements.ElementAt(i);
                string name = (string)att.Attribute("Name");
                int price = (int)att.Attribute("Price");
                uint attHash = (uint)att;

                NativeItem menuItem = new NativeItem(name)
                {
                    Description = $"Price: ${price}"
                };

                ItemData itemData = new ItemData
                {
                    Hash = attHash,
                    item = menuItem,
                    price = price
                };

                switch (compIndex)
                {
                    case ComponentIndex.Livery:
                        subMenuData.CamoItems.Add(itemData);
                        break;
                    case ComponentIndex.Scope:
                        subMenuData.ScopeItems.Add(itemData);
                        break;
                    case ComponentIndex.Clip:
                        subMenuData.ClipItems.Add(itemData);
                        break;
                    case ComponentIndex.Muzzle:
                        subMenuData.MuzzleItems.Add(itemData);
                        break;
                    case ComponentIndex.Flashlight:
                        subMenuData.FlashlightItems.Add(itemData);
                        break;
                    case ComponentIndex.Barrel:
                        subMenuData.BarrelItems.Add(itemData);
                        break;
                    case ComponentIndex.Grip:
                        subMenuData.GripItems.Add(itemData);
                        break;
                    case ComponentIndex.Varmod:
                        subMenuData.VarmodItems.Add(itemData);
                        break;
                }

                menuItem.Activated += (o, e) =>
                {
                    if (!ComponentPurchased(weaponHash, attHash, price, name, elements, OnSuccess))
                        return;
                    
                    switch (compIndex)
                    {
                        case ComponentIndex.Livery:
                            subMenuData.SetIndex(subMenuData.CamoItems, "Livery", ix);
                            break;
                        case ComponentIndex.Scope:
                            subMenuData.SetIndex(subMenuData.ScopeItems, "Scope", ix);
                            break;
                        case ComponentIndex.Clip:
                            subMenuData.SetIndex(subMenuData.ClipItems, "Clip", ix);
                            break;
                        case ComponentIndex.Muzzle:
                            subMenuData.SetIndex(subMenuData.MuzzleItems, "Muzzle", ix);
                            break;
                        case ComponentIndex.Flashlight:
                            subMenuData.SetIndex(subMenuData.FlashlightItems, "Flashlight", ix);
                            break;
                        case ComponentIndex.Barrel:
                            subMenuData.SetIndex(subMenuData.BarrelItems, "Barrel", ix);
                            break;
                        case ComponentIndex.Grip:
                            subMenuData.SetIndex(subMenuData.GripItems, "Grip", ix);
                            break;
                        case ComponentIndex.Varmod:
                            subMenuData.SetIndex(subMenuData.VarmodItems, "Finish", ix);
                            break;
                    }

                    //Main.LittleJacob.ProcessVoice(true);
                };

                atcmntMenu.Add(menuItem);
            }

            atcmntMenu.SelectedIndexChanged += (o, e) =>
            {
                ComponentSelected?.Invoke(this,
                    new ComponentPreviewEventArgs(weaponHash,
                    (uint)elements.ElementAt(e.Index), compIndex));
            };

            atcmntMenu.Closed += (o, e) =>
            {
                SpawnWeaponObject?.Invoke(this, weaponHash);
            };

            return atcmntMenu;
        }

        void AmmoPurchased(uint weapon, int value)
        {
            int maxAmmo;
            unsafe
            {
                Function.Call(Hash.GET_MAX_AMMO, Main.PPID, weapon, &maxAmmo);
            }
            int purchasableAmmo = maxAmmo - Function.Call<int>(Hash.GET_AMMO_IN_PED_WEAPON, Main.PPID, weapon);

            if (purchasableAmmo == 0)
            {
                GTA.UI.Notification.Show("Max ammo capacity reached!");
                return;
            }

            int ammoToPurchase = value >= purchasableAmmo ? value - purchasableAmmo : value;
            uint ammoType = Function.Call<uint>(Hash.GET_PED_AMMO_TYPE_FROM_WEAPON, Main.PPID, weapon);
            ScriptSettings ini = ScriptSettings.Load("scripts\\LittleJacobMod.ini");
            int price = ini.GetValue("AmmoPrices", ammoType.ToString(), 1);

            if (price == 0)
                price = 50000;

            int ammoPrice = ammoToPurchase * price;

            if (Game.Player.Money < ammoPrice)
            {
                GTA.UI.Notification.Show("Can't purchase ammo. Not enough money!");
                return;
            }

            Game.Player.Money -= ammoPrice;
            Function.Call(Hash._ADD_AMMO_TO_PED_BY_TYPE, Main.PPID, ammoType, ammoToPurchase);
            LoadoutSaving.SetAmmo(weapon, Function.Call<int>(Hash.GET_AMMO_IN_PED_WEAPON, Main.PPID, weapon));
        }

        bool TintPurchased(uint weapon, int index)
        {
            int price = 5000;
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
            LoadoutSaving.SetTint(weapon, index);
            return true;
        }

        bool ComponentPurchased(uint weapon, uint component, int price, string name, 
            IEnumerable<XElement> elements, Action<uint, uint> OnSuccess)
        {
            price = ApplyDiscount(price);

            if (Game.Player.Money < price)
            {
                GTA.UI.Notification.Show($"Couldn't purchase {name}. Not enough money!");
                return false;
            }

            Game.Player.Money -= price;

            if (name.Contains("None"))
            {
                int size = elements.Count();

                for (int i = 0; i < size; i++)
                {
                    uint hash = (uint)elements.ElementAt(i);
                    
                    if (hash != component && Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON_COMPONENT, Main.PPID, weapon, hash))
                    {
                        Function.Call(Hash.REMOVE_WEAPON_COMPONENT_FROM_PED, Main.PPID, weapon, hash);
                    }
                }

                GTA.UI.Notification.Show($"Attachments removed!");
                OnSuccess(weapon, (uint)WeaponComponentHash.Invalid);
            }
            else
            {
                Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, Main.PPID, weapon, component);
                uint slide = TintsAndCamos.ReturnSlide(component);

                if (slide != (uint)WeaponComponentHash.Invalid)
                {
                    Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, Main.PPID, weapon, slide);
                }
                
                GTA.UI.Notification.Show($"{name} purchased!");
                OnSuccess(weapon, component);
            }

            return true;
        }

        bool CamoColorPurchased(uint weapon, int index, string name, int price)
        {
            Saving.Utils.StoredWeapon sw = LoadoutSaving.GetStoreReference(weapon);

            if (sw == null)
                return false;

            if (sw.Camo == (uint)WeaponComponentHash.Invalid)
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

            var storedWeapon = LoadoutSaving.GetStoreReference(weapon);
            Game.Player.Money -= price;
            Function.Call(Hash._SET_PED_WEAPON_LIVERY_COLOR, Main.PPID, weapon, storedWeapon.Camo, index);
            uint slide = TintsAndCamos.ReturnSlide(storedWeapon.Camo);
            
            if (slide != (uint)WeaponComponentHash.Invalid)
            {
                Function.Call(Hash._SET_PED_WEAPON_LIVERY_COLOR, Main.PPID, weapon, slide, index);
            }

            GTA.UI.Notification.Show("Camo color purchased!");
            LoadoutSaving.SetCamoColor(weapon, index);
            return true;
        }

        int ApplyDiscount(int price)
        {
            bool canApply = false;

            if (Function.Call<bool>(Hash.IS_PED_MODEL, Main.PPID, PedHash.Michael) && MissionSaving.MProgress >= 4)
                canApply = true;
            else if (Function.Call<bool>(Hash.IS_PED_MODEL, Main.PPID, PedHash.Franklin) && MissionSaving.FProgress >= 4)
                canApply = true;
            else if (Function.Call<bool>(Hash.IS_PED_MODEL, Main.PPID, PedHash.Trevor) && MissionSaving.TUnlocked)
                canApply = true;

            if (canApply)
            {
                float disc = price * 0.20f;
                return (int)(price - disc);
            }
            else
                return price;
        }
    }
}
