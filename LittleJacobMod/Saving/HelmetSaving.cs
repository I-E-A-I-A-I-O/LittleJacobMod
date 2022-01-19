using System;
using System.IO;
using GTA;

namespace LittleJacobMod.Saving
{
    public struct HelmetState
    {
        public static bool MpftvOwned;
        public static bool Mpfnv1Owned;
        public static bool Mpfnv2Owned;
        public static bool MpmtvOwned;
        public static bool Mpmnv1Owned;
        public static bool Mpmnv2Owned;

        public static void Load(bool constructor = false)
        {
            if (!constructor)
            {
                GTA.UI.LoadingPrompt.Show("Loading helmets...");
            }

            try
            {
                string dir = Directory.GetCurrentDirectory();
                string filePath = $"{dir}\\scripts\\LittleJacobMod\\Gear\\helmets.data";

                if (!Directory.Exists($"{dir}\\scripts\\LittleJacobMod\\Gear"))
                {
                    return;
                }
                else if (!File.Exists(filePath))
                {
                    GTA.UI.Notification.Show("~g~LittleJacobMod:~w~ No helmet data saved!");
                    return;
                }

                using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open, FileAccess.Read)))
                {
                    bool mpFTV = reader.ReadBoolean();
                    bool mpFNV1 = reader.ReadBoolean();
                    bool mpFNV2 = reader.ReadBoolean();
                    bool mpMTV = reader.ReadBoolean();
                    bool mpMNV1 = reader.ReadBoolean();
                    bool mpMNV2 = reader.ReadBoolean();

                    MpftvOwned = mpFTV;
                    Mpfnv1Owned = mpFNV1;
                    Mpfnv2Owned = mpFNV2;
                    MpmtvOwned = mpMTV;
                    Mpmnv1Owned = mpMNV1;
                    Mpmnv2Owned = mpMNV2;
                }
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
                string dir = Directory.GetCurrentDirectory();
                string filePath = $"{dir}\\scripts\\LittleJacobMod\\Gear\\helmets.data";

                if (!Directory.Exists($"{dir}\\scripts\\LittleJacobMod\\Gear"))
                {
                    Directory.CreateDirectory($"{dir}\\scripts\\LittleJacobMod\\Gear");
                }

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create, FileAccess.Write)))
                {
                    writer.Write(MpftvOwned);
                    writer.Write(Mpfnv1Owned);
                    writer.Write(Mpfnv2Owned);
                    writer.Write(MpmtvOwned);
                    writer.Write(Mpmnv1Owned);
                    writer.Write(Mpmnv2Owned);
                }
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
