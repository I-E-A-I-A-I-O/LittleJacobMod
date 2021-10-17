using System;
using System.Collections.Generic;
using LemonUI;
using LemonUI.Menus;
using GTA;
using GTA.Native;
using LittleJacobMod.Utils;

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
        NativeMenu weapon;
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
                var item = new NativeItem(Weapon.GetDisplayNameFromHash(WeaponHashes.meeleHashes[i]));
                melee.Add(item);
                item.Activated += (o, e) => MeleeWeaponSelected(i);
            }

            for (var i = 0; i < WeaponHashes.pistolHashes.Count; i++)
            {
                var index = i;
                var pistolMenu = new NativeMenu(Weapon.GetDisplayNameFromHash(WeaponHashes.pistolHashes[i]), Weapon.GetDisplayNameFromHash(WeaponHashes.pistolHashes[i]));

                pistolMenu.Shown += (o, e) => WeaponSelected(WeaponHashes.pistolHashes[index], 3000, pistolMenu);
                pistolMenu.Closed += (o, e) => pistolMenu.Clear();
                
                pool.Add(pistolMenu);
                pistols.AddSubMenu(pistolMenu);
            }
        }

        public void ShowMainMenu()
        {
            mainMenu.Visible = true;
        }

        void WeaponSelected(WeaponHash weaponHash, int price, NativeMenu menu)
        {
            var hasWeapon = Game.Player.Character.Weapons.HasWeapon(weaponHash);
            if (!hasWeapon)
            {
                if (Game.Player.Money < price)
                {
                    GTA.UI.Notification.Show("You don't have enough money!");
                    mainMenu.Back();
                    return;
                }
                GTA.UI.Notification.Show($"{Weapon.GetDisplayNameFromHash(weaponHash)} purchased!");
                Game.Player.Money -= price;
                Game.Player.Character.Weapons.Give(weaponHash, 0, true, false);
            }
            Script.Wait(0);
            var weapon = Game.Player.Character.Weapons.Current;
            var ammoOptionItem = new NativeSliderItem("Ammo", weapon.MaxAmmo, 1);
            ammoOptionItem.Activated += (o, e) => AmmoPurchased(weapon, ammoOptionItem.Value);
            menu.Add(ammoOptionItem);
            if (weapon.Components.ScopeVariationsCount > 0)
            {
                var scopesSlider = new NativeListItem<WeaponComponentItem>("Scopes");
                for (int i = 0; i < weapon.Components.ScopeVariationsCount; i++)
                {
                    scopesSlider.Add(new WeaponComponentItem(i, weapon.Components.GetScopeComponent(i).DisplayName));
                }
                scopesSlider.Activated += (o, e) => ScopePurchased(weapon, scopesSlider.SelectedItem);
            }
        }

        void AmmoPurchased(Weapon weapon, int value)
        {
            var purchasableAmmo = weapon.MaxAmmo - weapon.Ammo;

            if (purchasableAmmo == 0)
            {
                GTA.UI.Notification.Show("Max ammo capacity reached");
                return;
            }

            var ammoToPurchase = value >= purchasableAmmo ? value - purchasableAmmo : value;
            var ammoPrice = value * 2;
            if (Game.Player.Money < ammoPrice)
            {
                GTA.UI.Notification.Show("Can't purchase ammo. Not enough money!");
                return;
            }
            Game.Player.Money -= ammoPrice;
            weapon.Ammo += ammoToPurchase;
        }

        void ScopePurchased(Weapon weapon, WeaponComponentItem scope)
        {
            var componentHash = weapon.Components.GetScopeComponent(scope.Index).ComponentHash;
            var price = 2500;
            if (!Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON_COMPONENT, Game.Player.Character.Handle, weapon.Hash, componentHash))
            {
                if (Game.Player.Money < price)
                {
                    GTA.UI.Notification.Show("Couldn't purchase the scope. Not enough money!");
                    return;
                }
                Game.Player.Character.Money -= price;
                GTA.UI.Notification.Show("Scope purchased");
            }
            weapon.Components.GetScopeComponent(scope.Index).Active = true;
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
            GTA.UI.Notification.Show($"{Weapon.GetDisplayNameFromHash(WeaponHashes.meeleHashes[index])} purchased!");
            Game.Player.Money -= 1000;
        }
    }
}
