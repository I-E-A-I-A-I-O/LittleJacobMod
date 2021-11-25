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
                var tintsMenu = new NativeMenu("Tints", "Tints");
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
                var clipsMenu = new NativeMenu("Clips", "Clips");
                int index;

                if (storeRef != null)
                {
                    index = weapon.Clips.Values.ToList().IndexOf(storeRef.Clip);
                    index = index == -1 ? 0 : index;
                }
                else
                {
                    index = 0;
                }

                for (int i = 0; i < weapon.Clips.Count; i++)
                {
                    var ix = i;
                    var namePrice = weapon.Clips.Keys.ElementAt(i).Split('-');
                    var clipItem = new NativeItem(namePrice[0])
                    {
                        Description = $"Price:{namePrice[1]}"
                    };

                    if (i == index)
                    {
                        clipItem.Enabled = false;
                        clipItem.Description = "Current clip";
                    }

                    clipItem.Activated += (o, e) =>
                    {
                        ComponentPurchased(weapon.WeaponHash, weapon.Clips.ElementAt(ix), null, LoadoutSaving.SetClip);
                        Main.LittleJacob.ProcessVoice(true);
                    };

                    clipsMenu.Add(clipItem);
                }

                clipsMenu.SelectedIndexChanged += (o, e) =>
                {
                    ComponentSelected?.Invoke(this, 
                        new ComponentPreviewEventArgs(weapon.WeaponHash, 
                        weapon.Clips.ElementAt(e.Index).Value, ComponentIndex.Clip));

                    _currentItem = clipsMenu.Items[e.Index];
                    
                    int curClip = weapon.Clips.Values.ToList().IndexOf(LoadoutSaving.GetStoreReference(weapon.WeaponHash).Clip);
                    curClip = curClip == -1 ? 0 : curClip;

                    _ownedItem = clipsMenu.Items[curClip];
                };

                clipsMenu.Closed += (o, e) =>
                {
                    ReloadComponents?.Invoke(this, weapon.WeaponHash);
                };

                Pool.Add(clipsMenu);
                menu.AddSubMenu(clipsMenu);
            }

            if (weapon.HasMuzzleOrSupp)
            {
                var muzzleMenu = new NativeMenu("Muzzle and suppressors", "Muzzle and suppressors");
                int index;

                if (storeRef != null)
                {
                    index = weapon.MuzzlesAndSupps.Values.ToList().IndexOf(storeRef.Muzzle);
                    index = index == -1 ? 0 : index;
                }
                else
                {
                    index = 0;
                }

                for (int i = 0; i < weapon.MuzzlesAndSupps.Count; i++)
                {
                    var ix = i;
                    var namePrice = weapon.MuzzlesAndSupps.Keys.ElementAt(i).Split('-');
                    var muzzleItem = new NativeItem(namePrice[0])
                    {
                        Description = $"Price:{namePrice[1]}"
                    };

                    if (i == index)
                    {
                        muzzleItem.Enabled = false;
                        muzzleItem.Description = "Current muzzle attachment";
                    }

                    muzzleItem.Activated += (o, e) =>
                    {
                        ComponentPurchased(weapon.WeaponHash, weapon.MuzzlesAndSupps.ElementAt(ix), weapon.MuzzlesAndSupps.Values.ToList(), LoadoutSaving.SetMuzzle);
                        Main.LittleJacob.ProcessVoice(true);
                    };

                    muzzleMenu.Add(muzzleItem);
                }

                muzzleMenu.SelectedIndexChanged += (o, e) =>
                {
                    ComponentSelected?.Invoke(this,
                        new ComponentPreviewEventArgs(weapon.WeaponHash,
                        weapon.MuzzlesAndSupps.ElementAt(e.Index).Value, ComponentIndex.Muzzle));

                    _currentItem = muzzleMenu.Items[e.Index];

                    int curMuzz = weapon.MuzzlesAndSupps.Values.ToList().IndexOf(LoadoutSaving.GetStoreReference(weapon.WeaponHash).Muzzle);
                    curMuzz = curMuzz == -1 ? 0 : curMuzz;

                    _ownedItem = muzzleMenu.Items[curMuzz];
                };

                muzzleMenu.Closed += (o, e) =>
                {
                    ReloadComponents?.Invoke(this, weapon.WeaponHash);
                };

                Pool.Add(muzzleMenu);
                menu.AddSubMenu(muzzleMenu);
            }

            /*if (weapon.HasFlaslight)
            {
                var flashSlider = new NativeListItem<string>("Flashlight");

                for (int i = 0; i < weapon.FlashLight.Count; i++)
                {
                    flashSlider.Add(weapon.FlashLight.ElementAt(i).Key);
                }

                var index = weapon.FlashLight.Values.ToList().IndexOf(storeRef.Flashlight);
                flashSlider.SelectedIndex = index == -1 ? 0 : index;

                flashSlider.Activated += (o, e) =>
                {
                    ComponentPurchased(weapon.WeaponHash, weapon.FlashLight.ElementAt(flashSlider.SelectedIndex), weapon.FlashLight.Values.ToList(), LoadoutSaving.SetFlashlight);
                    Main.LittleJacob.ProcessVoice(true);
                };

                flashSlider.ItemChanged += (o, e) =>
                {
                    ComponentSelected?.Invoke(this, new ComponentPreviewEventArgs(weapon.WeaponHash,
                        weapon.FlashLight.ElementAt(flashSlider.SelectedIndex).Value, ComponentIndex.Flashlight));

                    _lastItem = flashSlider;
                    _installedIndex = weapon.FlashLight.Values.ToList().IndexOf(LoadoutSaving.GetStoreReference(weapon.WeaponHash).Flashlight);
                    _itemChanged = true;
                };

                menu.Add(flashSlider);
            }

            if (weapon.HasScope)
            {
                var scopeSlider = new NativeListItem<string>("Scopes");

                for (int i = 0; i < weapon.Scopes.Count; i++)
                {
                    scopeSlider.Add(weapon.Scopes.ElementAt(i).Key);
                }

                var index = weapon.Scopes.Values.ToList().IndexOf(storeRef.Scope);
                scopeSlider.SelectedIndex = index == -1 ? 0 : index;

                scopeSlider.Activated += (o, e) =>
                {
                    ComponentPurchased(weapon.WeaponHash, weapon.Scopes.ElementAt(scopeSlider.SelectedIndex), weapon.Scopes.Values.ToList(), LoadoutSaving.SetScope);
                    Main.LittleJacob.ProcessVoice(true);
                };

                scopeSlider.ItemChanged += (o, e) =>
                {
                    ComponentSelected?.Invoke(this, new ComponentPreviewEventArgs(weapon.WeaponHash, 
                        weapon.Scopes.ElementAt(scopeSlider.SelectedIndex).Value, ComponentIndex.Scope));

                    _lastItem = scopeSlider;
                    _installedIndex = weapon.Scopes.Values.ToList().IndexOf(LoadoutSaving.GetStoreReference(weapon.WeaponHash).Scope);
                    _itemChanged = true;
                };

                menu.Add(scopeSlider);
            }

            if (weapon.HasGrip)
            {
                var gripSlider = new NativeListItem<string>("Grips");

                for (int i = 0; i < weapon.Grips.Count; i++)
                {
                    gripSlider.Add(weapon.Grips.ElementAt(i).Key);
                }

                var index = weapon.Grips.Values.ToList().IndexOf(storeRef.Grip);
                gripSlider.SelectedIndex = index == -1 ? 0 : index;

                gripSlider.Activated += (o, e) => 
                { 
                    ComponentPurchased(weapon.WeaponHash, weapon.Grips.ElementAt(gripSlider.SelectedIndex),
                        weapon.Grips.Values.ToList(), LoadoutSaving.SetGrip);
                    Main.LittleJacob.ProcessVoice(true); 
                };
                
                gripSlider.ItemChanged += (o, e) =>
                {
                    ComponentSelected?.Invoke(this, new ComponentPreviewEventArgs(weapon.WeaponHash, 
                        weapon.Grips.ElementAt(gripSlider.SelectedIndex).Value, ComponentIndex.Grip));

                    _lastItem = gripSlider;
                    _installedIndex = weapon.Grips.Values.ToList().IndexOf(LoadoutSaving.GetStoreReference(weapon.WeaponHash).Grip);
                    _itemChanged = true;
                };

                menu.Add(gripSlider);
            }

            if (weapon.HasBarrel)
            {
                var barrelSlider = new NativeListItem<string>("Barrels");

                for (int i = 0; i < weapon.Barrels.Count; i++)
                {
                    barrelSlider.Add(weapon.Barrels.ElementAt(i).Key);
                }

                var index = weapon.Barrels.Values.ToList().IndexOf(storeRef.Barrel);
                barrelSlider.SelectedIndex = index == -1 ? 0 : index;

                barrelSlider.Activated += (o, e) => 
                { 
                    ComponentPurchased(weapon.WeaponHash, weapon.Barrels.ElementAt(barrelSlider.SelectedIndex), weapon.Barrels.Values.ToList(), LoadoutSaving.SetBarrel);
                    Main.LittleJacob.ProcessVoice(true); 
                };

                barrelSlider.ItemChanged += (o, e) =>
                {
                    ComponentSelected?.Invoke(this, new ComponentPreviewEventArgs(weapon.WeaponHash, weapon.Barrels.ElementAt(barrelSlider.SelectedIndex).Value, ComponentIndex.Barrel));
                    
                    _lastItem = barrelSlider;
                    _installedIndex = weapon.Barrels.Values.ToList().IndexOf(LoadoutSaving.GetStoreReference(weapon.WeaponHash).Barrel);
                    _itemChanged = true;
                };

                menu.Add(barrelSlider);
            }

            if (weapon.HasCamo)
            {
                var camoSlider = new NativeListItem<string>("Camo");
                var camoColorSlider = new NativeListItem<string>("Camo color");

                for (int i = 0; i < weapon.Camos.Count; i++)
                {
                    camoSlider.Add(weapon.Camos.ElementAt(i).Key);
                }

                for (int i = 0; i < TintsAndCamos.CamoColor.Count; i++)
                {
                    camoColorSlider.Add(TintsAndCamos.CamoColor[i]);
                }

                camoColorSlider.Description = "Price: $10000";
                camoSlider.Description = "Price: $60000";

                var index = weapon.Camos.Values.ToList().IndexOf(storeRef.Camo);
                camoSlider.SelectedIndex = index == -1 ? 0 : index;
                index = storeRef.GetCamoColor();
                camoColorSlider.SelectedIndex = index < 0 ? 0 : index;

                camoSlider.Activated += (o, e) => { ComponentPurchased(weapon.WeaponHash, weapon.Camos.ElementAt(camoSlider.SelectedIndex), weapon.Camos.Values.ToList(), LoadoutSaving.SetCamo, true); Main.LittleJacob.ProcessVoice(true); };

                camoSlider.ItemChanged += (o, e) =>
                {
                    ComponentSelected?.Invoke(this, new ComponentPreviewEventArgs(weapon.WeaponHash, weapon.Camos.ElementAt(camoSlider.SelectedIndex).Value, ComponentIndex.Camo));
                    
                    _lastItem = camoSlider;
                    _installedIndex = weapon.Camos.Values.ToList().IndexOf(LoadoutSaving.GetStoreReference(weapon.WeaponHash).Camo);
                    _itemChanged = true;
                };

                camoColorSlider.ItemChanged += (o, e) =>
                {
                    var eventArgs = new CamoColorEventArgs
                    {
                        Camo = storeRef.Camo,
                        ColorIndex = e.Index
                    };

                    CamoColorChanged?.Invoke(this, eventArgs);

                    _lastItem = camoColorSlider;
                    _installedIndex = LoadoutSaving.GetStoreReference(weapon.WeaponHash).GetCamoColor();
                    _itemChanged = true;
                };

                camoColorSlider.Activated += (o, e) => CamoColorPurchased(weapon.WeaponHash, camoColorSlider.SelectedIndex);
                
                menu.Add(camoSlider);
                menu.Add(camoColorSlider);
            }*/
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
            _ownedItem.Description = "Price $5000";
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
            _ownedItem.Description = $"Price{weaponComponent.Key.Split('-')[1]}";
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
        }
    }
}
