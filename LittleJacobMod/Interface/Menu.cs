using System;
using System.Collections.Generic;
using System.Linq;
using LemonUI;
using LemonUI.Menus;
using GTA;
using GTA.Native;
using LittleJacobMod.Utils;
using LittleJacobMod.Utils.Weapons;
using LittleJacobMod.Saving;

namespace LittleJacobMod.Interface
{
    public class Menu
    {
        public ObjectPool Pool { get; private set; }
        List<SubMenuData> _subMenus = new List<SubMenuData>();
        NativeMenu _mainMenu;
        NativeMenu _helm1;
        NativeMenu _helm2;
        NativeMenu _helm3;
        int _helmColor;

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

            AddSubmenu(WeaponsList.Melee, _melee, false, true);
            AddSubmenu(WeaponsList.Pistols, _pistols);
            AddSubmenu(WeaponsList.SMGs, _mg);
            AddSubmenu(WeaponsList.Rifles, _rifles);
            AddSubmenu(WeaponsList.Shotguns, _shotguns);
            AddSubmenu(WeaponsList.Snipers, _snipers);
            AddSubmenu(WeaponsList.Heavy, _heavy);
            AddSubmenu(WeaponsList.Explosives, _explosives, true);

            _melee.SelectedIndexChanged += (o, e) => { SelectedIndexChanged(WeaponsList.Melee[e.Index]); };
            _pistols.SelectedIndexChanged += (o, e) => { SelectedIndexChanged(WeaponsList.Pistols[e.Index]); };
            _rifles.SelectedIndexChanged += (o, e) => { SelectedIndexChanged(WeaponsList.Rifles[e.Index]); };
            _mg.SelectedIndexChanged += (o, e) => { SelectedIndexChanged(WeaponsList.SMGs[e.Index]); };
            _shotguns.SelectedIndexChanged += (o, e) => { SelectedIndexChanged(WeaponsList.Shotguns[e.Index]); };
            _snipers.SelectedIndexChanged += (o, e) => { SelectedIndexChanged(WeaponsList.Snipers[e.Index]); };
            _heavy.SelectedIndexChanged += (o, e) => { SelectedIndexChanged(WeaponsList.Heavy[e.Index]); };
            _explosives.SelectedIndexChanged += (o, e) => { SelectedIndexChanged(WeaponsList.Explosives[e.Index]); };
        }

        public void ReloadOptions()
        {
            foreach (SubMenuData sub in _subMenus)
            {
                sub.RestartLists();
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
                HelmetMenuChanged?.Invoke(this, true);
            };

            menu.Closed += (o, e) =>
            {
                int helmIndx = Function.Call<int>(Hash.GET_PED_PROP_INDEX, Main.PPID, 0);
                Function.Call(Hash.SET_PED_PROP_INDEX, Main.PPID, 0, helmIndx, _helmColor, 1);
                HelmetMenuChanged?.Invoke(this, false);
            };

            menu.SelectedIndexChanged += (o, e) =>
            {
                int helmIndx = Function.Call<int>(Hash.GET_PED_PROP_INDEX, Main.PPID, 0);
                Function.Call(Hash.SET_PED_PROP_INDEX, Main.PPID, 0, helmIndx, e.Index, 1);
            };
        }

        private void AddSubmenu(List<Utils.Weapons.Weapon> weaponGroup, NativeMenu parentMenu, bool isExplosive = false, bool isMelee = false)
        {
            for (int i = 0; i < weaponGroup.Count; i++)
            {
                string displayName = weaponGroup[i].Name;
                NativeMenu subMenu = new NativeMenu(displayName, displayName)
                {
                    Description = $"Price: ${weaponGroup[i].Price}"
                };

                AddOptions(weaponGroup[i], subMenu, isExplosive, isMelee);

                subMenu.Shown += (o, e) =>
                {
                    SpawnWeaponObject?.Invoke(this, weaponGroup[parentMenu.SelectedIndex].WeaponHash);
                    WeaponSelected(weaponGroup[parentMenu.SelectedIndex], weaponGroup[parentMenu.SelectedIndex].Price, subMenu);
                };

                Pool.Add(subMenu);
                parentMenu.AddSubMenu(subMenu);
            }
        }

