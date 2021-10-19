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
    internal class Menu
    {
        ObjectPool pool;
        NativeMenu mainMenu;
        NativeMenu melee;
        NativeMenu pistols;
        NativeMenu rifles;
        NativeMenu mg;
        NativeMenu snipers;
        NativeMenu heavy;
        NativeMenu shotguns;
        NativeMenu explosives;
        public ObjectPool Pool => pool;

        public Menu()
        {
            pool = new ObjectPool();
            mainMenu = new NativeMenu("Little Jacob", "Weapon store");
            melee = new NativeMenu("Melee", "Melee weapons");
            pistols = new NativeMenu("Pistols", "Pistols");
            rifles = new NativeMenu("Rifles", "Rifles");
            mg = new NativeMenu("Machine guns", "Machine guns");
            snipers = new NativeMenu("Sniper rifles", "Sniper rifles");
            shotguns = new NativeMenu("Shotguns", "Shotguns");
            explosives = new NativeMenu("Explosives", "Explosives");
            heavy = new NativeMenu("Heavy Weapons", "Heavy Weapons");

            pool.Add(mainMenu);
            pool.Add(melee);
            pool.Add(pistols);
            pool.Add(rifles);
            pool.Add(mg);
            pool.Add(snipers);
            pool.Add(explosives);
            pool.Add(shotguns);
            pool.Add(heavy);

            mainMenu.AddSubMenu(melee);
            mainMenu.AddSubMenu(pistols);
            mainMenu.AddSubMenu(rifles);
            mainMenu.AddSubMenu(shotguns);
            mainMenu.AddSubMenu(mg);
            mainMenu.AddSubMenu(snipers);
            mainMenu.AddSubMenu(heavy);
            mainMenu.AddSubMenu(explosives);

            for (var i = 0; i < WeaponHashes.meeleHashes.Count; i++)
            {
                var index = i;
                var item = new NativeItem(WeaponHashes.meeleHashes[i].ToString());
                melee.Add(item);
                item.Activated += (o, e) => MeleeWeaponSelected(index);
            }

            for (var i = 0; i < WeaponsList.Pistols.Count; i++)
            {
                var index = i;
                var displayName = WeaponsList.Pistols[i].Name;
                var pistolMenu = new NativeMenu(displayName, displayName);

                pistolMenu.Shown += (o, e) => WeaponSelected(WeaponsList.Pistols[index], WeaponsList.Pistols[index].Price, pistolMenu);
                pistolMenu.Closed += (o, e) => pistolMenu.Clear();
                
                pool.Add(pistolMenu);
                pistols.AddSubMenu(pistolMenu);
            }

            for (var i = 0; i < WeaponsList.SMGs.Count; i++)
            {
                var index = i;
                var displayName = WeaponsList.SMGs[i].Name;
                var smgMenu = new NativeMenu(displayName, displayName);

                smgMenu.Shown += (o, e) => WeaponSelected(WeaponsList.SMGs[index], WeaponsList.SMGs[index].Price, smgMenu);
                smgMenu.Closed += (o, e) => smgMenu.Clear();

                pool.Add(smgMenu);
                mg.AddSubMenu(smgMenu);
            }

            for (var i = 0; i < WeaponsList.Shotguns.Count; i++)
            {
                var index = i;
                var displayName = WeaponsList.Shotguns[i].Name;
                var shotgunMenu = new NativeMenu(displayName, displayName);

                shotgunMenu.Shown += (o, e) => WeaponSelected(WeaponsList.Shotguns[index], WeaponsList.Shotguns[index].Price, shotgunMenu);
                shotgunMenu.Closed += (o, e) => shotgunMenu.Clear();

                pool.Add(shotgunMenu);
                shotguns.AddSubMenu(shotgunMenu);
            }

            for (var i = 0; i < WeaponsList.Rifles.Count; i++)
            {
                var index = i;
                var displayName = WeaponsList.Rifles[i].Name;
                var rifleMenu = new NativeMenu(displayName, displayName);

                rifleMenu.Shown += (o, e) => WeaponSelected(WeaponsList.Rifles[index], WeaponsList.Rifles[index].Price, rifleMenu);
                rifleMenu.Closed += (o, e) => rifleMenu.Clear();

                pool.Add(rifleMenu);
                rifles.AddSubMenu(rifleMenu);
            }

            for (var i = 0; i < WeaponsList.Snipers.Count; i++)
            {
                var index = i;
                var displayName = WeaponsList.Snipers[i].Name;
                var sniperMenu = new NativeMenu(displayName, displayName);

                sniperMenu.Shown += (o, e) => WeaponSelected(WeaponsList.Snipers[index], WeaponsList.Snipers[index].Price, sniperMenu);
                sniperMenu.Closed += (o, e) => sniperMenu.Clear();

                pool.Add(sniperMenu);
                snipers.AddSubMenu(sniperMenu);
            }

            for (var i = 0; i < WeaponsList.Heavy.Count; i++)
            {
                var index = i;
                var displayName = WeaponsList.Heavy[i].Name;
                var heavyMenu = new NativeMenu(displayName, displayName);

                heavyMenu.Shown += (o, e) => WeaponSelected(WeaponsList.Heavy[index], WeaponsList.Heavy[index].Price, heavyMenu);
                heavyMenu.Closed += (o, e) => heavyMenu.Clear();

                pool.Add(heavyMenu);
                heavy.AddSubMenu(heavyMenu);
            }
        }

        public void ShowMainMenu()
        {
            mainMenu.Visible = true;
        }

        void WeaponSelected(Utils.Weapons.Weapon weapon, int price, NativeMenu menu)
        {
            var hasWeapon = Game.Player.Character.Weapons.HasWeapon(weapon.WeaponHash);
            if (!hasWeapon)
            {
                if (Game.Player.Money < price)
                {
                    GTA.UI.Notification.Show("You don't have enough money!");
                    mainMenu.Back();
                    return;
                }
                GTA.UI.Notification.Show($"{weapon.Name} purchased!");
                Game.Player.Money -= price;
                Game.Player.Character.Weapons.Give(weapon.WeaponHash, 0, false, false);
            }
            Script.Wait(1);
            Game.Player.Character.Weapons.Select(weapon.WeaponHash, true);
            Script.Wait(1);
            var currentWeapon = Game.Player.Character.Weapons.Current;

            if (!LadoutSaving.IsPedMainPlayer(Game.Player.Character) || weapon.SaveFileWeapon)
            {
                LadoutSaving.AddWeapon(currentWeapon);
            }

            var ammoOptionItem = new NativeSliderItem("Ammo", currentWeapon.MaxAmmo, 1);
            ammoOptionItem.Activated += (o, e) => AmmoPurchased(currentWeapon, ammoOptionItem.Value);
            menu.Add(ammoOptionItem);

            if (weapon.WeaponHash != WeaponHash.PericoPistol && weapon.WeaponHash != WeaponHash.DoubleActionRevolver && weapon.WeaponHash != WeaponHash.NavyRevolver)
            {
                var tintSlider = new NativeListItem<string>("Tints");
                for (int i = 0; i < Function.Call<int>(Hash.GET_WEAPON_TINT_COUNT, currentWeapon.Hash); i++)
                {
                    tintSlider.Add($"{i}");
                }
                tintSlider.Activated += (o, e) => TintPurchased(currentWeapon, tintSlider.SelectedIndex, weapon.SaveFileWeapon);
                menu.Add(tintSlider);
            }

            if (weapon.HasClip)
            {
                var clipSlider = new NativeListItem<string>("Clips");
                for (int i = 0; i < weapon.Clips.Count; i++)
                {
                    clipSlider.Add(weapon.Clips.ElementAt(i).Key);
                }
                clipSlider.Activated += (o, e) => ClipPurchased(currentWeapon, weapon.Clips.ElementAt(clipSlider.SelectedIndex), weapon.SaveFileWeapon);
                menu.Add(clipSlider);
            }

            if (weapon.HasMuzzleOrSupp)
            {
                var muzzleSlider = new NativeListItem<string>("Muzzle and suppressors");
                for (int i = 0; i < weapon.MuzzlesAndSupps.Count; i++)
                {
                    muzzleSlider.Add(weapon.MuzzlesAndSupps.ElementAt(i).Key);
                }
                muzzleSlider.Activated += (o, e) => MuzzlePurchased(currentWeapon, weapon.MuzzlesAndSupps.ElementAt(muzzleSlider.SelectedIndex), weapon.MuzzlesAndSupps.Values.ToList(), weapon.SaveFileWeapon);
                menu.Add(muzzleSlider);
            }

            if (weapon.HasFlaslight)
            {
                var flashSlider = new NativeListItem<string>("Flashlight");
                for (int i = 0; i < weapon.FlashLight.Count; i++)
                {
                    flashSlider.Add(weapon.FlashLight.ElementAt(i).Key);
                }
                flashSlider.Activated += (o, e) => FlashPurchased(currentWeapon, weapon.FlashLight.ElementAt(flashSlider.SelectedIndex), weapon.FlashLight.Values.ToList(), weapon.SaveFileWeapon);
                menu.Add(flashSlider);
            }

            if (weapon.HasScope)
            {
                var scopeSlider = new NativeListItem<string>("Scopes");
                for (int i = 0; i < weapon.Scopes.Count; i++)
                {
                    scopeSlider.Add(weapon.Scopes.ElementAt(i).Key);
                }
                scopeSlider.Activated += (o, e) => ScopePurchased(currentWeapon, weapon.Scopes.ElementAt(scopeSlider.SelectedIndex), weapon.Scopes.Values.ToList(), weapon.SaveFileWeapon);
                menu.Add(scopeSlider);
            }

            if (weapon.HasGrip)
            {
                var gripSlider = new NativeListItem<string>("Grips");
                for (int i = 0; i < weapon.Grips.Count; i++)
                {
                    gripSlider.Add(weapon.Grips.ElementAt(i).Key);
                }
                gripSlider.Activated += (o, e) => GripPurchased(currentWeapon, weapon.Grips.ElementAt(gripSlider.SelectedIndex), weapon.Grips.Values.ToList(), weapon.SaveFileWeapon);
                menu.Add(gripSlider);
            }

            if (weapon.HasBarrel)
            {
                var barrelSlider = new NativeListItem<string>("Barrels");
                for (int i = 0; i < weapon.Barrels.Count; i++)
                {
                    barrelSlider.Add(weapon.Barrels.ElementAt(i).Key);
                }
                barrelSlider.Activated += (o, e) => BarrelPurchased(currentWeapon, weapon.Barrels.ElementAt(barrelSlider.SelectedIndex), weapon.Barrels.Values.ToList());
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
                for (int i = 0; i < 31; i++)
                {
                    camoColorSlider.Add($"{i}");
                }
                camoSlider.Activated += (o, e) => CamoPurchased(currentWeapon, weapon.Camos.ElementAt(camoSlider.SelectedIndex), weapon.Camos.Values.ToList());
                camoColorSlider.Activated += (o, e) => CamoColorPurchased(currentWeapon, camoColorSlider.SelectedIndex);
                menu.Add(camoSlider);
                menu.Add(camoColorSlider);
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
            LadoutSaving.SetAmmo(weapon.Hash, weapon.Ammo);
        }

        void TintPurchased(GTA.Weapon weapon, int index, bool saveFileWeapon)
        {
            var price = 5000;
            if (Game.Player.Money < price)
            {
                GTA.UI.Notification.Show("Couldn't purchase this tint. Not enough money!");
                return;
            }
            Game.Player.Money -= price;
            Function.Call(Hash.SET_PED_WEAPON_TINT_INDEX, Game.Player.Character.Handle, weapon.Hash, index);
            GTA.UI.Notification.Show($"Tint purchased!");
            if (!LadoutSaving.IsPedMainPlayer(Game.Player.Character) || saveFileWeapon)
            {
                LadoutSaving.SetTint(weapon.Hash, index);
            }
        }

        void ClipPurchased(GTA.Weapon weapon, KeyValuePair<string, WeaponComponentHash> weaponComponent, bool saveFileWeapon)
        {
            var price = int.Parse(weaponComponent.Key.Split('$')[1]);
            if (Game.Player.Money < price)
            {
                GTA.UI.Notification.Show("Couldn't purchase the clip. Not enough money!");
                return;
            }
            Game.Player.Money -= price;
            GTA.UI.Notification.Show($"{weaponComponent.Key.Split('-')[0]} purchased!");
            Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, Game.Player.Character.Handle, weapon.Hash, weaponComponent.Value);
            if (!LadoutSaving.IsPedMainPlayer(Game.Player.Character) || saveFileWeapon)
            {
                LadoutSaving.SetClip(weapon.Hash, weaponComponent.Value);
            }
        }

        void MuzzlePurchased(GTA.Weapon weapon, KeyValuePair<string, WeaponComponentHash> weaponComponent, List<WeaponComponentHash> components, bool saveFileWeapon)
        {
            var price = int.Parse(weaponComponent.Key.Split('$')[1]);
            GTA.UI.Screen.ShowSubtitle($"{price} {weaponComponent.Key.Split('$')[1]}");
            if (Game.Player.Money < price)
            {
                GTA.UI.Notification.Show("Couldn't purchase the muzzle attachment. Not enough money!");
                return;
            }
            Game.Player.Money -= price;
            if (weaponComponent.Key.Contains("None"))
            {
                foreach (WeaponComponentHash component in components)
                {
                    if (weaponComponent.Value != component && Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON_COMPONENT, Game.Player.Character.Handle, weapon.Hash, component))
                    {
                        Function.Call(Hash.REMOVE_WEAPON_COMPONENT_FROM_PED, Game.Player.Character.Handle, weapon.Hash, component);
                    }
                }
                GTA.UI.Notification.Show("Muzzle attachments removed!");
                if (!LadoutSaving.IsPedMainPlayer(Game.Player.Character) || saveFileWeapon)
                {
                    LadoutSaving.SetMuzzle(weapon.Hash, WeaponComponentHash.Invalid);
                }
            } else
            {
                Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, Game.Player.Character.Handle, weapon.Hash, weaponComponent.Value);
                GTA.UI.Notification.Show($"{weaponComponent.Key.Split('-')[0]} purchased!");
                if (!LadoutSaving.IsPedMainPlayer(Game.Player.Character) || saveFileWeapon)
                {
                    LadoutSaving.SetMuzzle(weapon.Hash, weaponComponent.Value);
                }
            }
        }

        void FlashPurchased(GTA.Weapon weapon, KeyValuePair<string, WeaponComponentHash> weaponComponent, List<WeaponComponentHash> components, bool saveFileWeapon)
        {
            var price = int.Parse(weaponComponent.Key.Split('$')[1]);
            if (Game.Player.Money < price)
            {
                GTA.UI.Notification.Show("Couldn't purchase flashlight. Not enough money!");
                return;
            }
            Game.Player.Money -= price;
            if (weaponComponent.Key.Contains("None"))
            {
                foreach (WeaponComponentHash component in components)
                {
                    if (weaponComponent.Value != component && Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON_COMPONENT, Game.Player.Character.Handle, weapon.Hash, component))
                    {
                        Function.Call(Hash.REMOVE_WEAPON_COMPONENT_FROM_PED, Game.Player.Character.Handle, weapon.Hash, component);
                    }
                }
                GTA.UI.Notification.Show("Flashlight removed!");
                if (!LadoutSaving.IsPedMainPlayer(Game.Player.Character) || saveFileWeapon)
                {
                    LadoutSaving.SetFlashlight(weapon.Hash, WeaponComponentHash.Invalid);
                }
            }
            else
            {
                Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, Game.Player.Character.Handle, weapon.Hash, weaponComponent.Value);
                GTA.UI.Notification.Show($"{weaponComponent.Key.Split('-')[0]} purchased!");
                if (!LadoutSaving.IsPedMainPlayer(Game.Player.Character) || saveFileWeapon)
                {
                    LadoutSaving.SetFlashlight(weapon.Hash, weaponComponent.Value);
                }
            }
        }

        void ScopePurchased(GTA.Weapon weapon, KeyValuePair<string, WeaponComponentHash> weaponComponent, List<WeaponComponentHash> components, bool saveFileWeapon)
        {
            var price = int.Parse(weaponComponent.Key.Split('$')[1]);
            if (Game.Player.Money < price)
            {
                GTA.UI.Notification.Show("Couldn't purchase scope. Not enough money!");
                return;
            }
            Game.Player.Money -= price;
            if (weaponComponent.Key.Contains("None"))
            {
                foreach (WeaponComponentHash component in components)
                {
                    if (weaponComponent.Value != component && Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON_COMPONENT, Game.Player.Character.Handle, weapon.Hash, component))
                    {
                        Function.Call(Hash.REMOVE_WEAPON_COMPONENT_FROM_PED, Game.Player.Character.Handle, weapon.Hash, component);
                    }
                }
                GTA.UI.Notification.Show("Scope removed!");
                if (!LadoutSaving.IsPedMainPlayer(Game.Player.Character) || saveFileWeapon)
                {
                    LadoutSaving.SetScope(weapon.Hash, WeaponComponentHash.Invalid);
                }
            }
            else
            {
                Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, Game.Player.Character.Handle, weapon.Hash, weaponComponent.Value);
                GTA.UI.Notification.Show($"{weaponComponent.Key.Split('-')[0]} purchased!");
                if (!LadoutSaving.IsPedMainPlayer(Game.Player.Character) || saveFileWeapon)
                {
                    LadoutSaving.SetScope(weapon.Hash, weaponComponent.Value);
                }
            }
        }

        void GripPurchased(GTA.Weapon weapon, KeyValuePair<string, WeaponComponentHash> weaponComponent, List<WeaponComponentHash> components, bool saveFileWeapon)
        {
            var price = int.Parse(weaponComponent.Key.Split('$')[1]);
            if (Game.Player.Money < price)
            {
                GTA.UI.Notification.Show("Couldn't purchase grip. Not enough money!");
                return;
            }
            Game.Player.Money -= price;
            if (weaponComponent.Key.Contains("None"))
            {
                foreach (WeaponComponentHash component in components)
                {
                    if (weaponComponent.Value != component && Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON_COMPONENT, Game.Player.Character.Handle, weapon.Hash, component))
                    {
                        Function.Call(Hash.REMOVE_WEAPON_COMPONENT_FROM_PED, Game.Player.Character.Handle, weapon.Hash, component);
                    }
                }
                GTA.UI.Notification.Show("Grip removed!");
                if (!LadoutSaving.IsPedMainPlayer(Game.Player.Character) || saveFileWeapon)
                {
                    LadoutSaving.SetGrip(weapon.Hash, WeaponComponentHash.Invalid);
                }
            }
            else
            {
                Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, Game.Player.Character.Handle, weapon.Hash, weaponComponent.Value);
                GTA.UI.Notification.Show($"{weaponComponent.Key.Split('-')[0]} purchased!");
                if (!LadoutSaving.IsPedMainPlayer(Game.Player.Character) || saveFileWeapon)
                {
                    LadoutSaving.SetGrip(weapon.Hash, weaponComponent.Value);
                }
            }
        }

        void BarrelPurchased(GTA.Weapon weapon, KeyValuePair<string, WeaponComponentHash> weaponComponent, List<WeaponComponentHash> components)
        {
            var price = int.Parse(weaponComponent.Key.Split('$')[1]);
            if (Game.Player.Money < price)
            {
                GTA.UI.Notification.Show("Couldn't purchase barrel. Not enough money!");
                return;
            }
            Game.Player.Money -= price;
            if (weaponComponent.Key.Contains("None"))
            {
                foreach (WeaponComponentHash component in components)
                {
                    if (weaponComponent.Value != component && Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON_COMPONENT, Game.Player.Character.Handle, weapon.Hash, component))
                    {
                        Function.Call(Hash.REMOVE_WEAPON_COMPONENT_FROM_PED, Game.Player.Character.Handle, weapon.Hash, component);
                    }
                }
                GTA.UI.Notification.Show("Custom Barrel removed!");
                LadoutSaving.SetBarrel(weapon.Hash, WeaponComponentHash.Invalid);
            }
            else
            {
                Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, Game.Player.Character.Handle, weapon.Hash, weaponComponent.Value);
                GTA.UI.Notification.Show($"{weaponComponent.Key.Split('-')[0]} purchased!");
                LadoutSaving.SetBarrel(weapon.Hash, weaponComponent.Value);
            }
        }

        void CamoPurchased(GTA.Weapon weapon, KeyValuePair<string, WeaponComponentHash> weaponComponent, List<WeaponComponentHash> components)
        {
            var price = 20000;
            if (Game.Player.Money < price)
            {
                GTA.UI.Notification.Show("Couldn't purchase camo. Not enough money!");
                return;
            }
            Game.Player.Money -= price;
            if (weaponComponent.Key.Contains("None"))
            {
                foreach (WeaponComponentHash component in components)
                {
                    if (weaponComponent.Value != component && Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON_COMPONENT, Game.Player.Character.Handle, weapon.Hash, component))
                    {
                        Function.Call(Hash.REMOVE_WEAPON_COMPONENT_FROM_PED, Game.Player.Character.Handle, weapon.Hash, component);
                    }
                }
                GTA.UI.Notification.Show("Camo removed!");
                LadoutSaving.SetCamo(weapon.Hash, WeaponComponentHash.Invalid);
            }
            else
            {
                Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, Game.Player.Character.Handle, weapon.Hash, weaponComponent.Value);
                GTA.UI.Notification.Show($"{weaponComponent.Key.Split('-')[0]} purchased!");
                LadoutSaving.SetCamo(weapon.Hash, weaponComponent.Value);
            }
        }

        void CamoColorPurchased(GTA.Weapon weapon, int index)
        {
            var price = 5000;
            if (Game.Player.Money < price)
            {
                GTA.UI.Notification.Show("Couldn't purchase camo color. Not enough money!");
                return;
            }
            Game.Player.Money -= price;
            if (!LadoutSaving.HasCamo(weapon.Hash))
            {
                GTA.UI.Notification.Show("Buy a camo first!");
                return;
            }
            else
            {
                var storedWeapon = LadoutSaving.GetStoreReference(weapon.Hash);
                Function.Call(Hash._SET_PED_WEAPON_LIVERY_COLOR, Game.Player.Character.Handle, weapon.Hash, storedWeapon.Camo, index);
                GTA.UI.Notification.Show("Camo color purchased!");
                LadoutSaving.SetCamoColor(weapon.Hash, index);
            }
        }

        void MeleeWeaponSelected(int index)
        {
            var hasWeapon = Game.Player.Character.Weapons.HasWeapon(WeaponHashes.meeleHashes[index]);
            if (hasWeapon)
            {
                GTA.UI.Notification.Show("You already have that weapon!");
                return;
            }

            if (Game.Player.Money < 1000)
            {
                GTA.UI.Notification.Show("You don't have enough money!");
                return;
            }

            Game.Player.Character.Weapons.Give(WeaponHashes.meeleHashes[index], 1, true, true);
            GTA.UI.Notification.Show($"{WeaponHashes.meeleHashes[index]} purchased!");
            Game.Player.Money -= 1000;
        }
    }
}
