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
            Pool = new ObjectPool();
            _mainMenu = new NativeMenu("Little Jacob", "Weapon store");
            NativeMenu melee = new NativeMenu("Melee", "Melee weapons");
            NativeMenu pistols = new NativeMenu("Pistols", "Pistols");
            NativeMenu rifles = new NativeMenu("Rifles", "Rifles");
            NativeMenu mg = new NativeMenu("Machine guns", "Machine guns");
            NativeMenu snipers = new NativeMenu("Sniper rifles", "Sniper rifles");
            NativeMenu shotguns = new NativeMenu("Shotguns", "Shotguns");
            NativeMenu explosives = new NativeMenu("Explosives", "Explosives");
            NativeMenu heavy = new NativeMenu("Heavy Weapons", "Heavy Weapons");
            NativeMenu gearMenu = new NativeMenu("Gear", "Gear");
            NativeItem armorOption = new NativeItem("Armor", "Buy armor");
            _helm1 = new NativeMenu("Thermal Vision Helmet", "Thermal Vision Helmet", "Price: $60000");
            _helm2 = new NativeMenu("Night Vision Helmet", "Night Vision Helmet", "Price: $40000");
            _helm3 = new NativeMenu("Tactical Night Vision", "Tactical Night Vision", "Price: $20000");

            Pool.Add(_mainMenu);
            Pool.Add(melee);
            Pool.Add(pistols);
            Pool.Add(rifles);
            Pool.Add(mg);
            Pool.Add(snipers);
            Pool.Add(explosives);
            Pool.Add(shotguns);
            Pool.Add(heavy);
            Pool.Add(gearMenu);
            Pool.Add(_helm1);
            Pool.Add(_helm2);
            Pool.Add(_helm3);

            _mainMenu.AddSubMenu(melee);
            _mainMenu.AddSubMenu(pistols);
            _mainMenu.AddSubMenu(rifles);
            _mainMenu.AddSubMenu(shotguns);
            _mainMenu.AddSubMenu(mg);
            _mainMenu.AddSubMenu(snipers);
            _mainMenu.AddSubMenu(heavy);
            _mainMenu.AddSubMenu(explosives);
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
            List<WeaponData> meleeList = AddSubmenu($"{baseDir}\\Normal\\Melee", melee, true);
            List<WeaponData> pistolList = AddSubmenu($"{baseDir}\\Normal\\Pistols", pistols);
            List<WeaponData> pistols2 = AddSubmenu($"{baseDir}\\MK2\\Pistols", pistols);
            List<WeaponData> mgList = AddSubmenu($"{baseDir}\\Normal\\Machine Guns", mg);
            List<WeaponData> mg2 = AddSubmenu($"{baseDir}\\MK2\\Machine Guns", mg);
            List<WeaponData> rifleList = AddSubmenu($"{baseDir}\\Normal\\Rifles", rifles);
            List<WeaponData> rifles2 = AddSubmenu($"{baseDir}\\MK2\\Rifles", rifles);
            List<WeaponData> shotgunList = AddSubmenu($"{baseDir}\\Normal\\Shotguns", shotguns);
            List<WeaponData> shotguns2 = AddSubmenu($"{baseDir}\\MK2\\Shotguns", shotguns);
            List<WeaponData> sniperList = AddSubmenu($"{baseDir}\\Normal\\Snipers", snipers);
            List<WeaponData> snipers2 = AddSubmenu($"{baseDir}\\MK2\\Snipers", snipers);
            List<WeaponData> heavyList = AddSubmenu($"{baseDir}\\Normal\\Heavy", heavy);
            List<WeaponData> explosiveList = AddSubmenu($"{baseDir}\\Normal\\Explosives", explosives);

            pistolList.AddRange(pistols2);
            mgList.AddRange(mg2);
            rifleList.AddRange(rifles2);
            shotgunList.AddRange(shotguns2);
            sniperList.AddRange(snipers2);

            Mapper.WeaponData.AddRange(meleeList);
            Mapper.WeaponData.AddRange(pistolList);
            Mapper.WeaponData.AddRange(mgList);
            Mapper.WeaponData.AddRange(rifleList);
            Mapper.WeaponData.AddRange(shotgunList);
            Mapper.WeaponData.AddRange(sniperList);
            Mapper.WeaponData.AddRange(heavyList);
            Mapper.WeaponData.AddRange(explosiveList);

            melee.SelectedIndexChanged += (o, e) => { SelectedIndexChanged(meleeList[e.Index].WeaponHash); };
            pistols.SelectedIndexChanged += (o, e) => { SelectedIndexChanged(pistolList[e.Index].WeaponHash); };
            rifles.SelectedIndexChanged += (o, e) => { SelectedIndexChanged(rifleList[e.Index].WeaponHash); };
            mg.SelectedIndexChanged += (o, e) => { SelectedIndexChanged(mgList[e.Index].WeaponHash); };
            shotguns.SelectedIndexChanged += (o, e) => { SelectedIndexChanged(shotgunList[e.Index].WeaponHash); };
            snipers.SelectedIndexChanged += (o, e) => { SelectedIndexChanged(sniperList[e.Index].WeaponHash); };
            heavy.SelectedIndexChanged += (o, e) => { SelectedIndexChanged(heavyList[e.Index].WeaponHash); };
            explosives.SelectedIndexChanged += (o, e) => { SelectedIndexChanged(explosiveList[e.Index].WeaponHash); };
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
                    weaponName, weaponName, $"Price: ${weaponPrice.ToString()}"
                );
                uint weaponHash = (uint)document.Element("Hash");
                WeaponData data = new WeaponData
                {
                    WeaponHash = weaponHash,
                    Flags = new List<bool>()
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
                        ammoOptionItem.Description = $"Selected ammo: {ammoOptionItem.Value.ToString()}\nPrice: ${(price * ammoOptionItem.Value).ToString()}";
                    };

                    ammoOptionItem.ValueChanged += (o, e) =>
                    {
                        uint ammoType = Function.Call<uint>(Hash.GET_PED_AMMO_TYPE_FROM_WEAPON, Main.PPID, weaponHash);
                        ScriptSettings ini = ScriptSettings.Load("scripts\\LittleJacobMod.ini");
                        int price = ini.GetValue("AmmoPrices", ammoType.ToString(), 1);
                        ammoOptionItem.Description = $"Selected ammo: {ammoOptionItem.Value.ToString()}\nPrice: ${(price * ammoOptionItem.Value).ToString()}";
                    };

                    weaponMenu.Add(ammoOptionItem);
                }
                
                SubMenuData subMenuData = new SubMenuData(weaponHash);

                if ((bool)document.Element("Flags").Element("Tint"))
                {
                    XElement att = document.Element("Attachments");
                    IEnumerable<XElement> tints = att.Element("Tints").Descendants();
                    NativeMenu tintMenu = new NativeMenu("Weapon Tints", "Tints");
                    var xElements = tints.ToList();
                    int size = xElements.Count();

                    for (int n = 0; n < size; n++)
                    {
                        int ix = n;
                        XElement tint = xElements.ElementAt(n);
                        NativeItem tintItem = new NativeItem(
                            (string)tint, $"Price: ${tint.Attribute("Price")}"
                            );
                        ItemData itemData = new ItemData();
                        itemData.Item = tintItem;
                        itemData.Price = (int)tint.Attribute("Price");
                        subMenuData.TintItems.Add(itemData);

                        tintItem.Activated += (o, e) =>
                        {
                            SubMenuData.SetIndex(subMenuData.TintItems, "Tint", ix);
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
                    data.Flags.Add(true);
                    data.Muzzles = new List<uint>();
                    for (int c = 0; c < muzzles.Count(); c++)
                        data.Muzzles.Add((uint)muzzles.ElementAt(c));
                } else
                    data.Flags.Add(false);

                if ((bool)document.Element("Flags").Element("Clip"))
                {
                    XElement att = document.Element("Attachments");
                    IEnumerable<XElement> clips = att.Element("Clip").Descendants();
                    NativeMenu attMenu = AddOption("Weapon Clips", "Clip", weaponHash, clips,
                    ComponentIndex.Clip, LoadoutSaving.SetClip, subMenuData);
                    weaponMenu.AddSubMenu(attMenu);
                    Pool.Add(attMenu);
                    data.Flags.Add(true);
                    data.Clips = new List<uint>();
                    for (int c = 0; c < clips.Count(); c++)
                        data.Clips.Add((uint)clips.ElementAt(c));
                } else
                    data.Flags.Add(false);

                if ((bool)document.Element("Flags").Element("Barrel"))
                {
                    XElement att = document.Element("Attachments");
                    IEnumerable<XElement> barrels = att.Element("Barrel").Descendants();
                    NativeMenu attMenu = AddOption("Weapon Barrels", "Barrel", weaponHash, barrels,
                    ComponentIndex.Barrel, LoadoutSaving.SetBarrel, subMenuData);
                    weaponMenu.AddSubMenu(attMenu);
                    Pool.Add(attMenu);
                    data.Flags.Add(true);
                    data.Barrels = new List<uint>();
                    for (int c = 0; c < barrels.Count(); c++)
                        data.Barrels.Add((uint)barrels.ElementAt(c));
                } else
                    data.Flags.Add(false);

                if ((bool)document.Element("Flags").Element("Grip"))
                {
                    XElement att = document.Element("Attachments");
                    IEnumerable<XElement> grips = att.Element("Grip").Descendants();
                    NativeMenu attMenu = AddOption("Grip Attachments", "Grip", weaponHash, grips,
                    ComponentIndex.Grip, LoadoutSaving.SetGrip, subMenuData);
                    weaponMenu.AddSubMenu(attMenu);
                    Pool.Add(attMenu);
                    data.Flags.Add(true);
                    data.Grips = new List<uint>();
                    for (int c = 0; c < grips.Count(); c++)
                        data.Grips.Add((uint)grips.ElementAt(c));
                } else
                    data.Flags.Add(false);

                if ((bool)document.Element("Flags").Element("Scope"))
                {
                    XElement att = document.Element("Attachments");
                    IEnumerable<XElement> scopes = att.Element("Scope").Descendants();
                    NativeMenu attMenu = AddOption("Scope Attachments", "Scope", weaponHash, scopes,
                    ComponentIndex.Scope, LoadoutSaving.SetScope, subMenuData);
                    weaponMenu.AddSubMenu(attMenu);
                    Pool.Add(attMenu);
                    data.Flags.Add(true);
                    data.Scopes = new List<uint>();
                    for (int c = 0; c < scopes.Count(); c++)
                        data.Scopes.Add((uint)scopes.ElementAt(c));
                } else
                    data.Flags.Add(false);

                if ((bool)document.Element("Flags").Element("Camo"))
                {
                    XElement att = document.Element("Attachments");
                    IEnumerable<XElement> camos = att.Element("Camos").Element("Styles").Descendants();
                    NativeMenu attMenu = AddOption("Weapon Liveries", "Livery", weaponHash, camos,
                    ComponentIndex.Livery, LoadoutSaving.SetCamo, subMenuData);
                    weaponMenu.AddSubMenu(attMenu);
                    Pool.Add(attMenu);
                    data.Camos = new List<uint>();
                    for (int c = 0; c < camos.Count(); c++)
                        data.Camos.Add((uint)camos.ElementAt(c));

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
                            Description = $"Price: ${price.ToString()}"
                        };
                        ItemData itemData = new ItemData();
                        itemData.Price = price;
                        itemData.Item = camoColorItem;
                        subMenuData.CamoColorItems.Add(itemData);

                        camoColorItem.Activated += (o, e) =>
                        {
                            SubMenuData.SetIndex(subMenuData.CamoColorItems, colorName, ix);
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
                    data.Flags.Add(true);
                } else
                    data.Flags.Add(false);

                if ((bool)document.Element("Flags").Element("Flashlight"))
                {
                    XElement att = document.Element("Attachments");
                    IEnumerable<XElement> flash = att.Element("Flashlight").Descendants();
                    NativeMenu attMenu = AddOption("Flashlights", "Flashlight", weaponHash, flash,
                    ComponentIndex.Flashlight, LoadoutSaving.SetFlashlight, subMenuData);
                    weaponMenu.AddSubMenu(attMenu);
                    Pool.Add(attMenu);
                    data.Flags.Add(true);
                    data.Flashlights = new List<uint>();
                    for (int c = 0; c < flash.Count(); c++)
                        data.Flashlights.Add((uint)flash.ElementAt(c));
                } else
                    data.Flags.Add(false);

                if ((bool)document.Element("Flags").Element("Varmod"))
                {
                    XElement att = document.Element("Attachments");
                    IEnumerable<XElement> varmods = att.Element("Varmod").Descendants();
                    NativeMenu attMenu = AddOption("Weapon Finishes", "Finish", weaponHash, varmods,
                    ComponentIndex.Varmod, LoadoutSaving.SetVarmod, subMenuData);
                    weaponMenu.AddSubMenu(attMenu);
                    Pool.Add(attMenu);
                    data.Flags.Add(true);
                    data.Varmods = new List<uint>();
                    for (int c = 0; c < varmods.Count(); c++)
                        data.Varmods.Add((uint)varmods.ElementAt(c));
                } else
                    data.Flags.Add(false);

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
                NativeItem item = new NativeItem(colors[i], $"Price: ${price.ToString()}");

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
                int pedType = Main.IsMPped();

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

        private static bool WeaponSelected(string name, uint weapon, int price)
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

        private NativeMenu AddOption(string title, string subtitle, uint weaponHash, IEnumerable<XElement> elements, 
        ComponentIndex compIndex, Action<uint, uint> onSuccess, SubMenuData subMenuData)
        {
            NativeMenu atcmntMenu = new NativeMenu(title, subtitle);
            var xElements = elements.ToList();
            int size = xElements.Count();

            for (int i = 0; i < size; i++)
            {
                int ix = i;
                XElement att = xElements.ElementAt(i);
                string name = (string)att.Attribute("Name");
                int price = (int)att.Attribute("Price");
                uint attHash = (uint)att;

                NativeItem menuItem = new NativeItem(name)
                {
                    Description = $"Price: ${price.ToString()}"
                };

                ItemData itemData = new ItemData
                {
                    Hash = attHash,
                    Item = menuItem,
                    Price = price
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
                    if (!ComponentPurchased(weaponHash, attHash, price, name, xElements, onSuccess))
                        return;
                    
                    switch (compIndex)
                    {
                        case ComponentIndex.Livery:
                            SubMenuData.SetIndex(subMenuData.CamoItems, "Livery", ix);
                            break;
                        case ComponentIndex.Scope:
                            SubMenuData.SetIndex(subMenuData.ScopeItems, "Scope", ix);
                            break;
                        case ComponentIndex.Clip:
                            SubMenuData.SetIndex(subMenuData.ClipItems, "Clip", ix);
                            break;
                        case ComponentIndex.Muzzle:
                            SubMenuData.SetIndex(subMenuData.MuzzleItems, "Muzzle", ix);
                            break;
                        case ComponentIndex.Flashlight:
                            SubMenuData.SetIndex(subMenuData.FlashlightItems, "Flashlight", ix);
                            break;
                        case ComponentIndex.Barrel:
                            SubMenuData.SetIndex(subMenuData.BarrelItems, "Barrel", ix);
                            break;
                        case ComponentIndex.Grip:
                            SubMenuData.SetIndex(subMenuData.GripItems, "Grip", ix);
                            break;
                        case ComponentIndex.Varmod:
                            SubMenuData.SetIndex(subMenuData.VarmodItems, "Finish", ix);
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
                    (uint)xElements.ElementAt(e.Index), compIndex));
            };

            atcmntMenu.Closed += (o, e) =>
            {
                SpawnWeaponObject?.Invoke(this, weaponHash);
            };

            return atcmntMenu;
        }

        private void AmmoPurchased(uint weapon, int value)
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

        private bool TintPurchased(uint weapon, int index)
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

        private bool ComponentPurchased(uint weapon, uint component, int price, string name, 
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

        private bool CamoColorPurchased(uint weapon, int index, string name, int price)
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

        private int ApplyDiscount(int price)
        {
            bool canApply = false;

            if (Function.Call<bool>(Hash.IS_PED_MODEL, Main.PPID, (uint)PedHash.Michael) && MissionSaving.MProgress >= 4)
                canApply = true;
            else if (Function.Call<bool>(Hash.IS_PED_MODEL, Main.PPID, (uint)PedHash.Franklin) && MissionSaving.FProgress >= 4)
                canApply = true;
            else if (Function.Call<bool>(Hash.IS_PED_MODEL, Main.PPID, (uint)PedHash.Trevor) && MissionSaving.TUnlocked)
                canApply = true;

            if (!canApply) return price;
            float disc = price * 0.20f;
            return (int)(price - disc);

        }
    }
}
