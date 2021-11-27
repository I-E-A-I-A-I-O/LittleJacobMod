using System;
using System.Collections.Generic;
using GTA;
using LittleJacobMod.Saving.Utils;
using System.IO;
using GTA.Native;

namespace LittleJacobMod.Saving
{
    internal static class LoadoutSaving
    {
        static readonly List<StoredWeapon> StoredWeapons = new List<StoredWeapon>();
        public static bool Busy { get; private set; } = false;
        public static event EventHandler WeaponsLoaded;

        public static void UpdateWeaponMap()
        {
            Mapper.Process(StoredWeapons);
        }

        public static int Count()
        {
            return StoredWeapons.Count;
        }

        public static bool IsWeaponInStore(WeaponHash weapon)
        {
            foreach (var storedWeapon in StoredWeapons)
            {
                if (storedWeapon.WeaponHash == weapon)
                {
                    return true;
                }
            }
            return false;
        }

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

        public static void SetAmmo(WeaponHash hash, int ammo)
        {
            for (var i = 0; i < StoredWeapons.Count; i++)
            {
                if (StoredWeapons[i].WeaponHash == hash)
                {
                    StoredWeapons[i].Ammo = ammo;
                    return;
                }
            }
        }

        public static void UpdateAmmo(WeaponHash hash, int ammo)
        {
            var playerHandle = Game.Player.Character.Handle;
            var currentWeaponAmmoType = Function.Call<uint>(Hash.GET_PED_AMMO_TYPE_FROM_WEAPON, playerHandle, hash);
            foreach (StoredWeapon weapon in StoredWeapons)
            {
                if (weapon.WeaponHash == hash)
                {
                    weapon.Ammo = ammo;
                } else
                {
                    var storedWeaponAmmoType = Function.Call<uint>(Hash.GET_PED_AMMO_TYPE_FROM_WEAPON, playerHandle, weapon.WeaponHash);
                    if (storedWeaponAmmoType == currentWeaponAmmoType)
                    {
                        weapon.Ammo = ammo;
                    }
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

        public static void PerformSave(PedHash ped)
        {
            if (!Main.SavingEnabled)
            {
                return;
            }

            GTA.UI.LoadingPrompt.Show("Saving weapon loadout...");
            Busy = true;
            try
            {
                var dir = Directory.GetCurrentDirectory();
                var filePath = $"{dir}\\scripts\\LittleJacobMod\\Loadouts\\{(int)ped}.data";
                
                if (!Directory.Exists($"{dir}\\scripts\\LittleJacobMod\\Loadouts"))
                {
                    Directory.CreateDirectory($"{dir}\\scripts\\LittleJacobMod\\Loadouts");
                }

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                using (var writer = new BinaryWriter(File.Open(filePath, FileMode.Create, FileAccess.Write)))
                {
                    writer.Write(StoredWeapons.Count);
                    foreach (var weapon in StoredWeapons)
                    {
                        writer.Write(weapon.Ammo);
                        writer.Write(weapon.Barrel.ToString());
                        writer.Write(weapon.Camo.ToString());
                        writer.Write(weapon.CamoColor);
                        writer.Write(weapon.Clip.ToString());
                        writer.Write(weapon.Flashlight.ToString());
                        writer.Write(weapon.Grip.ToString());
                        writer.Write(weapon.Muzzle.ToString());
                        writer.Write(weapon.Scope.ToString());
                        writer.Write(weapon.Tint);
                        writer.Write(weapon.WeaponHash.ToString());
                    }
                }
            } catch (Exception)
            {
                GTA.UI.Notification.Show("~g~LittleJacobMod:~w~ Error saving weapon loadout!");
            } finally
            {
                Script.Wait(1000);
                GTA.UI.LoadingPrompt.Hide();
                Busy = false;
            }
        }

        public static void PerformLoad(bool constructor = false)
        {
            if (!constructor)
            {
                GTA.UI.LoadingPrompt.Show("Loading weapon loadout...");
            }

            Busy = true;
            StoredWeapons.Clear();
            var characterHandle = Game.Player.Character.Handle;
            var weaponsRemoved = false;

            if (!Main.IsMainCharacter())
            {
                RemoveWeapons();
                weaponsRemoved = true;
            }

            try
            {
                var dir = Directory.GetCurrentDirectory();
                var filePath = $"{dir}\\scripts\\LittleJacobMod\\Loadouts\\{Game.Player.Character.Model.Hash}.data";
                if (!Directory.Exists($"{dir}\\scripts\\LittleJacobMod\\Loadouts"))
                {
                    return;
                } else if (!File.Exists(filePath))
                {
                    GTA.UI.Notification.Show("~g~LittleJacobMod:~w~ No weapon loadouts saved for this ped!");
                    return;
                }

                if (!weaponsRemoved && !Main.MissionFlag)
                {
                    RemoveWeapons();
                }

                var loadedAmmoTypes = new List<uint>();

                using (var reader = new BinaryReader(File.Open(filePath, FileMode.Open, FileAccess.Read)))
                {
                    var count = reader.ReadInt32();
                    for (var i = 0; i < count; i++)
                    {
                        var ammo = reader.ReadInt32();
                        Enum.TryParse<WeaponComponentHash>(reader.ReadString(), out var barrel);
                        Enum.TryParse<WeaponComponentHash>(reader.ReadString(), out var camo);
                        var camoColor = reader.ReadInt32();
                        Enum.TryParse<WeaponComponentHash>(reader.ReadString(), out var clip);
                        Enum.TryParse<WeaponComponentHash>(reader.ReadString(), out var flash);
                        Enum.TryParse<WeaponComponentHash>(reader.ReadString(), out var grip);
                        Enum.TryParse<WeaponComponentHash>(reader.ReadString(), out var muzzle);
                        Enum.TryParse<WeaponComponentHash>(reader.ReadString(), out var scope);
                        var tint = reader.ReadInt32();
                        Enum.TryParse<WeaponHash>(reader.ReadString(), out var weaponHash);

                        if (Game.Player.Character.Weapons.HasWeapon(weaponHash))
                        {
                            Game.Player.Character.Weapons.Remove(weaponHash);
                        }
                        
                        Game.Player.Character.Weapons.Give(weaponHash, 0, false, false);

                        var storedWeapon = new StoredWeapon(weaponHash)
                        {
                            Ammo = ammo
                        };

                        if (barrel != WeaponComponentHash.Invalid)
                        {
                            Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, characterHandle, weaponHash, barrel);
                            storedWeapon.Barrel = barrel;
                        }

                        if (camo != WeaponComponentHash.Invalid)
                        {
                            Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, characterHandle, weaponHash, camo);

                            var slide = LittleJacobMod.Utils.TintsAndCamos.ReturnSlide(camo);

                            if (slide != WeaponComponentHash.Invalid)
                            {
                                Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, characterHandle, weaponHash, slide);
                            }

                            storedWeapon.Camo = camo;
                            
                            if (camoColor != -1)
                            {
                                Function.Call(Hash._SET_PED_WEAPON_LIVERY_COLOR, characterHandle, weaponHash, camo, camoColor);
                                
                                if (slide != WeaponComponentHash.Invalid)
                                {
                                    Function.Call(Hash._SET_PED_WEAPON_LIVERY_COLOR, characterHandle, weaponHash, slide, camoColor);
                                }

                                storedWeapon.CamoColor = camoColor;
                            }
                        }

                        if (clip != WeaponComponentHash.Invalid)
                        {
                            Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, characterHandle, weaponHash, clip);
                            storedWeapon.Clip = clip;
                        }

                        if (flash != WeaponComponentHash.Invalid)
                        {
                            Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, characterHandle, weaponHash, flash);
                            storedWeapon.Flashlight = flash;
                        }

                        if (grip != WeaponComponentHash.Invalid)
                        {
                            Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, characterHandle, weaponHash, grip);
                            storedWeapon.Grip = grip;
                        }

                        if (muzzle != WeaponComponentHash.Invalid)
                        {
                            Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, characterHandle, weaponHash, muzzle);
                            storedWeapon.Muzzle = muzzle;
                        }

