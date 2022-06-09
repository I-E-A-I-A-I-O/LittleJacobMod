using System;
using System.IO;
using GTA;

namespace LittleJacobMod.Saving
{
    public struct MissionSaving
    {
        public static int MProgress = 1;
        public static int FProgress = 1;
        public static bool TUnlocked;

        public static void Save()
        {
            GTA.UI.LoadingPrompt.Show("Saving mission progress...");
            try
            {
                var dir = Directory.GetCurrentDirectory();
                var filePath = $"{dir}\\scripts\\LittleJacobMod\\Missions\\missions.data";

                if (!Directory.Exists($"{dir}\\scripts\\LittleJacobMod\\Missions"))
                {
                    Directory.CreateDirectory($"{dir}\\scripts\\LittleJacobMod\\Missions");
                }

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                using (var writer = new BinaryWriter(File.Open(filePath, FileMode.Create, FileAccess.Write)))
                {
                    writer.Write(MProgress);
                    writer.Write(FProgress);
                    writer.Write(TUnlocked);
                }
            }
            catch (Exception)
            {
                GTA.UI.Notification.Show("~g~LittleJacobMod:~w~ Error saving mission progress!");
            }
            finally
            {
                Script.Wait(1000);
                GTA.UI.LoadingPrompt.Hide();
            }
        }

        public static void Load(bool constructor = false)
        {
            if (!constructor)
            {
                GTA.UI.LoadingPrompt.Show("Loading mission progress...");
            }

            try
            {
                var dir = Directory.GetCurrentDirectory();
                var filePath = $"{dir}\\scripts\\LittleJacobMod\\Missions\\missions.data";

                if (!Directory.Exists($"{dir}\\scripts\\LittleJacobMod\\Missions") || !File.Exists(filePath))
                {
                    return;
                }

                using (var reader = new BinaryReader(File.Open(filePath, FileMode.Open, FileAccess.Read)))
                {
                    MProgress = reader.ReadInt32();
                    FProgress = reader.ReadInt32();
                    TUnlocked = reader.ReadBoolean();
                }
            }
            catch (Exception)
            {
                GTA.UI.Notification.Show("~g~LittleJacobMod:~w~ Error loading mission progress!");
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
    }
}
