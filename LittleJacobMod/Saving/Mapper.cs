using System;
using System.Collections.Generic;
using GTA;
using LittleJacobMod.Saving.Utils;
using LittleJacobMod.Utils.Weapons;
using Weapon = LittleJacobMod.Utils.Weapons.Weapon;

namespace LittleJacobMod.Saving
{
    internal class Mapper
    {
        static List<Weapon> Weapons { get; } = new List<Weapon>();

        public static void Initialize()
        {
            Weapons.AddRange(WeaponsList.Pistols);
            Weapons.AddRange(WeaponsList.SMGs);
            Weapons.AddRange(WeaponsList.Rifles);
            Weapons.AddRange(WeaponsList.Snipers);
            Weapons.AddRange(WeaponsList.Heavy);
            Weapons.AddRange(WeaponsList.Shotguns);
            Weapons.AddRange(WeaponsList.Explosives);
        }

        static Weapon GetWeapon(WeaponHash hash)
        {
            foreach (var weapon in Weapons)
            {
                if (weapon.WeaponHash == hash)
                {
                    return weapon;
                }
            }
            return null;
        }

        public static void Process(List<StoredWeapon> weapons)
        {
            bool changes = false;

            try
            {
                for (var i = 0; i < Weapons.Count; i++)
                {
                    if (Game.Player.Character.Weapons.HasWeapon(Weapons[i].WeaponHash) && !LoadoutSaving.IsWeaponInStore(Weapons[i].WeaponHash))
                    {
                        changes = true;
                        var storedWeapon = new StoredWeapon(Weapons[i].WeaponHash);
                        storedWeapon.Tint = storedWeapon.GetTintIndex();
                        storedWeapon.Ammo = Game.Player.Character.Weapons[Weapons[i].WeaponHash].Ammo;
                        if (Weapons[i].HasMuzzleOrSupp)
                        {
                            foreach (var muzzleOrSupp in Weapons[i].MuzzlesAndSupps)
                            {
                                if (muzzleOrSupp.Value == WeaponComponentHash.Invalid)
                                {
                                    continue;
                                }
                                else if (storedWeapon.HasComponent(muzzleOrSupp.Value))
                                {
                                    storedWeapon.Muzzle = muzzleOrSupp.Value;
                                }
                            }
                        }

                        if (Weapons[i].HasClip)
                        {
                            foreach (var clip in Weapons[i].Clips)
                            {
                                if (clip.Value == WeaponComponentHash.Invalid)
                                {
                                    continue;
                                }
                                else if (storedWeapon.HasComponent(clip.Value))
                                {
                                    storedWeapon.Clip = clip.Value;
                                }
                            }
                        }

                        if (Weapons[i].HasScope)
                        {
                            foreach (var scope in Weapons[i].Scopes)
                            {
                                if (scope.Value == WeaponComponentHash.Invalid)
                                {
                                    continue;
                                }
                                else if (storedWeapon.HasComponent(scope.Value))
                                {
                                    storedWeapon.Scope = scope.Value;
                                }
                            }
                        }

                        if (Weapons[i].HasGrip)
                        {
                            foreach (var grip in Weapons[i].Grips)
                            {
                                if (grip.Value == WeaponComponentHash.Invalid)
                                {
                                    continue;
                                }
                                else if (storedWeapon.HasComponent(grip.Value))
                                {
                                    storedWeapon.Grip = grip.Value;
                                }
                            }
                        }

                        if (Weapons[i].HasBarrel)
                        {
                            foreach (var barrel in Weapons[i].Barrels)
                            {
                                if (barrel.Value == WeaponComponentHash.Invalid)
                                {
                                    continue;
                                }
                                else if (storedWeapon.HasComponent(barrel.Value))
                                {
                                    storedWeapon.Barrel = barrel.Value;
                                }
                            }
                        }

                        if (Weapons[i].HasCamo)
                        {
                            foreach (var camo in Weapons[i].Camos)
                            {
                                if (camo.Value == WeaponComponentHash.Invalid)
                                {
                                    continue;
                                }
                                else if (storedWeapon.HasComponent(camo.Value))
                                {
                                    storedWeapon.Camo = camo.Value;
                                    storedWeapon.CamoColor = storedWeapon.GetCamoColor();
                                }
                            }
                        }

                        if (Weapons[i].HasFlaslight)
                        {
                            foreach (var flash in Weapons[i].FlashLight)
                            {
                                if (flash.Value == WeaponComponentHash.Invalid)
                                {
                                    continue;
                                }
                                else if (storedWeapon.HasComponent(flash.Value))
                                {
                                    storedWeapon.Flashlight = flash.Value;
                                }
                            }
                        }
                        weapons.Add(storedWeapon);
                    }
                }

                for (var i = 0; i < weapons.Count; i++)
                {
                    if (!Game.Player.Character.Weapons.HasWeapon(weapons[i].WeaponHash))
                    {
                        changes = true;
                        weapons.RemoveAt(i);
                    }
                }

                for (var i = 0; i < weapons.Count; i++)
                {
                    var weaponCatalogOption = GetWeapon(weapons[i].WeaponHash);
                    if (weaponCatalogOption == null)
                    {
                        continue;
                    }

                    var playerStoredWeapon = Game.Player.Character.Weapons[weapons[i].WeaponHash];
                    if (playerStoredWeapon.Ammo > weapons[i].Ammo && Main.CurrentPed == (PedHash)Game.Player.Character.Model.Hash)
                    {
                        weapons[i].Ammo = playerStoredWeapon.Ammo;
                    }

                    int tintIndex = weapons[i].GetTintIndex();
                    if (tintIndex != weapons[i].Tint)
                    {
                        weapons[i].Tint = tintIndex;
                    }

                    if (weaponCatalogOption.HasMuzzleOrSupp)
                    {
                        foreach (var muzzleOrSupp in weaponCatalogOption.MuzzlesAndSupps)
                        {
                            if (muzzleOrSupp.Value == WeaponComponentHash.Invalid || muzzleOrSupp.Value == weapons[i].Muzzle)
                            {
                                continue;
                            }
                            else if (weapons[i].HasComponent(muzzleOrSupp.Value))
                            {
                                weapons[i].Muzzle = muzzleOrSupp.Value;
                            }
                        }
                    }

                    if (weaponCatalogOption.HasClip)
                    {
                        foreach (var clip in weaponCatalogOption.Clips)
                        {
                            if (clip.Value == WeaponComponentHash.Invalid || clip.Value == weapons[i].Clip)
                            {
                                continue;
                            }
                            else if (weapons[i].HasComponent(clip.Value))
                            {
                                weapons[i].Clip = clip.Value;
                            }
                        }
                    }

                    if (weaponCatalogOption.HasScope)
                    {
                        foreach (var scope in weaponCatalogOption.Scopes)
                        {
                            if (scope.Value == WeaponComponentHash.Invalid || scope.Value == weapons[i].Scope)
                            {
                                continue;
                            }
                            else if (weapons[i].HasComponent(scope.Value))
                            {
                                weapons[i].Scope = scope.Value;
                            }
                        }
                    }

                    if (weaponCatalogOption.HasGrip)
                    {
                        foreach (var grip in weaponCatalogOption.Grips)
                        {
                            if (grip.Value == WeaponComponentHash.Invalid || grip.Value == weapons[i].Grip)
                            {
                                continue;
                            }
                            else if (weapons[i].HasComponent(grip.Value))
                            {
                                weapons[i].Grip = grip.Value;
                            }
                        }
                    }

                    if (weaponCatalogOption.HasBarrel)
                    {
                        foreach (var barrel in weaponCatalogOption.Barrels)
                        {
                            if (barrel.Value == WeaponComponentHash.Invalid || barrel.Value == weapons[i].Barrel)
                            {
                                continue;
                            }
                            else if (weapons[i].HasComponent(barrel.Value))
                            {
                                weapons[i].Barrel = barrel.Value;
                            }
                        }
                    }

                    if (weaponCatalogOption.HasCamo)
                    {
                        foreach (var camo in weaponCatalogOption.Camos)
                        {
                            if (camo.Value == WeaponComponentHash.Invalid || camo.Value == weapons[i].Camo)
                            {
                                continue;
                            }
                            else if (weapons[i].HasComponent(camo.Value))
                            {
                                weapons[i].Camo = camo.Value;
                                weapons[i].CamoColor = weapons[i].GetCamoColor();
                            }
                        }
                    }

                    if (weaponCatalogOption.HasFlaslight)
                    {
                        foreach (var flash in weaponCatalogOption.FlashLight)
                        {
                            if (flash.Value == WeaponComponentHash.Invalid || flash.Value == weapons[i].Flashlight)
                            {
                                continue;
                            }
                            else if (weapons[i].HasComponent(flash.Value))
                            {
                                weapons[i].Flashlight = flash.Value;
                            }
                        }
                    }
                }
            } catch (Exception) { }

            if (changes)
            {
                LoadoutSaving.PerformSave(Main.CurrentPed);
            }
        }
    }
}