        private void SelectedIndexChanged(Utils.Weapons.Weapon weapon)
        {
            SpawnWeaponObject?.Invoke(this, weapon.WeaponHash);
        }

        public void ShowMainMenu()
        {
            _mainMenu.Visible = true;
        }

        bool WeaponSelected(Utils.Weapons.Weapon weapon, int price, NativeMenu menu)
        {
            bool hasWeapon = Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON, Main.PPID, weapon.WeaponHash, 0);
            if (!hasWeapon)
            {
                if (Game.Player.Money < price)
                {
                    GTA.UI.Notification.Show("You don't have enough money!");
                    menu.Back();
                    return false;
                }
                GTA.UI.Notification.Show($"{weapon.Name} purchased!");
                Main.LittleJacob.ProcessVoice(true, true);
                Game.Player.Money -= price;
                Function.Call<bool>(Hash.GIVE_WEAPON_TO_PED, Main.PPID, weapon.WeaponHash, 0, true, true);
            }

            var currentWeapon = Game.Player.Character.Weapons.Current;
            LoadoutSaving.AddWeapon(currentWeapon);

            return true;
        }

        void AddOptions(Utils.Weapons.Weapon weapon, NativeMenu menu, bool isExplosive, bool isMelee)
        {
            if (isMelee)
            {
                return;
            }

            var ammoOptionItem = new NativeSliderItem("Ammo", 250, 1);
            ammoOptionItem.Activated += (o, e) => 
            {
                AmmoPurchased(Game.Player.Character.Weapons.Current, ammoOptionItem.Value);
            };
            menu.Add(ammoOptionItem);

            if (isExplosive)
            {
                return;
            }

            SubMenuData menuData = new SubMenuData(weapon.WeaponHash);
            var storeRef = LoadoutSaving.GetStoreReference(weapon.WeaponHash);

            if (weapon.WeaponHash != 1470379660 && weapon.WeaponHash != 2548703416 && weapon.WeaponHash != 2441047180)
            {
                NativeMenu tintsMenu = new NativeMenu("Tint", "Tint");
                int index;

                if (storeRef != null)
                {
                    index = storeRef.GetTintIndex();
                    index = index == -1 ? 0 : index;
                } else
                {
                    index = 0;
                }

                int tintCount = Function.Call<int>(Hash.GET_WEAPON_TINT_COUNT, weapon.WeaponHash);
                List<string> tints;

                if (tintCount > 9)
                {
                    tints = TintsAndCamos.TintsMk2;
                }
                else
                {
                    tints = TintsAndCamos.Tints;
                }

                for (int i = 0; i < tints.Count; i++)
                {
                    int ix = i;
                    NativeItem tintItem = new NativeItem(tints[i])
                    {
                        Description = "Price: $5000"
                    };
                    menuData.TintItems.Add(tintItem);

                    if (i == index)
                    {
                        tintItem.Enabled = false;
                        tintItem.Description = "Current tint";
                    }

                    tintItem.Activated += (o, e) =>
                    {
                        menuData.SetListIndex(menuData.TintItems, "Tint", 5000, ix);
                        TintPurchased(weapon.WeaponHash, ix);
                        Main.LittleJacob.ProcessVoice(true);
                    };

                    tintsMenu.Add(tintItem);
                }

                tintsMenu.SelectedIndexChanged += (o, e) =>
                {
                    TintChanged?.Invoke(this, e.Index);
                };

                tintsMenu.Closed += (o, e) =>
                {
                    SpawnWeaponObject?.Invoke(this, weapon.WeaponHash);
                };

                Pool.Add(tintsMenu);
                menu.AddSubMenu(tintsMenu);
            }

            if (weapon.HasClip)
            {
                AddOption("Clip", storeRef, weapon, menu, weapon.Clips, ComponentIndex.Clip, LoadoutSaving.SetClip, menuData);
            }

            if (weapon.HasMuzzleOrSupp)
            {
                AddOption("Muzzle", storeRef, weapon, menu, weapon.MuzzlesAndSupps, ComponentIndex.Muzzle, LoadoutSaving.SetMuzzle, menuData);
            }

            if (weapon.HasFlaslight)
            {
                AddOption("Flashlight", storeRef, weapon, menu, weapon.FlashLight, ComponentIndex.Flashlight, LoadoutSaving.SetFlashlight, menuData);
            }

            if (weapon.HasScope)
            {
                AddOption("Scope", storeRef, weapon, menu, weapon.Scopes, ComponentIndex.Scope, LoadoutSaving.SetScope, menuData);
            }

            if (weapon.HasGrip)
            {
                AddOption("Grip", storeRef, weapon, menu, weapon.Grips, ComponentIndex.Grip, LoadoutSaving.SetGrip, menuData);
            }

            if (weapon.HasBarrel)
            {
                AddOption("Barrel", storeRef, weapon, menu, weapon.Barrels, ComponentIndex.Barrel, LoadoutSaving.SetBarrel, menuData);
            }

            if (weapon.HasCamo)
            {
                AddOption("Livery", storeRef, weapon, menu, weapon.Camos, ComponentIndex.Livery, LoadoutSaving.SetCamo, menuData);

                var camoColorMenu = new NativeMenu("Livery color", "Livery color");
                int index;

                if (storeRef != null)
                {
                    index = storeRef.GetCamoColor();
                    index = index == -1 ? 0 : index;
                }
                else
                {
                    index = 0;
                }

                for (int i = 0; i < TintsAndCamos.CamoColor.Count; i++)
                {
                    var ix = i;

                    var camoColorItem = new NativeItem(TintsAndCamos.CamoColor[i])
                    {
                        Description = "Price: $10000"
                    };
                    menuData.CamoColorItems.Add(camoColorItem);

                    if (i == index)
                    {
                        camoColorItem.Enabled = false;
                        camoColorItem.Description = "Current livery color";
                    }

                    camoColorItem.Activated += (o, e) =>
                    {
                        menuData.SetListIndex(menuData.CamoColorItems, "Livery color", 10000, ix);
                        CamoColorPurchased(weapon.WeaponHash, ix);
                        Main.LittleJacob.ProcessVoice(true);
                    };

                    camoColorMenu.Add(camoColorItem);
                }

                camoColorMenu.SelectedIndexChanged += (o, e) =>
                {
                    var reference = LoadoutSaving.GetStoreReference(weapon.WeaponHash);
                    var colorIndex = reference.GetCamoColor();

                    var ev = new CamoColorEventArgs
                    {
                        Camo = reference.Camo,
                        ColorIndex = e.Index
                    };

                    CamoColorChanged?.Invoke(this, ev);
                };

                camoColorMenu.Closed += (o, e) =>
                {
                    SpawnWeaponObject?.Invoke(this, weapon.WeaponHash);
                };

                Pool.Add(camoColorMenu);
                menu.AddSubMenu(camoColorMenu);
            }

            _subMenus.Add(menuData);
        }

        void AddOption(string title, Saving.Utils.StoredWeapon storeRef, Utils.Weapons.Weapon weapon, NativeMenu menu, Dictionary<string, uint> componentGroup, ComponentIndex compIndex, Action<uint, uint> OnSuccess, SubMenuData subMenuData)
        {
            var atcmntMenu = new NativeMenu(title, title);
            int index;

            if (storeRef != null)
            {
                index = componentGroup.Values.ToList().IndexOf(GetCurrentAttachment(storeRef, compIndex));
                index = index == -1 ? 0 : index;
            }
            else
            {
                index = 0;
            }

            for (int i = 0; i < componentGroup.Count; i++)
            {
                int ix = i;
                string[] namePrice;

                if (compIndex == ComponentIndex.Livery)
                {
                    namePrice = new string[] { componentGroup.Keys.ElementAt(i), " $60000" };
                } else
                {
                    namePrice = componentGroup.Keys.ElementAt(i).Split('-');
                }

                NativeItem menuItem = new NativeItem(namePrice[0])
                {
                    Description = $"Price:{namePrice[1]}"
                };

                switch (compIndex)
                {
                    case ComponentIndex.Livery:
                        subMenuData.CamoItems.Add(menuItem);
                        break;
                    case ComponentIndex.Scope:
                        subMenuData.ScopeItems.Add(namePrice[1], menuItem);
                        break;
                    case ComponentIndex.Clip:
                        subMenuData.ClipItems.Add(namePrice[1], menuItem);
                        break;
                    case ComponentIndex.Muzzle:
                        subMenuData.MuzzleItems.Add(namePrice[1], menuItem);
                        break;
                    case ComponentIndex.Flashlight:
                        subMenuData.FlashlightItems.Add(namePrice[1], menuItem);
                        break;
                    case ComponentIndex.Barrel:
                        subMenuData.BarrelItems.Add(namePrice[1], menuItem);
                        break;
                    case ComponentIndex.Grip:
                        subMenuData.GripItems.Add(namePrice[1], menuItem);
                        break;
                }

                if (i == index)
                {
                    menuItem.Enabled = false;
                    menuItem.Description = $"Current {compIndex}";
                }

                menuItem.Activated += (o, e) =>
                {
                    switch (compIndex)
                    {
                        case ComponentIndex.Livery:
                            subMenuData.SetListIndex(subMenuData.CamoItems, "Livery", 60000, ix);
                            break;
                        case ComponentIndex.Scope:
                            subMenuData.SetDictIndex(subMenuData.ScopeItems, "Scope", ix);
                            break;
                        case ComponentIndex.Clip:
                            subMenuData.SetDictIndex(subMenuData.ClipItems, "Clip", ix);
                            break;
                        case ComponentIndex.Muzzle:
                            subMenuData.SetDictIndex(subMenuData.MuzzleItems, "Muzzle", ix);
                            break;
                        case ComponentIndex.Flashlight:
                            subMenuData.SetDictIndex(subMenuData.FlashlightItems, "Flashlight", ix);
                            break;
                        case ComponentIndex.Barrel:
                            subMenuData.SetDictIndex(subMenuData.BarrelItems, "Barrel", ix);
                            break;
                        case ComponentIndex.Grip:
                            subMenuData.SetDictIndex(subMenuData.GripItems, "Grip", ix);
                            break;
                    }

                    ComponentPurchased(weapon.WeaponHash, componentGroup.ElementAt(ix), componentGroup.Values.ToList(), OnSuccess, compIndex == ComponentIndex.Livery);
                    Main.LittleJacob.ProcessVoice(true);
                };

                atcmntMenu.Add(menuItem);
            }

            atcmntMenu.SelectedIndexChanged += (o, e) =>
            {
                ComponentSelected?.Invoke(this,
                    new ComponentPreviewEventArgs(weapon.WeaponHash,
                    componentGroup.ElementAt(e.Index).Value, compIndex));

                int curItem = componentGroup.Values.ToList().IndexOf(GetCurrentAttachment(weapon.WeaponHash, compIndex));
                curItem = curItem == -1 ? 0 : curItem;
            };

            atcmntMenu.Closed += (o, e) =>
            {
                SpawnWeaponObject?.Invoke(this, weapon.WeaponHash);
            };

            Pool.Add(atcmntMenu);
            menu.AddSubMenu(atcmntMenu);
        }

        uint GetCurrentAttachment(uint weaponHash, ComponentIndex compIndex)
        {
            var storeRef = LoadoutSaving.GetStoreReference(weaponHash);
            switch (compIndex)
            {
                case ComponentIndex.Clip:
                    return storeRef.Clip;

                case ComponentIndex.Muzzle:
                    return storeRef.Muzzle;

                case ComponentIndex.Flashlight:
                    return storeRef.Flashlight;

                case ComponentIndex.Grip:
                    return storeRef.Grip;

                case ComponentIndex.Livery:
                    return storeRef.Camo;

                case ComponentIndex.Scope:
                    return storeRef.Scope;

                case ComponentIndex.Barrel:
                    return storeRef.Barrel;

                default:
                    return (uint)WeaponComponentHash.Invalid;
            }
        }

        uint GetCurrentAttachment(Saving.Utils.StoredWeapon storeRef, ComponentIndex compIndex)
        {
            switch (compIndex)
            {
                case ComponentIndex.Clip:
                    return storeRef.Clip;

                case ComponentIndex.Muzzle:
                    return storeRef.Muzzle;

                case ComponentIndex.Flashlight:
                    return storeRef.Flashlight;

                case ComponentIndex.Grip:
                    return storeRef.Grip;

                case ComponentIndex.Livery:
                    return storeRef.Camo;

                case ComponentIndex.Scope:
                    return storeRef.Scope;

                case ComponentIndex.Barrel:
                    return storeRef.Barrel;

                default:
                    return (uint)WeaponComponentHash.Invalid;
            }
        }

        void AmmoPurchased(GTA.Weapon weapon, int value)
        {
            var purchasableAmmo = weapon.MaxAmmo - weapon.Ammo;

            if (purchasableAmmo == 0)
            {
                GTA.UI.Notification.Show("Max ammo capacity reached!");
                return;
            }

            var ammoToPurchase = value >= purchasableAmmo ? value - purchasableAmmo : value;
            var ammoPrice = ammoToPurchase * 2;

            if (Game.Player.Money < ammoPrice)
            {
                GTA.UI.Notification.Show("Can't purchase ammo. Not enough money!");
                return;
            }

            Game.Player.Money -= ammoPrice;
            var ammoType = Function.Call<uint>(Hash.GET_PED_AMMO_TYPE_FROM_WEAPON, Main.PPID, weapon.Hash);
            Function.Call(Hash._ADD_AMMO_TO_PED_BY_TYPE, Main.PPID, ammoType, ammoToPurchase);
            LoadoutSaving.SetAmmo((uint)weapon.Hash, weapon.Ammo);
        }

        void TintPurchased(uint weapon, int index)
        {
            int price = 5000;
            if (Game.Player.Money < price)
            {
                GTA.UI.Notification.Show("Couldn't purchase this tint. Not enough money!");
                return;
            }
            
            Game.Player.Money -= price;
            TintChanged?.Invoke(this, index);
            Function.Call(Hash.SET_PED_WEAPON_TINT_INDEX, Main.PPID, weapon, index);
            GTA.UI.Notification.Show("Tint purchased!");
            LoadoutSaving.SetTint(weapon, index);
        }

        void ComponentPurchased(uint weapon, KeyValuePair<string, uint> weaponComponent, List<uint> components, Action<uint, uint> OnSuccess, bool isCamo = false)
        {
            int price = isCamo ? 60000 : int.Parse(weaponComponent.Key.Split('$')[1]);
            string name = isCamo ? weaponComponent.Key : weaponComponent.Key.Split('-')[0];

            if (Game.Player.Money < price)
            {
                GTA.UI.Notification.Show($"Couldn't purchase {name}. Not enough money!");
                return;
            }

            Game.Player.Money -= price;

            if (weaponComponent.Key.Contains("None"))
            {
                foreach (uint component in components)
                {
                    if (weaponComponent.Value != component && Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON_COMPONENT, Main.PPID, weapon, component))
                    {
                        Function.Call(Hash.REMOVE_WEAPON_COMPONENT_FROM_PED, Main.PPID, weapon, component);
                    }
                }

                GTA.UI.Notification.Show($"Attachments removed!");
                OnSuccess(weapon, (uint)WeaponComponentHash.Invalid);
            }
            else
            {
                Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, Main.PPID, weapon, weaponComponent.Value);
                
                if (isCamo)
                {
                    var slide = TintsAndCamos.ReturnSlide(weaponComponent.Value);

                    if (slide != (uint)WeaponComponentHash.Invalid)
                    {
                        Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, Main.PPID, weapon, slide);
                    }
                }
                
                GTA.UI.Notification.Show($"{name} purchased!");
                OnSuccess(weapon, weaponComponent.Value);
            }
        }

        void CamoColorPurchased(uint weapon, int index)
        {
            if (!LoadoutSaving.HasCamo(weapon))
            {
                GTA.UI.Notification.Show("Buy a camo first!");
                return;
            }

            int price = 10000;

            if (Game.Player.Money < price)
            {
                GTA.UI.Notification.Show("Couldn't purchase camo color. Not enough money!");
                return;
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
        }
    }
}
