using System;
using System.Collections.Generic;
using GTA;
using GTA.Native;
using LittleJacobMod.Saving.Utils;
using LittleJacobMod.Utils.Weapons;
using System.Linq;
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

        static Weapon GetWeapon(uint hash)
        {
            foreach (Weapon weapon in Weapons)
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
            if (Main.PPID == 0)
                return;

            bool changes = false;

            try
            {
                for (int i = 0; i < Weapons.Count; i++)
                {
                    Weapon weapon = Weapons[i];
                    bool hasWeapon = Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON, Main.PPID, weapon.WeaponHash, false);
                    bool isInStore = LoadoutSaving.IsWeaponInStore(weapon.WeaponHash);

                    if (hasWeapon && !isInStore)
                    {
                        changes = true;
                        StoredWeapon storedWeapon = new StoredWeapon(weapon.WeaponHash);
                        storedWeapon.Tint = storedWeapon.GetTintIndex();
                        storedWeapon.Ammo = Function.Call<int>(Hash.GET_AMMO_IN_PED_WEAPON, Main.PPID, weapon.WeaponHash);

                        if (weapon.HasMuzzleOrSupp)
                        {
                            for (int n = 0; n < weapon.MuzzlesAndSupps.Count; n++)
                            {
                                uint muzzleOrSupp = weapon.MuzzlesAndSupps.ElementAt(n).Value;

                                if (muzzleOrSupp == (uint)WeaponComponentHash.Invalid)
                                {
                                    continue;
                                }
                                else if (storedWeapon.HasComponent(muzzleOrSupp))
                                {
                                    storedWeapon.Muzzle = muzzleOrSupp;
                                }
                            }
                        }

                        if (weapon.HasClip)
                        {
                            for (int n = 0; n < weapon.Clips.Count; n++)
                            {
                                uint clip = weapon.Clips.ElementAt(n).Value;

                                if (clip == (uint)WeaponComponentHash.Invalid)
                                {
                                    continue;
                                }
                                else if (storedWeapon.HasComponent(clip))
                                {
                                    storedWeapon.Clip = clip;
                                }
                            }
                        }

                        if (weapon.HasScope)
                        {
                            for (int n = 0; n < weapon.Scopes.Count; n++)
                            {
                                uint scope = weapon.Scopes.ElementAt(n).Value;

                                if (scope == (uint)WeaponComponentHash.Invalid)
                                {
                                    continue;
                                }
                                else if (storedWeapon.HasComponent(scope))
                                {
                                    storedWeapon.Scope = scope;
                                }
                            }
                        }

                        if (weapon.HasGrip)
                        {
                            for (int n = 0; n < weapon.Grips.Count; n++)
                            {
                                uint grip = weapon.Grips.ElementAt(n).Value;

                                if (grip == (uint)WeaponComponentHash.Invalid)
                                {
                                    continue;
                                }
                                else if (storedWeapon.HasComponent(grip))
                                {
                                    storedWeapon.Grip = grip;
                                }
                            }
                        }

                        if (weapon.HasBarrel)
                        {
                            for (int n = 0; n < weapon.Barrels.Count; n++)
                            {
                                uint barrel = weapon.Barrels.ElementAt(n).Value;

                                if (barrel == (uint)WeaponComponentHash.Invalid)
                                {
                                    continue;
                                }
                                else if (storedWeapon.HasComponent(barrel))
                                {
                                    storedWeapon.Barrel = barrel;
                                }
                            }
                        }

                        if (weapon.HasCamo)
                        {
                            for (int n = 0; n < weapon.Camos.Count; n++)
                            {
                                uint camo = weapon.Camos.ElementAt(n).Value;

                                if (camo == (uint)WeaponComponentHash.Invalid)
                                {
                                    continue;
                                }
                                else if (storedWeapon.HasComponent(camo))
                                {
                                    storedWeapon.Camo = camo;
                                    storedWeapon.CamoColor = storedWeapon.GetCamoColor();
                                }
                            }
                        }

                        if (weapon.HasFlaslight)
                        {
                            for (int n = 0; n < weapon.FlashLight.Count; n++)
                            {
                                uint flash = weapon.FlashLight.ElementAt(n).Value;

                                if (flash == (uint)WeaponComponentHash.Invalid)
                                {
                                    continue;
                                }
                                else if (storedWeapon.HasComponent(flash))
                                {
                                    storedWeapon.Flashlight = flash;
                                }
                            }
                        }

                        weapons.Add(storedWeapon);
                    }
                }

                for (int i = weapons.Count - 1; i > -1; i--)
                {
                    StoredWeapon weapon = weapons[i];
                    bool hasWeapon = Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON, Main.PPID, weapon.WeaponHash, false);

                    if (!hasWeapon)
                    {
                        changes = true;
                        GTA.UI.Screen.ShowSubtitle($"CHANGE 2 WEAPON {weapon.WeaponHash}");
                        weapons.RemoveAt(i);
                        continue;
                    }

                    Weapon weaponCatalogOption = GetWeapon(weapons[i].WeaponHash);
                    int ammo = Function.Call<int>(Hash.GET_AMMO_IN_PED_WEAPON, Main.PPID, weapon.WeaponHash);

                    if (ammo > weapon.Ammo && MapperMain.CurrentPed == Function.Call<uint>(Hash.GET_ENTITY_MODEL, Main.PPID))
                    {
                        weapon.Ammo = ammo;
                    }

                    int tintIndex = weapon.GetTintIndex();

                    if (tintIndex != weapon.Tint)
                    {
                        weapon.Tint = tintIndex;
                    }

                    if (weaponCatalogOption.HasMuzzleOrSupp)
                    {
                        for (int n = 0; n < weaponCatalogOption.MuzzlesAndSupps.Count; n++)
                        {
                            uint attachment = weaponCatalogOption.MuzzlesAndSupps.ElementAt(n).Value;

                            if (attachment == (uint)WeaponComponentHash.Invalid || attachment == weapon.Muzzle)
                            {
                                continue;
                            }
                            else if (weapon.HasComponent(attachment))
                            {
                                weapon.Muzzle = attachment;
                            }
                        }
                    }

                    if (weaponCatalogOption.HasClip)
                    {
                        for (int n = 0; n < weaponCatalogOption.Clips.Count; n++)
                        {
                            uint attachment = weaponCatalogOption.Clips.ElementAt(n).Value;

                            if (attachment == (uint)WeaponComponentHash.Invalid || attachment == weapon.Clip)
                            {
                                continue;
                            }
                            else if (weapon.HasComponent(attachment))
                            {
                                weapon.Clip = attachment;
                            }
                        }
                    }

                    if (weaponCatalogOption.HasScope)
                    {
                        for (int n = 0; n < weaponCatalogOption.Scopes.Count; n++)
                        {
                            uint attachment = weaponCatalogOption.Scopes.ElementAt(n).Value;

                            if (attachment == (uint)WeaponComponentHash.Invalid || attachment == weapon.Scope)
                            {
                                continue;
                            }
                            else if (weapon.HasComponent(attachment))
                            {
                                weapon.Scope = attachment;
                            }
                        }
                    }

                    if (weaponCatalogOption.HasGrip)
                    {
                        for (int n = 0; n < weaponCatalogOption.Grips.Count; n++)
                        {
                            uint attachment = weaponCatalogOption.Grips.ElementAt(n).Value;

                            if (attachment == (uint)WeaponComponentHash.Invalid || attachment == weapon.Grip)
                            {
                                continue;
                            }
                            else if (weapon.HasComponent(attachment))
                            {
                                weapon.Grip = attachment;
                            }
                        }
                    }

                    if (weaponCatalogOption.HasBarrel)
                    {
                        for (int n = 0; n < weaponCatalogOption.Barrels.Count; n++)
                        {
                            uint attachment = weaponCatalogOption.Barrels.ElementAt(n).Value;

                            if (attachment == (uint)WeaponComponentHash.Invalid || attachment == weapon.Barrel)
                            {
                                continue;
                            }
                            else if (weapon.HasComponent(attachment))
                            {
                                weapon.Barrel = attachment;
                            }
                        }
                    }

                    if (weaponCatalogOption.HasCamo)
                    {
                        for (int n = 0; n < weaponCatalogOption.Camos.Count; n++)
                        {
                            uint attachment = weaponCatalogOption.Camos.ElementAt(n).Value;

                            if (attachment == (uint)WeaponComponentHash.Invalid || attachment == weapon.Camo)
                            {
                                continue;
                            }
                            else if (weapon.HasComponent(attachment))
                            {
                                weapon.Camo = attachment;
                                weapon.CamoColor = weapon.GetCamoColor();
                            }
                        }
                    }

                    if (weaponCatalogOption.HasFlaslight)
                    {
                        for (int n = 0; n < weaponCatalogOption.FlashLight.Count; n++)
                        {
                            uint attachment = weaponCatalogOption.Camos.ElementAt(n).Value;

                            if (attachment == (uint)WeaponComponentHash.Invalid || attachment == weapon.Flashlight)
                            {
                                continue;
                            }
                            else if (weapon.HasComponent(attachment))
                            {
                                weapon.Flashlight = attachment;
                            }
                        }
                    }
                }
            } catch (Exception) { }

            if (changes)
            {
                LoadoutSaving.PerformSave(MapperMain.CurrentPed);
            }
        }
    }
}
