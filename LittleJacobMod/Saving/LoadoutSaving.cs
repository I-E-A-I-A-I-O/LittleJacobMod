using System;
using System.Collections.Generic;
using GTA;
using LittleJacobMod.Saving.Utils;
using System.IO;
using System.Linq;
using GTA.Native;
using LittleJacobMod.Utils.Types;
using Newtonsoft.Json;

namespace LittleJacobMod.Saving
{
    internal static class LoadoutSaving
    {
        private static readonly List<StoredWeapon> StoredWeapons = new();
        public static bool Busy { get; private set; }
        public static event EventHandler WeaponsLoaded;

        public static void UpdateWeaponMap(bool updating)
        {
            Mapper.Process(StoredWeapons, updating);
        }

        public static bool IsWeaponInStore(uint weapon)
        {
            for (var i = 0; i < StoredWeapons.Count; i++)
            {
                if (StoredWeapons[i].WeaponHash == weapon)
                {
                    return true;
                }
            }
            return false;
        }

        public static void AddWeapon (uint weapon)
        {
            if (StoredWeapons.Any(t => t.WeaponHash == weapon))
            {
                return;
            }

            StoredWeapon storedWeapon = new(weapon)
            {
                Ammo = Function.Call<int>(Hash.GET_AMMO_IN_PED_WEAPON, Main.PPID, weapon)
            };

            StoredWeapons.Add(storedWeapon);
        }

        public static void SetAmmo(uint hash, int ammo)
        {
            foreach (var t in StoredWeapons.Where(t => t.WeaponHash == hash))
            {
                t.Ammo = ammo;
                return;
            }
        }
        
        public static void SetAttachment(uint weaponHash, GroupedComponent component)
        {
            var weapon = StoredWeapons.Find(ti => ti.WeaponHash == weaponHash);

            if (weapon == null) return;
            weapon.Attachments ??= new Dictionary<string, GroupedComponent>();
            
            if (!weapon.Attachments.ContainsKey(component.Group))
            {
                weapon.Attachments.Add(component.Group, component);
            }
            else
            {
                weapon.Attachments[component.Group] = component;
            }
        }

        public static void SetCamo(uint weaponHash, Component component)
        {
            var weapon = StoredWeapons.Find(ti => ti.WeaponHash == weaponHash);

            if (weapon == null) return;

            weapon.Camo = component;
        }
        
        public static void SetTint(uint hash, int tint)
        {
            var weapon = StoredWeapons.Find(ti => ti.WeaponHash == hash);
            if (weapon != null) weapon.Tint = tint;
        }

        public static void SetCamoColor(uint hash, int color)
        {
            var weapon = StoredWeapons.Find(ti => ti.WeaponHash == hash);
            if (weapon != null) weapon.CamoColor = color;
        }

        public static StoredWeapon GetStoreReference(uint hash)
        {
            return StoredWeapons.Find(ti => ti.WeaponHash == hash);
        }

