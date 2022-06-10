using System;
using System.IO;
using System.Runtime.CompilerServices;
using GTA;
using LittleJacobMod.Utils.Types;
using Newtonsoft.Json;

namespace LittleJacobMod.Saving
{
    public static class HelmetSaving
    {
        public static HelmetOwnership? State;
        public static event EventHandler? HelmetsLoaded;

        public static void Load(bool constructor = false)
        {
            if (!constructor)
            {
                GTA.UI.LoadingPrompt.Show("Loading helmets...");
            }

            try
            {
                var dir = Directory.GetCurrentDirectory();
                var filePath = $"{dir}\\scripts\\LittleJacobMod\\Gear\\helmets.json";

                if (!Directory.Exists($"{dir}\\scripts\\LittleJacobMod\\Gear"))
                {
                    State = new();
                    return;
                }
                else if (!File.Exists(filePath))
                {
                    State = new();
                    GTA.UI.Notification.Show("~g~LittleJacobMod:~w~ No helmet data saved!");
                    return;
                }

                var text = File.ReadAllText(filePath);
                State = JsonConvert.DeserializeObject<HelmetOwnership>(text);
                HelmetsLoaded?.Invoke(null, EventArgs.Empty);
            }
            catch (Exception)
            {
                GTA.UI.Notification.Show("~g~LittleJacobMod:~w~ Error loading helmets!");
            }
            finally
            {
                if (!constructor)
                {
                    Script.Wait(1000);
                }

                GTA.UI.LoadingPrompt.Hide();
            }
        }

        public static void Save()
        {
            GTA.UI.LoadingPrompt.Show("Saving helmets...");
            try
            {
                var dir = Directory.GetCurrentDirectory();
                var filePath = $"{dir}\\scripts\\LittleJacobMod\\Gear\\helmets.json";

                if (!Directory.Exists($"{dir}\\scripts\\LittleJacobMod\\Gear"))
                {
                    Directory.CreateDirectory($"{dir}\\scripts\\LittleJacobMod\\Gear");
                }

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                
                File.WriteAllText(filePath, JsonConvert.SerializeObject(State));
            }
            catch (Exception)
            {
                GTA.UI.Notification.Show("~g~LittleJacobMod:~w~ Error saving helmets!");
            }
            finally
            {
                Script.Wait(1000);
                GTA.UI.LoadingPrompt.Hide();
            }
        }
    }
}
