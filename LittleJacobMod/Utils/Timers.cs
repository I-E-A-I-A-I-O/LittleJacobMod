using System;
using GTA;

namespace LittleJacobMod.Utils
{
    internal static class Timers
    {
        private static bool OfferSmokeTimerStarted { get; set; }
        private static int OfferSmokeTimerStart { get; set; }
        private static int OfferSmokeTimerCurrent { get; set; }
        private static bool AutoSaveTimerStarted { get; set; }
        private static int AutoSaveTimerStart { get; set; }
        private static int AutoSaveTimerCurrent { get; set; }

        public static bool AutoSaveTimer()
        {
            if (!AutoSaveTimerStarted)
            {
                AutoSaveTimerStarted = true;
                AutoSaveTimerStart = Game.GameTime;
                return false;
            }
            else
            {
                AutoSaveTimerCurrent = Game.GameTime;
                if (AutoSaveTimerCurrent - AutoSaveTimerStart >= 600000)
                {
                    AutoSaveTimerStarted = false;
                    return true;
                }
                return false;
            }
        }

        public static bool OfferSmoke()
        {
            if (!OfferSmokeTimerStarted)
            {
                OfferSmokeTimerStarted = true;
                OfferSmokeTimerStart = Game.GameTime;
                return false;
            } else
            {
                OfferSmokeTimerCurrent = Game.GameTime;
                if (OfferSmokeTimerCurrent - OfferSmokeTimerStart > 4000)
                {
                    OfferSmokeTimerStarted = false;
                    return true;
                }
                return false;
            }
        }

        public static void RestartOfferSmokeTimer()
        {
            OfferSmokeTimerStarted = false;
        }
    }
}