                        if (scope != WeaponComponentHash.Invalid)
                        {
                            Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, characterHandle, weaponHash, scope);
                            storedWeapon.Scope = scope;
                        }

                        if (tint != -1)
                        {
                            Function.Call(Hash.SET_PED_WEAPON_TINT_INDEX, characterHandle, weaponHash, tint);
                            storedWeapon.Tint = tint;
                        }

                        var ammoType = Function.Call<uint>(Hash.GET_PED_AMMO_TYPE_FROM_WEAPON, Game.Player.Character.Handle, weaponHash);
                        
                        if (loadedAmmoTypes.Contains(ammoType))
                        {
                            StoredWeapons.Add(storedWeapon);
                            continue;
                        }

                        Function.Call(Hash._ADD_AMMO_TO_PED_BY_TYPE, Game.Player.Character.Handle, ammoType, ammo);
                        loadedAmmoTypes.Add(ammoType);
                        StoredWeapons.Add(storedWeapon);
                    }
                }
            }
            catch (Exception)
            {
                GTA.UI.Notification.Show("~g~LittleJacobMod:~w~ Error loading weapon loadout!");
            }
            finally
            {
                if (!constructor)
                {
                    Script.Wait(1000);
                }
                
                GTA.UI.LoadingPrompt.Hide();
                Busy = false;
            }

            WeaponsLoaded?.Invoke(null, null);
        }

        public static void RemoveWeapons()
        {
            Game.Player.Character.Weapons.RemoveAll();
        }
    }
}
