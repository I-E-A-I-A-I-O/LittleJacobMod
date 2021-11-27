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

        NativeMenu mainMenu;
        NativeMenu melee;
        NativeMenu pistols;
        NativeMenu rifles;
        NativeMenu mg;
        NativeMenu snipers;
        NativeMenu heavy;
        NativeMenu shotguns;
        NativeMenu explosives;

        NativeItem _currentItem;
        NativeItem _ownedItem;

        public static event EventHandler<ComponentPreviewEventArgs> ComponentSelected;
        public static event EventHandler<WeaponHash> SpawnWeaponObject;
        public static event EventHandler<int> TintChanged;
        public static event EventHandler<CamoColorEventArgs> CamoColorChanged;
        public static event EventHandler<WeaponHash> ReloadComponents;

        public Menu()
        {
            Pool = new ObjectPool();
            mainMenu = new NativeMenu("Little Jacob", "Weapon store");
            melee = new NativeMenu("Melee", "Melee weapons");
            pistols = new NativeMenu("Pistols", "Pistols");
            rifles = new NativeMenu("Rifles", "Rifles");
            mg = new NativeMenu("Machine guns", "Machine guns");
            snipers = new NativeMenu("Sniper rifles", "Sniper rifles");
            shotguns = new NativeMenu("Shotguns", "Shotguns");
            explosives = new NativeMenu("Explosives", "Explosives");
            heavy = new NativeMenu("Heavy Weapons", "Heavy Weapons");
            var armorOption = new NativeItem("Armor", "Buy armor");

            Pool.Add(mainMenu);
            Pool.Add(melee);
            Pool.Add(pistols);
            Pool.Add(rifles);
            Pool.Add(mg);
            Pool.Add(snipers);
            Pool.Add(explosives);
            Pool.Add(shotguns);
            Pool.Add(heavy);

            mainMenu.AddSubMenu(melee);
            mainMenu.AddSubMenu(pistols);
            mainMenu.AddSubMenu(rifles);
            mainMenu.AddSubMenu(shotguns);
            mainMenu.AddSubMenu(mg);
            mainMenu.AddSubMenu(snipers);
            mainMenu.AddSubMenu(heavy);
            mainMenu.AddSubMenu(explosives);
            mainMenu.Add(armorOption);

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

            AddSubmenu(WeaponsList.Melee, melee, false, true);
            AddSubmenu(WeaponsList.Pistols, pistols);
            AddSubmenu(WeaponsList.SMGs, mg);
            AddSubmenu(WeaponsList.Rifles, rifles);
            AddSubmenu(WeaponsList.Shotguns, shotguns);
            AddSubmenu(WeaponsList.Snipers, snipers);
            AddSubmenu(WeaponsList.Heavy, heavy);
            AddSubmenu(WeaponsList.Explosives, explosives, true);

            melee.SelectedIndexChanged += (o, e) => { SelectedIndexChanged(WeaponsList.Melee[e.Index]); };
            pistols.SelectedIndexChanged += (o, e) => { SelectedIndexChanged(WeaponsList.Pistols[e.Index]); };
            rifles.SelectedIndexChanged += (o, e) => { SelectedIndexChanged(WeaponsList.Rifles[e.Index]); };
            mg.SelectedIndexChanged += (o, e) => { SelectedIndexChanged(WeaponsList.SMGs[e.Index]); };
            shotguns.SelectedIndexChanged += (o, e) => { SelectedIndexChanged(WeaponsList.Shotguns[e.Index]); };
            snipers.SelectedIndexChanged += (o, e) => { SelectedIndexChanged(WeaponsList.Snipers[e.Index]); };
            heavy.SelectedIndexChanged += (o, e) => { SelectedIndexChanged(WeaponsList.Heavy[e.Index]); };
            explosives.SelectedIndexChanged += (o, e) => { SelectedIndexChanged(WeaponsList.Explosives[e.Index]); };
        }

        private void AddSubmenu(List<Utils.Weapons.Weapon> weaponGroup, NativeMenu parentMenu, bool isExplosive = false, bool isMelee = false)
        {
            for (var i = 0; i < weaponGroup.Count; i++)
            {
                var displayName = weaponGroup[i].Name;
                var subMenu = new NativeMenu(displayName, displayName)
                {
                    Description = $"Price: ${weaponGroup[i].Price}"
                };

                AddOptions(weaponGroup[i], subMenu, isExplosive, isMelee);

                subMenu.Shown += (o, e) =>
                {
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
            mainMenu.Visible = true;
        }

        bool WeaponSelected(Utils.Weapons.Weapon weapon, int price, NativeMenu menu)
        {
            var hasWeapon = Game.Player.Character.Weapons.HasWeapon(weapon.WeaponHash);
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
                Game.Player.Character.Weapons.Give(weapon.WeaponHash, 0, false, false);
            }

            Game.Player.Character.Weapons.Select(weapon.WeaponHash, true);
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

            var storeRef = LoadoutSaving.GetStoreReference(weapon.WeaponHash);

            if (weapon.WeaponHash != WeaponHash.PericoPistol && weapon.WeaponHash != WeaponHash.DoubleActionRevolver && weapon.WeaponHash != WeaponHash.NavyRevolver)
            {
                var tintsMenu = new NativeMenu("Tint", "Tint");
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
                    var ix = i;

                    var tintItem = new NativeItem(tints[i])
                    {
                        Description = "Price: $5000"
                    };

                    if (i == index)
                    {
                        tintItem.Enabled = false;
                        tintItem.Description = "Current tint";
                    }

                    tintItem.Activated += (o, e) =>
                    {
                        TintPurchased(weapon.WeaponHash, ix);
                        Main.LittleJacob.ProcessVoice(true);
                    };

                    tintsMenu.Add(tintItem);
                }

                tintsMenu.SelectedIndexChanged += (o, e) =>
                {
                    _currentItem = tintsMenu.Items[e.Index];
                    _ownedItem = tintsMenu.Items[LoadoutSaving.GetStoreReference(weapon.WeaponHash).GetTintIndex()];
                    
                    TintChanged?.Invoke(this, e.Index);
                };

                tintsMenu.Closed += (o, e) =>
                {
                    ReloadComponents?.Invoke(this, weapon.WeaponHash);
                };

                Pool.Add(tintsMenu);
                menu.AddSubMenu(tintsMenu);
            }

            if (weapon.HasClip)
            {
                AddOption("Clip", storeRef, weapon, menu, weapon.Clips, ComponentIndex.Clip, LoadoutSaving.SetClip);
            }

            if (weapon.HasMuzzleOrSupp)
            {
                AddOption("Muzzle", storeRef, weapon, menu, weapon.MuzzlesAndSupps, ComponentIndex.Muzzle, LoadoutSaving.SetMuzzle);
            }

            if (weapon.HasFlaslight)
            {
                AddOption("Flashlight", storeRef, weapon, menu, weapon.FlashLight, ComponentIndex.Flashlight, LoadoutSaving.SetFlashlight);
            }

            if (weapon.HasScope)
            {
                AddOption("Scope", storeRef, weapon, menu, weapon.Scopes, ComponentIndex.Scope, LoadoutSaving.SetScope);
            }

            if (weapon.HasGrip)
            {
                AddOption("Grip", storeRef, weapon, menu, weapon.Grips, ComponentIndex.Grip, LoadoutSaving.SetGrip);
            }

            if (weapon.HasBarrel)
            {
                AddOption("Barrel", storeRef, weapon, menu, weapon.Barrels, ComponentIndex.Barrel, LoadoutSaving.SetBarrel);
            }

            if (weapon.HasCamo)
            {
                AddOption("Livery", storeRef, weapon, menu, weapon.Camos, ComponentIndex.Camo, LoadoutSaving.SetCamo, true);

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

                    if (i == index)
                    {
                        camoColorItem.Enabled = false;
                        camoColorItem.Description = "Current livery color";
                    }

                    camoColorItem.Activated += (o, e) =>
                    {
                        CamoColorPurchased(weapon.WeaponHash, ix);
                        Main.LittleJacob.ProcessVoice(true);
                    };

                    camoColorMenu.Add(camoColorItem);
                }

                camoColorMenu.SelectedIndexChanged += (o, e) =>
                {
                    var reference = LoadoutSaving.GetStoreReference(weapon.WeaponHash);
                    _currentItem = camoColorMenu.Items[e.Index];

                    var colorIndex = reference.GetCamoColor();
                    _ownedItem = colorIndex == -1 ? null : camoColorMenu.Items[colorIndex];

                    var ev = new CamoColorEventArgs
                    {
                        Camo = reference.Camo,
                        ColorIndex = e.Index
                    };

                    CamoColorChanged?.Invoke(this, ev);
                };

                camoColorMenu.Closed += (o, e) =>
                {
                    ReloadComponents?.Invoke(this, weapon.WeaponHash);
                };

                Pool.Add(camoColorMenu);
                menu.AddSubMenu(camoColorMenu);
            }
        }

        void AddOption(string title, Saving.Utils.StoredWeapon storeRef, Utils.Weapons.Weapon weapon, NativeMenu menu, Dictionary<string, WeaponComponentHash> componentGroup, ComponentIndex compIndex, Action<WeaponHash, WeaponComponentHash> OnSuccess, bool isCamo = false)
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
                var ix = i;
                string[] namePrice;

                if (isCamo)
                {
                    namePrice = new string[] { componentGroup.Keys.ElementAt(i), " $60000" };
                } else
                {
                    namePrice = componentGroup.Keys.ElementAt(i).Split('-');
                }

                var menuItem = new NativeItem(namePrice[0])
                {
                    Description = $"Price:{namePrice[1]}"
                };

                if (i == index)
                {
                    menuItem.Enabled = false;
                    menuItem.Description = "Current attachment";
                }

                menuItem.Activated += (o, e) =>
                {
                    ComponentPurchased(weapon.WeaponHash, componentGroup.ElementAt(ix), componentGroup.Values.ToList(), OnSuccess, isCamo);
                    Main.LittleJacob.ProcessVoice(true);
                };

                atcmntMenu.Add(menuItem);
            }

            atcmntMenu.SelectedIndexChanged += (o, e) =>
            {
                ComponentSelected?.Invoke(this,
                    new ComponentPreviewEventArgs(weapon.WeaponHash,
                    componentGroup.ElementAt(e.Index).Value, compIndex));

                _currentItem = atcmntMenu.Items[e.Index];

                int curItem = componentGroup.Values.ToList().IndexOf(GetCurrentAttachment(weapon.WeaponHash, compIndex));
                curItem = curItem == -1 ? 0 : curItem;

                _ownedItem = atcmntMenu.Items[curItem];
            };

            atcmntMenu.Closed += (o, e) =>
            {
                ReloadComponents?.Invoke(this, weapon.WeaponHash);
            };

            Pool.Add(atcmntMenu);
            menu.AddSubMenu(atcmntMenu);
        }

        WeaponComponentHash GetCurrentAttachment(WeaponHash weaponHash, ComponentIndex compIndex)
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

                case ComponentIndex.Camo:
                    return storeRef.Camo;

                case ComponentIndex.Scope:
                    return storeRef.Scope;

                case ComponentIndex.Barrel:
                    return storeRef.Barrel;

                default:
                    return WeaponComponentHash.Invalid;
            }
        }

        WeaponComponentHash GetCurrentAttachment(Saving.Utils.StoredWeapon storeRef, ComponentIndex compIndex)
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

                case ComponentIndex.Camo:
                    return storeRef.Camo;

                case ComponentIndex.Scope:
                    return storeRef.Scope;

                case ComponentIndex.Barrel:
                    return storeRef.Barrel;

                default:
                    return WeaponComponentHash.Invalid;
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
            var ammoType = Function.Call<uint>(Hash.GET_PED_AMMO_TYPE_FROM_WEAPON, Game.Player.Character.Handle, weapon.Hash);
            Function.Call(Hash._ADD_AMMO_TO_PED_BY_TYPE, Game.Player.Character.Handle, ammoType, ammoToPurchase);
            LoadoutSaving.SetAmmo(weapon.Hash, weapon.Ammo);
        }

        void TintPurchased(WeaponHash weapon, int index)
        {
            var price = 5000;
            if (Game.Player.Money < price)
            {
                GTA.UI.Notification.Show("Couldn't purchase this tint. Not enough money!");
                return;
            }

            _ownedItem.Enabled = true;
            _ownedItem.Description = "Price: $5000";
            _currentItem.Enabled = false;
            _currentItem.Description = "Current Tint";
            
            Game.Player.Money -= price;

            TintChanged?.Invoke(this, index);
            Function.Call(Hash.SET_PED_WEAPON_TINT_INDEX, Game.Player.Character.Handle, weapon, index);

            GTA.UI.Notification.Show("Tint purchased!");
            LoadoutSaving.SetTint(weapon, index);
        }

        void ComponentPurchased(WeaponHash weapon, KeyValuePair<string, WeaponComponentHash> weaponComponent, List<WeaponComponentHash> components, Action<WeaponHash, WeaponComponentHash> OnSuccess, bool isCamo = false)
        {
            var price = isCamo ? 60000 : int.Parse(weaponComponent.Key.Split('$')[1]);
            var name = isCamo ? weaponComponent.Key : weaponComponent.Key.Split('-')[0];

            if (Game.Player.Money < price)
            {
                GTA.UI.Notification.Show($"Couldn't purchase {name}. Not enough money!");
                return;
            }

            Game.Player.Money -= price;

            if (weaponComponent.Key.Contains("None"))
            {
                foreach (WeaponComponentHash component in components)
                {
                    if (weaponComponent.Value != component && Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON_COMPONENT, Game.Player.Character.Handle, weapon, component))
                    {
                        Function.Call(Hash.REMOVE_WEAPON_COMPONENT_FROM_PED, Game.Player.Character.Handle, weapon, component);
                    }
                }

                GTA.UI.Notification.Show($"Attachments removed!");
                OnSuccess(weapon, WeaponComponentHash.Invalid);
            }
            else
            {
                Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, Game.Player.Character.Handle, weapon, weaponComponent.Value);
                
                if (isCamo)
                {
                    var slide = TintsAndCamos.ReturnSlide(weaponComponent.Value);

                    if (slide != WeaponComponentHash.Invalid)
                    {
                        Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, Game.Player.Character.Handle, weapon, slide);
                    }
                }
                
                GTA.UI.Notification.Show($"{name} purchased!");
                OnSuccess(weapon, weaponComponent.Value);
            }

            _ownedItem.Enabled = true;
            _ownedItem.Description = $"Price:{(isCamo ? " $60000" : weaponComponent.Key.Split('-')[1])}";
            _currentItem.Enabled = false;
            _currentItem.Description = "Current Attachment";
        }

        void CamoColorPurchased(WeaponHash weapon, int index)
        {
            if (!LoadoutSaving.HasCamo(weapon))
            {
                GTA.UI.Notification.Show("Buy a camo first!");
                return;
            }

            var price = 10000;

            if (Game.Player.Money < price)
            {
                GTA.UI.Notification.Show("Couldn't purchase camo color. Not enough money!");
                return;
            }

            var storedWeapon = LoadoutSaving.GetStoreReference(weapon);

            Game.Player.Money -= price;

            Function.Call(Hash._SET_PED_WEAPON_LIVERY_COLOR, Game.Player.Character.Handle, weapon, storedWeapon.Camo, index);

            var slide = TintsAndCamos.ReturnSlide(storedWeapon.Camo);
            
            if (slide != WeaponComponentHash.Invalid)
            {
                Function.Call(Hash._SET_PED_WEAPON_LIVERY_COLOR, Game.Player.Character.Handle, weapon, slide, index);
            }

            GTA.UI.Notification.Show("Camo color purchased!");
            LoadoutSaving.SetCamoColor(weapon, index);

            _ownedItem.Enabled = true;
            _ownedItem.Description = $"Price: $60000";
            _currentItem.Enabled = false;
            _currentItem.Description = "Current livery color";
        }
    }
}
