using System.Collections.Generic;
using System.Linq;
using GTA;
using GTA.Native;
using LittleJacobMod.Saving.Utils;

namespace LittleJacobMod.Saving
{
    internal struct WeaponData
    {
        public List<bool> Flags;
        public List<uint> Muzzles;
        public List<uint> Clips;
        public List<uint> Barrels;
        public List<uint> Grips;
        public List<uint> Scopes;
        public List<uint> Camos;
        public List<uint> Flashlights;
        public List<uint> Varmods;
        public uint WeaponHash;
    }

    internal class Mapper
    {
        public static readonly List<WeaponData> WeaponData = new List<WeaponData>();

        public static void Process(List<StoredWeapon> weapons, bool updating)
        {
            if (Main.PPID == 0 || !Main.MenuCreated)
                return;

            bool changes = false;

            for (int i = 0; i < WeaponData.Count; i++)
            {
                WeaponData weapon = WeaponData[i];
                bool hasWeapon = Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON, Main.PPID, weapon.WeaponHash, false);
                bool isInStore = LoadoutSaving.IsWeaponInStore(weapon.WeaponHash);

                if (!hasWeapon || isInStore) continue;
                changes = true;
                StoredWeapon storedWeapon = new StoredWeapon(weapon.WeaponHash);
                storedWeapon.Tint = storedWeapon.GetTintIndex();
                storedWeapon.Ammo = Function.Call<int>(Hash.GET_AMMO_IN_PED_WEAPON, Main.PPID, weapon.WeaponHash);

                if (weapon.Flags[0])
                {
                    for (int n = 0; n < weapon.Muzzles.Count; n++)
                    {
                        uint muzzleOrSupp = weapon.Muzzles.ElementAt(n);

                        if (muzzleOrSupp == (uint)WeaponComponentHash.Invalid)
                        {
                            continue;
                        }

                        if (storedWeapon.HasComponent(muzzleOrSupp))
                        {
                            storedWeapon.Muzzle = muzzleOrSupp;
                        }
                    }
                }

                if (weapon.Flags[1])
                {
                    for (int n = 0; n < weapon.Clips.Count; n++)
                    {
                        uint clip = weapon.Clips.ElementAt(n);

                        if (clip == (uint)WeaponComponentHash.Invalid)
                        {
                            continue;
                        }

                        if (storedWeapon.HasComponent(clip))
                        {
                            storedWeapon.Clip = clip;
                        }
                    }
                }

                if (weapon.Flags[4])
                {
                    for (int n = 0; n < weapon.Scopes.Count; n++)
                    {
                        uint scope = weapon.Scopes.ElementAt(n);

                        if (scope == (uint)WeaponComponentHash.Invalid)
                        {
                            continue;
                        }

                        if (storedWeapon.HasComponent(scope))
                        {
                            storedWeapon.Scope = scope;
                        }
                    }
                }

                if (weapon.Flags[3])
                {
                    for (int n = 0; n < weapon.Grips.Count; n++)
                    {
                        uint grip = weapon.Grips.ElementAt(n);

                        if (grip == (uint)WeaponComponentHash.Invalid)
                        {
                            continue;
                        }

                        if (storedWeapon.HasComponent(grip))
                        {
                            storedWeapon.Grip = grip;
                        }
                    }
                }

                if (weapon.Flags[2])
                {
                    for (int n = 0; n < weapon.Barrels.Count; n++)
                    {
                        uint barrel = weapon.Barrels.ElementAt(n);

                        if (barrel == (uint)WeaponComponentHash.Invalid)
                        {
                            continue;
                        }

                        if (storedWeapon.HasComponent(barrel))
                        {
                            storedWeapon.Barrel = barrel;
                        }
                    }
                }

                if (weapon.Flags[5])
                {
                    for (int n = 0; n < weapon.Camos.Count; n++)
                    {
                        uint camo = weapon.Camos.ElementAt(n);

                        if (camo == (uint)WeaponComponentHash.Invalid)
                        {
                            continue;
                        }

                        if (!storedWeapon.HasComponent(camo)) continue;
                        storedWeapon.Camo = camo;
                        storedWeapon.CamoColor = storedWeapon.GetCamoColor();
                    }
                }

                if (weapon.Flags[6])
                {
                    for (int n = 0; n < weapon.Flashlights.Count; n++)
                    {
                        uint flash = weapon.Flashlights.ElementAt(n);

                        if (flash == (uint)WeaponComponentHash.Invalid)
                        {
                            continue;
                        }

                        if (storedWeapon.HasComponent(flash))
                        {
                            storedWeapon.Flashlight = flash;
                        }
                    }
                }

                if (weapon.Flags[7])
                {
                    for (int n = 0; n < weapon.Varmods.Count; n++)
                    {
                        uint varmod = weapon.Varmods.ElementAt(n);

                        if (varmod == (uint)WeaponComponentHash.Invalid)
                        {
                            continue;
                        }

                        if (storedWeapon.HasComponent(varmod))
                        {
                            storedWeapon.Varmod = varmod;
                        }
                    }
                }

                weapons.Add(storedWeapon);
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

                WeaponData weaponCatalogOption = WeaponData.Find(ti => ti.WeaponHash == weapon.WeaponHash);
                int ammo = Function.Call<int>(Hash.GET_AMMO_IN_PED_WEAPON, Main.PPID, weapon.WeaponHash);

                if (ammo != weapon.Ammo && !updating)
                {
                    weapon.Ammo = ammo;
                }

                int tintIndex = weapon.GetTintIndex();

                if (tintIndex != weapon.Tint)
                {
                    weapon.Tint = tintIndex;
                }

                if (weaponCatalogOption.Flags[0])
                {
                    for (int n = 0; n < weaponCatalogOption.Muzzles.Count; n++)
                    {
                        uint attachment = weaponCatalogOption.Muzzles.ElementAt(n);

                        if (attachment == (uint)WeaponComponentHash.Invalid || attachment == weapon.Muzzle)
                        {
                            continue;
                        }

                        if (weapon.HasComponent(attachment))
                        {
                            weapon.Muzzle = attachment;
                        }
                    }
                }

                if (weaponCatalogOption.Flags[1])
                {
                    for (int n = 0; n < weaponCatalogOption.Clips.Count; n++)
                    {
                        uint attachment = weaponCatalogOption.Clips.ElementAt(n);

                        if (attachment == (uint)WeaponComponentHash.Invalid || attachment == weapon.Clip)
                        {
                            continue;
                        }

                        if (weapon.HasComponent(attachment))
                        {
                            weapon.Clip = attachment;
                        }
                    }
                }

                if (weaponCatalogOption.Flags[4])
                {
                    for (int n = 0; n < weaponCatalogOption.Scopes.Count; n++)
                    {
                        uint attachment = weaponCatalogOption.Scopes.ElementAt(n);

                        if (attachment == (uint)WeaponComponentHash.Invalid || attachment == weapon.Scope)
                        {
                            continue;
                        }

                        if (weapon.HasComponent(attachment))
                        {
                            weapon.Scope = attachment;
                        }
                    }
                }

                if (weaponCatalogOption.Flags[3])
                {
                    for (int n = 0; n < weaponCatalogOption.Grips.Count; n++)
                    {
                        uint attachment = weaponCatalogOption.Grips.ElementAt(n);

                        if (attachment == (uint)WeaponComponentHash.Invalid || attachment == weapon.Grip)
                        {
                            continue;
                        }

                        if (weapon.HasComponent(attachment))
                        {
                            weapon.Grip = attachment;
                        }
                    }
                }

                if (weaponCatalogOption.Flags[2])
                {
                    for (int n = 0; n < weaponCatalogOption.Barrels.Count; n++)
                    {
                        uint attachment = weaponCatalogOption.Barrels.ElementAt(n);

                        if (attachment == (uint)WeaponComponentHash.Invalid || attachment == weapon.Barrel)
                        {
                            continue;
                        }

                        if (weapon.HasComponent(attachment))
                        {
                            weapon.Barrel = attachment;
                        }
                    }
                }

                if (weaponCatalogOption.Flags[5])
                {
                    for (int n = 0; n < weaponCatalogOption.Camos.Count; n++)
                    {
                        uint attachment = weaponCatalogOption.Camos.ElementAt(n);

                        if (attachment == (uint)WeaponComponentHash.Invalid || attachment == weapon.Camo)
                        {
                            continue;
                        }

                        if (!weapon.HasComponent(attachment)) continue;
                        weapon.Camo = attachment;
                        weapon.CamoColor = weapon.GetCamoColor();
                    }
                }

                if (weaponCatalogOption.Flags[6])
                {
                    for (int n = 0; n < weaponCatalogOption.Flashlights.Count; n++)
                    {
                        uint attachment = weaponCatalogOption.Flashlights.ElementAt(n);

                        if (attachment == (uint)WeaponComponentHash.Invalid || attachment == weapon.Flashlight)
                        {
                            continue;
                        }

                        if (weapon.HasComponent(attachment))
                        {
                            weapon.Flashlight = attachment;
                        }
                    }
                }

                if (!weaponCatalogOption.Flags[7]) continue;
                {
                    for (int n = 0; n < weaponCatalogOption.Varmods.Count; n++)
                    {
                        uint attachment = weaponCatalogOption.Varmods.ElementAt(n);

                        if (attachment == (uint)WeaponComponentHash.Invalid || attachment == weapon.Varmod)
                        {
                            continue;
                        }

                        if (weapon.HasComponent(attachment))
                        {
                            weapon.Varmod = attachment;
                        }
                    }
                }
            }

            if (changes)
            {
                LoadoutSaving.PerformSave(MapperMain.CurrentPed);
            }
        }
    }
}
