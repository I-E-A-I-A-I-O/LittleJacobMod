using System;
using System.IO;
using GTA;

namespace LittleJacobMod.Saving
{
    public struct DeliverySaving
    {
        public static int PoliceChanceHigh = 30;
        public static int PoliceChanceLow = 8;
        public static int BadDealChance = 5;
        public static int StartChaseChance = 5;
        public static int HighSpeedChance = 5;
        public static int DealCount;
        public static int BaseChaseChance = 5;
        public static int BaseHighSpeedChance = 5;

        public static void GoodDeal()
        {
            if (PoliceChanceHigh > 30)
                PoliceChanceHigh -= 1;
            if (PoliceChanceLow > 8)
                PoliceChanceLow -= 1;
            if (BadDealChance > 5)
                BadDealChance -= 1;
            if (StartChaseChance > BaseChaseChance)
                StartChaseChance -= 1;
            if (HighSpeedChance < 45)
                HighSpeedChance += 1;
        }

        public static void BadDeal()
        {
            if (PoliceChanceHigh < 60)
                PoliceChanceHigh += 1;
            if (PoliceChanceLow < 30)
                PoliceChanceLow += 1;
            if (BadDealChance < 30)
                BadDealChance += 2;
            if (StartChaseChance < 40)
                StartChaseChance += 2;
            if (HighSpeedChance > BaseHighSpeedChance)
                HighSpeedChance -= 1;
        }
        
        public static void Save()
        {
            GTA.UI.LoadingPrompt.Show("Saving delivery data...");
            try
            {
                var dir = Directory.GetCurrentDirectory();
                var filePath = $"{dir}\\scripts\\LittleJacobMod\\Missions\\delivery.data";

                if (!Directory.Exists($"{dir}\\scripts\\LittleJacobMod\\Missions"))
                {
                    Directory.CreateDirectory($"{dir}\\scripts\\LittleJacobMod\\Missions");
                }

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                using var writer = new BinaryWriter(File.Open(filePath, FileMode.Create, FileAccess.Write));
                writer.Write(PoliceChanceHigh);
                writer.Write(PoliceChanceLow);
                writer.Write(BadDealChance);
                writer.Write(HighSpeedChance);
                writer.Write(StartChaseChance);
                writer.Write(BaseChaseChance);
                writer.Write(DealCount);
                writer.Write(BaseHighSpeedChance);
            }
            catch (Exception)
            {
                GTA.UI.Notification.Show("~g~LittleJacobMod:~w~ Error saving delivery data!");
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
                GTA.UI.LoadingPrompt.Show("Loading delivery data...");
            }

            try
            {
                var dir = Directory.GetCurrentDirectory();
                var filePath = $"{dir}\\scripts\\LittleJacobMod\\Missions\\delivery.data";

                if (!Directory.Exists($"{dir}\\scripts\\LittleJacobMod\\Missions") || !File.Exists(filePath))
                {
                    return;
                }

                using var reader = new BinaryReader(File.Open(filePath, FileMode.Open, FileAccess.Read));
                PoliceChanceHigh = reader.ReadInt32();
                PoliceChanceLow = reader.ReadInt32();
                BadDealChance = reader.ReadInt32();
                HighSpeedChance = reader.ReadInt32();
                StartChaseChance = reader.ReadInt32();
                BaseChaseChance = reader.ReadInt32();
                DealCount = reader.ReadInt32();
                BaseHighSpeedChance = reader.ReadInt32();
            }
            catch (Exception)
            {
                GTA.UI.Notification.Show("~g~LittleJacobMod:~w~ Error loading delivery data!");
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