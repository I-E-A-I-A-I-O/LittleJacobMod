using System;
using System.Collections.Generic;
using NAudio.Wave;
using System.IO;

namespace LittleJacobMod.Utils
{
    internal static class Voice
    {
        static string DirPath => $"{Directory.GetCurrentDirectory()}\\scripts\\LittleJacobMod\\VoiceLines\\LittleJacob";
        public static bool Playing { get; private set; }
        static int LastPlayed { get; set; } = 1;

        public static void PlayHello()
        {
            PlaySingle($"{DirPath}\\HELLO.wav");
        }

        public static void PlayBye()
        {
            var random = new Random();
            PlaySingle($"{DirPath}\\BYE_0{random.Next(1, 3)}.wav");
        }

        public static void PlayPolice()
        {
            var random = new Random();
            PlaySingle($"{DirPath}\\POLICE_0{random.Next(1, 6)}.wav");
        }

        public static void PlaySmokeOffer()
        {
            PlaySingle($"{DirPath}\\OFFERING_SMOKE.wav");
        }

        public static void PlayPurchase()
        {
            var random = new Random();
            var num = random.Next(1, 6);
            while (num == LastPlayed)
            {
                num = random.Next(1, 6);
            }
            LastPlayed = num;
            PlaySingle($"{DirPath}\\BUYING_0{num}.wav");
        }

        static void PlaySingle(string file)
        {
            try
            {
                Playing = true;
                WaveStream mainOutputStream = new WaveFileReader(file);
                WaveChannel32 waveChannel32 = new WaveChannel32(mainOutputStream)
                {
                    PadWithZeroes = false,
                    Volume = 0.15f
                };

                WaveOutEvent player = new WaveOutEvent();
                player.Init(waveChannel32);
                //player.Volume = 1;
                player.Play();
                player.PlaybackStopped += (sender, args) =>
                {
                    player.Dispose();
                    waveChannel32.Dispose();
                    mainOutputStream.Dispose();
                    Playing = false;
                };
            }
            catch (Exception)
            {
                Playing = false;
            }
        }
    }
}
