using System;
using System.IO;
using GTA;

namespace LittleJacobMod.Saving
{
    public struct HelmetState
    {
        static public bool MPFTVOwned;
        static public bool MPFNV1Owned;
        static public bool MPFNV2Owned;
        static public bool MPMTVOwned;
        static public bool MPMNV1Owned;
        static public bool MPMNV2Owned;

        static public void Load(bool constructor = false)
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

                    MPFTVOwned = mpFTV;
                    MPFNV1Owned = mpFNV1;
                    MPFNV2Owned = mpFNV2;
                    MPMTVOwned = mpMTV;
                    MPMNV1Owned = mpMNV1;
                    MPMNV2Owned = mpMNV2;
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

        static public void Save()
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
                    writer.Write(MPFTVOwned);
                    writer.Write(MPFNV1Owned);
                    writer.Write(MPFNV2Owned);
                    writer.Write(MPMTVOwned);
                    writer.Write(MPMNV1Owned);
                    writer.Write(MPMNV2Owned);
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
