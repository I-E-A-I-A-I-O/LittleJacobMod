using System.Collections.Generic;
using System.Linq;
using GTA;
using GTA.Native;
using LittleJacobMod.Saving.Utils;
using LittleJacobMod.Utils.Types;
using Weapon = LittleJacobMod.Utils.Types.Weapon;

namespace LittleJacobMod.Saving
{
    internal static class Mapper
    {
        public static List<Weapon> WeaponData = new();

        public static void Process(List<StoredWeapon> weapons, bool updating)
        {
            if (Main.PPID == 0 || !Main.MenuCreated || updating) return;

            var changes = false;

            foreach (var weapon in from weapon in WeaponData let hasWeapon = Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON, Main.PPID, weapon.Hash, false) let isInStore = LoadoutSaving.IsWeaponInStore(weapon.Hash) where hasWeapon && !isInStore select weapon)
            {
                changes = true;
                StoredWeapon storedWeapon = new()
                {
                    WeaponHash = weapon.Hash
                };
                storedWeapon.Tint = storedWeapon.GetTintIndex();
                storedWeapon.Ammo = Function.Call<int>(Hash.GET_AMMO_IN_PED_WEAPON, Main.PPID, weapon.Hash);
                storedWeapon.Attachments = new Dictionary<string, GroupedComponent>();

                if (weapon.Attachments != null)
                    foreach (var attachment in weapon.Attachments
                                 .Where(attachment => attachment.Hash != (uint)WeaponComponentHash.Invalid)
                                 .Where(attachment => storedWeapon.HasComponent(attachment.Hash)))
                    {
                        if (storedWeapon.Attachments.ContainsKey(attachment.Group))
                            storedWeapon.Attachments[attachment.Group] = attachment;
                        else storedWeapon.Attachments.Add(attachment.Group, attachment);
                    }

                if (weapon.CamoComponents != null)
                {
                    storedWeapon.Camo = new Component();
                    foreach (var camo in weapon.CamoComponents.Components.Where(camo => camo.Hash != (uint)WeaponComponentHash.Invalid).Where(camo => storedWeapon.HasComponent(camo.Hash)))
                    {
                        storedWeapon.Camo = camo;
                        storedWeapon.CamoColor = storedWeapon.GetCamoColor();
                    }
                }

                weapons.Add(storedWeapon);
            }

            for (var i = weapons.Count - 1; i > -1; i--)
            {
                var weapon = weapons[i];

                if (weapon.WeaponHash == (uint)WeaponHash.Unarmed) continue;

                var hasWeapon = Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON, Main.PPID, weapon.WeaponHash, false);

                if (!hasWeapon)
                {
                    changes = true;
                    weapons.RemoveAt(i);
                    continue;
                }

                var weaponCatalogOption = WeaponData.Find(ti => ti.Hash == weapon.WeaponHash);
                var ammo = Function.Call<int>(Hash.GET_AMMO_IN_PED_WEAPON, Main.PPID, weapon.WeaponHash);

                if (ammo != weapon.Ammo && !updating)
                {
                    weapon.Ammo = ammo;
                }

                var tintIndex = weapon.GetTintIndex();

                if (tintIndex != weapon.Tint)
                {
                    weapon.Tint = tintIndex;
                    changes = true;
                }

                if (weaponCatalogOption.Attachments != null)
                    foreach (var attachment in from attachment in weaponCatalogOption.Attachments
                             where attachment.Hash != (uint)WeaponComponentHash.Invalid
                             where weapon.Attachments != null
                             where weapon.Attachments.ContainsKey(attachment.Group)
                             where attachment.Hash != weapon.Attachments[attachment.Group].Hash
                             where weapon.HasComponent(attachment.Hash)
                             select attachment)
                    {
                        if (weapon.Attachments == null) break;
                        weapon.Attachments[attachment.Group] = attachment;
                        changes = true;
                    }

                if (weaponCatalogOption.CamoComponents?.Components == null) continue;
                foreach (var camo in weaponCatalogOption.CamoComponents.Components.Where(camo => camo.Hash != (uint)WeaponComponentHash.Invalid).Where(camo => weapon.Camo != null && camo.Hash != weapon.Camo.Hash).Where(camo => weapon.HasComponent(camo.Hash)))
                {
                    weapon.Camo = camo;
                    weapon.CamoColor = weapon.GetCamoColor();
                    changes = true;
                }
            }

            if (changes)
            {
                LoadoutSaving.PerformSave(MapperMain.CurrentPed);
            }
        }
    }
}
