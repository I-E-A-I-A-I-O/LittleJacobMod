using System;
using System.Collections.Generic;
using GTA;
using GTA.Native;
using LittleJacobMod.Saving.Utils;
using System.Linq;

namespace LittleJacobMod.Saving
{
    struct WeaponData
    {
        public List<bool> flags;
        public List<uint> muzzles;
        public List<uint> clips;
        public List<uint> barrels;
        public List<uint> grips;
        public List<uint> scopes;
        public List<uint> camos;
        public List<uint> flashlights;
        public List<uint> varmods;
        public uint weaponHash;
    }

    internal class Mapper
    {
        public static List<WeaponData> WeaponData = new List<WeaponData>();

        public static void Process(List<StoredWeapon> weapons)
        {
            if (Main.PPID == 0)
                return;

            bool changes = false;

            try
            {
                for (int i = 0; i < WeaponData.Count; i++)
                {
                    WeaponData weapon = WeaponData[i];
                    bool hasWeapon = Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON, Main.PPID, weapon.weaponHash, false);
                    bool isInStore = LoadoutSaving.IsWeaponInStore(weapon.weaponHash);

                    if (hasWeapon && !isInStore)
                    {
                        changes = true;
                        StoredWeapon storedWeapon = new StoredWeapon(weapon.weaponHash);
                        storedWeapon.Tint = storedWeapon.GetTintIndex();
                        storedWeapon.Ammo = Function.Call<int>(Hash.GET_AMMO_IN_PED_WEAPON, Main.PPID, weapon.weaponHash);

                        if (weapon.flags[0])
                        {
                            for (int n = 0; n < weapon.muzzles.Count; n++)
                            {
                                uint muzzleOrSupp = weapon.muzzles.ElementAt(n);

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

                        if (weapon.flags[1])
                        {
                            for (int n = 0; n < weapon.clips.Count; n++)
                            {
                                uint clip = weapon.clips.ElementAt(n);

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

                        if (weapon.flags[4])
                        {
                            for (int n = 0; n < weapon.scopes.Count; n++)
                            {
                                uint scope = weapon.scopes.ElementAt(n);

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

                        if (weapon.flags[3])
                        {
                            for (int n = 0; n < weapon.grips.Count; n++)
                            {
                                uint grip = weapon.grips.ElementAt(n);

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

                        if (weapon.flags[2])
                        {
                            for (int n = 0; n < weapon.barrels.Count; n++)
                            {
                                uint barrel = weapon.barrels.ElementAt(n);

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

                        if (weapon.flags[5])
                        {
                            for (int n = 0; n < weapon.camos.Count; n++)
                            {
                                uint camo = weapon.camos.ElementAt(n);

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

                        if (weapon.flags[6])
                        {
                            for (int n = 0; n < weapon.flashlights.Count; n++)
                            {
                                uint flash = weapon.flashlights.ElementAt(n);

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

                        if (weapon.flags[7])
                        {
                            for (int n = 0; n < weapon.varmods.Count; n++)
                            {
                                uint varmod = weapon.varmods.ElementAt(n);

                                if (varmod == (uint)WeaponComponentHash.Invalid)
                                {
                                    continue;
                                }
                                else if (storedWeapon.HasComponent(varmod))
                                {
                                    storedWeapon.Varmod = varmod;
                                }
                            }
                        }

                        weapons.Add(storedWeapon);
                    }
                }

                for (int i = weapons.Count - 1; i > -1; i--)
                {
                    StoredWeapon weapon = weapons[i];

                    if (weapon.WeaponHash == (uint)WeaponHash.Unarmed)
                        continue;

                    bool hasWeapon = Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON, Main.PPID, weapon.WeaponHash, false);

                    if (!hasWeapon)
                    {
                        changes = true;
                        weapons.RemoveAt(i);
                        continue;
                    }

                    WeaponData weaponCatalogOption = WeaponData.Find((ti) => ti.weaponHash == weapon.WeaponHash);
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

                    if (weaponCatalogOption.flags[0])
                    {
                        for (int n = 0; n < weaponCatalogOption.muzzles.Count; n++)
                        {
                            uint attachment = weaponCatalogOption.muzzles.ElementAt(n);

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

                    if (weaponCatalogOption.flags[1])
                    {
                        for (int n = 0; n < weaponCatalogOption.clips.Count; n++)
                        {
                            uint attachment = weaponCatalogOption.clips.ElementAt(n);

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

                    if (weaponCatalogOption.flags[4])
                    {
                        for (int n = 0; n < weaponCatalogOption.scopes.Count; n++)
                        {
                            uint attachment = weaponCatalogOption.scopes.ElementAt(n);

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

                    if (weaponCatalogOption.flags[3])
                    {
                        for (int n = 0; n < weaponCatalogOption.grips.Count; n++)
                        {
                            uint attachment = weaponCatalogOption.grips.ElementAt(n);

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

                    if (weaponCatalogOption.flags[2])
                    {
                        for (int n = 0; n < weaponCatalogOption.barrels.Count; n++)
                        {
                            uint attachment = weaponCatalogOption.barrels.ElementAt(n);

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

                    if (weaponCatalogOption.flags[5])
                    {
                        for (int n = 0; n < weaponCatalogOption.camos.Count; n++)
                        {
                            uint attachment = weaponCatalogOption.camos.ElementAt(n);

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

                    if (weaponCatalogOption.flags[6])
                    {
                        for (int n = 0; n < weaponCatalogOption.flashlights.Count; n++)
                        {
                            uint attachment = weaponCatalogOption.flashlights.ElementAt(n);

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

                    if (weaponCatalogOption.flags[7])
                    {
                        for (int n = 0; n < weaponCatalogOption.varmods.Count; n++)
                        {
                            uint attachment = weaponCatalogOption.varmods.ElementAt(n);

                            if (attachment == (uint)WeaponComponentHash.Invalid || attachment == weapon.Varmod)
                            {
                                continue;
                            }
                            else if (weapon.HasComponent(attachment))
                            {
                                weapon.Varmod = attachment;
                            }
                        }
                    }
                }
            } catch (Exception /*e*/) 
            {
                //GTA.UI.Notification.Show($"{e.StackTrace}");
            }

            if (changes)
            {
                LoadoutSaving.PerformSave(MapperMain.CurrentPed);
            }
        }
    }
}