        public static void PerformSave(uint ped)
        {
            if (ped == 0)
                return;

            GTA.UI.LoadingPrompt.Show("Saving weapon loadout...");
            Busy = true;

            try
            {
                var dir = Directory.GetCurrentDirectory();
                var filePath = $"{dir}\\scripts\\LittleJacobMod\\Loadouts\\{ped.ToString()}.json";
                
                if (!Directory.Exists($"{dir}\\scripts\\LittleJacobMod\\Loadouts"))
                {
                    Directory.CreateDirectory($"{dir}\\scripts\\LittleJacobMod\\Loadouts");
                }

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                
                File.WriteAllText(filePath, JsonConvert.SerializeObject(StoredWeapons));

                /*using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create, FileAccess.Write)))
                {
                    writer.Write(StoredWeapons.Count);
                    foreach (StoredWeapon weapon in StoredWeapons)
                    {
                        writer.Write(weapon.Ammo);
                        writer.Write(weapon.Barrel);
                        writer.Write(weapon.Camo);
                        writer.Write(weapon.CamoColor);
                        writer.Write(weapon.Clip);
                        writer.Write(weapon.Flashlight);
                        writer.Write(weapon.Grip);
                        writer.Write(weapon.Muzzle);
                        writer.Write(weapon.Scope);
                        writer.Write(weapon.Tint);
                        writer.Write(weapon.Varmod);
                        writer.Write(weapon.WeaponHash);
                    }
                }*/
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
            if (MapperMain.CurrentPed == 0) return;

            if (!constructor)
            {
                GTA.UI.LoadingPrompt.Show("Loading weapon loadout...");
            }

            Busy = true;
            StoredWeapons.Clear();
            var weaponsRemoved = false;

            if (!Main.IsMainCharacter())
            {
                RemoveWeapons();
                weaponsRemoved = true;
            }

            var dir = Directory.GetCurrentDirectory();
            var filePath = $"{dir}\\scripts\\LittleJacobMod\\Loadouts\\{MapperMain.CurrentPed.ToString()}";

            if (!Directory.Exists($"{dir}\\scripts\\LittleJacobMod\\Loadouts") || !File.Exists($"{filePath}.json"))
            {
                GTA.UI.Notification.Show("~g~LittleJacobMod:~w~ No weapon loadouts saved for this ped!");

                if (!constructor)
                {
                    Script.Wait(1000);
                }

                GTA.UI.LoadingPrompt.Hide();
                Busy = false;
                return;
            }
            
            if (!weaponsRemoved && !Main.MissionFlag)
            {
                RemoveWeapons();
            }

            List<uint> loadedAmmoTypes = new();
            var text = File.ReadAllText($"{filePath}.json");
            var storedWeaponsList = JsonConvert.DeserializeObject<List<StoredWeapon>>(text);

            if (storedWeaponsList == null)
            {
                GTA.UI.Notification.Show("~g~Error loading weapons.");

                if (!constructor)
                {
                    Script.Wait(1000);
                }

                GTA.UI.LoadingPrompt.Hide();
                Busy = false;
                return;
            }
            
            foreach (var weapon in storedWeaponsList)
            {
                if (Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON, Main.PPID, weapon.WeaponHash, false))
                {
                    Function.Call<bool>(Hash.REMOVE_WEAPON_FROM_PED, Main.PPID, weapon.WeaponHash);
                }
                
                Function.Call<bool>(Hash.GIVE_WEAPON_TO_PED, Main.PPID, weapon.WeaponHash, 0, false, false);

                if (weapon.Attachments != null)
                {
                    foreach (var attachment in weapon.Attachments.Where(attachment => attachment.Value.Hash != (uint)WeaponComponentHash.Invalid))
                    {
                        GTA.UI.Screen.ShowSubtitle(attachment.Key);
                        Script.Wait(500);
                        Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, Main.PPID, weapon.WeaponHash, attachment.Value.Hash);
                    }    
                }
                else
                {
                    GTA.UI.Screen.ShowSubtitle("IS NULLLLLLLLLLLLLLLLLL");
                }
                
                if (weapon.Tint != -1)
                {
                    Function.Call(Hash.SET_PED_WEAPON_TINT_INDEX, Main.PPID, weapon.WeaponHash, weapon.Tint);
                }

                if (weapon.Camo is not null && weapon.Camo.Hash is not (uint)WeaponComponentHash.Invalid)
                {
                    Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, Main.PPID, weapon.WeaponHash, weapon.Camo.Hash);
                    var slide = LittleJacobMod.Utils.TintsAndCamos.ReturnSlide(weapon.Camo.Hash);

                    if (slide != (uint)WeaponComponentHash.Invalid)
                    {
                        Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, Main.PPID, weapon.WeaponHash, slide);
                    }

                    if (weapon.CamoColor != -1)
                    {
                        Function.Call(Hash._SET_PED_WEAPON_LIVERY_COLOR, Main.PPID, weapon.WeaponHash, weapon.Camo.Hash, weapon.CamoColor);

                        if (slide != (uint)WeaponComponentHash.Invalid)
                        {
                            Function.Call(Hash._SET_PED_WEAPON_LIVERY_COLOR, Main.PPID, weapon.WeaponHash, slide, weapon.CamoColor);
                        }
                    }
                }
                
                var ammoType = Function.Call<uint>(Hash.GET_PED_AMMO_TYPE_FROM_WEAPON, Game.Player.Character.Handle, weapon.WeaponHash);

                if (loadedAmmoTypes.Contains(ammoType)) continue;

                Function.Call(Hash._ADD_AMMO_TO_PED_BY_TYPE, Main.PPID, ammoType, weapon.Ammo);
                loadedAmmoTypes.Add(ammoType);
            }

            StoredWeapons.AddRange(storedWeaponsList);

            if (!constructor)
            {
                Script.Wait(1000);
            }

            GTA.UI.LoadingPrompt.Hide();
            WeaponsLoaded?.Invoke(null, EventArgs.Empty);
            Busy = false;
        }

        private static bool TakesCamo(uint weapon)
        {
            return weapon == (uint)WeaponHash.AssaultrifleMk2
                || weapon == (uint)WeaponHash.BullpupRifleMk2
                || weapon == (uint)WeaponHash.CarbineRifleMk2
                || weapon == (uint)WeaponHash.CombatMGMk2
                || weapon == (uint)WeaponHash.HeavySniperMk2
                || weapon == (uint)WeaponHash.MarksmanRifleMk2
                || weapon == (uint)WeaponHash.PistolMk2
                || weapon == (uint)WeaponHash.PumpShotgunMk2
                || weapon == (uint)WeaponHash.RevolverMk2
                || weapon == (uint)WeaponHash.SMGMk2
                || weapon == (uint)WeaponHash.SNSPistolMk2
                || weapon == (uint)WeaponHash.SpecialCarbineMk2
                || weapon == 3347935668;
        }

        private static void RemoveWeapons()
        {
            Game.Player.Character.Weapons.RemoveAll();
        }
    }
}
