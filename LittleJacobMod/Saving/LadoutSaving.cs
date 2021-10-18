using System;
using System.Collections.Generic;
using GTA;
using LittleJacobMod.Saving.Utils;

namespace LittleJacobMod.Saving
{
    static internal class LadoutSaving
    {
        static readonly List<StoredWeapon> StoredWeapons = new List<StoredWeapon>();

        public static void AddWeapon (Weapon weapon)
        {
            foreach(var weaponInStore in StoredWeapons)
            {
                if (weaponInStore.WeaponHash == weapon.Hash)
                {
                    return;
                }
            }

            var storedWeapon = new StoredWeapon(weapon.Hash)
            {
                Ammo = weapon.Ammo
            };
            StoredWeapons.Add(storedWeapon);
        }

        public static void AddAmmo(WeaponHash hash, int ammo)
        {
            foreach (var weapon in StoredWeapons)
            {
                if (weapon.WeaponHash == hash)
                {
                    weapon.Ammo += ammo;
                    return;
                }
            }
        }

        public static void SetClip(WeaponHash hash, WeaponComponentHash componentHash)
        {
            foreach (var weapon in StoredWeapons)
            {
                if (weapon.WeaponHash == hash)
                {
                    weapon.Clip = componentHash;
                    return;
                }
            }
        }

        public static void SetGrip(WeaponHash hash, WeaponComponentHash componentHash)
        {
            foreach (var weapon in StoredWeapons)
            {
                if (weapon.WeaponHash == hash)
                {
                    weapon.Grip = componentHash;
                    return;
                }
            }
        }

        public static void SetBarrel(WeaponHash hash, WeaponComponentHash componentHash)
        {
            foreach (var weapon in StoredWeapons)
            {
                if (weapon.WeaponHash == hash)
                {
                    weapon.Barrel = componentHash;
                    return;
                }
            }
        }

        public static void SetCamo(WeaponHash hash, WeaponComponentHash componentHash)
        {
            foreach (var weapon in StoredWeapons)
            {
                if (weapon.WeaponHash == hash)
                {
                    weapon.Camo = componentHash;
                    return;
                }
            }
        }

        public static void SetTint(WeaponHash hash, int tint)
        {
            foreach (var weapon in StoredWeapons)
            {
                if (weapon.WeaponHash == hash)
                {
                    weapon.Tint = tint;
                    return;
                }
            }
        }

        public static void SetCamoColor(WeaponHash hash, int color)
        {
            foreach (var weapon in StoredWeapons)
            {
                if (weapon.WeaponHash == hash)
                {
                    weapon.CamoColor = color;
                    return;
                }
            }
        }

        public static void SetFlashlight(WeaponHash hash, WeaponComponentHash flashlight)
        {
            foreach (var weapon in StoredWeapons)
            {
                if (weapon.WeaponHash == hash)
                {
                    weapon.Flashlight = flashlight;
                    return;
                }
            }
        }

        public static void SetMuzzle(WeaponHash hash, WeaponComponentHash muzzle)
        {
            foreach(var weapon in StoredWeapons)
            {
                if (weapon.WeaponHash == hash)
                {
                    weapon.Muzzle = muzzle;
                    return;
                }
            }
        }

        public static void SetScope(WeaponHash hash, WeaponComponentHash scope)
        {
            foreach (var weapon in StoredWeapons)
            {
                if (weapon.WeaponHash == hash)
                {
                    weapon.Scope = scope;
                    return;
                }
            }
        }

        public static StoredWeapon GetStoreReference(WeaponHash hash)
        {
            foreach (var weapon in StoredWeapons)
            {
                if (weapon.WeaponHash == hash)
                {
                    return weapon;
                }
            }
            return null;
        }

        public static bool HasCamo(WeaponHash hash)
        {
            foreach (var weapon in StoredWeapons)
            {
                if (weapon.WeaponHash == hash)
                {
                    return weapon.Camo != WeaponComponentHash.Invalid;
                }
            }
            return false;
        }

        public static bool IsPedMainPlayer(Ped ped)
        {
            return ped.Model.Hash == unchecked((int)PedHash.Michael) || ped.Model.Hash == unchecked((int)PedHash.Franklin) || ped.Model.Hash == unchecked((int)PedHash.Trevor);
        }
    }
}
