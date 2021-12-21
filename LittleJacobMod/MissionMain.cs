using System;
using System.Collections.Generic;
using GTA;
using GTA.Native;
using GTA.Math;
using LittleJacobMod.Utils;

namespace LittleJacobMod
{
    class MissionMain : Script
    {
        public static bool Active { get; private set; }
        List<int> _peds;
        List<int> _vehicles;
        int _objective;
        string dir;
        Controls CancelMissionKey;

        public MissionMain()
        {
            var settings = ScriptSettings.Load("scripts\\LittleJacobMod.ini");
            dir = $"{BaseDirectory}\\LittleJacob\\Missions";
            CancelMissionKey = settings.GetValue("Controls", "CancelMission", Controls.INPUT_SWITCH_VISOR);

            Tick += OnTick;
        }

        public static void Start()
        {
            if (Function.Call<bool>(Hash.IS_PED_MODEL, Main.PPID, PedHash.Franklin))
            {

            } else if (Function.Call<bool>(Hash.IS_PED_MODEL, Main.PPID, PedHash.Michael))
            {

            } else
            {
                GTA
            }
        }

        void Quit()
        {

        }

        void Input()
        {

        }

        void OnTick(object sender, EventArgs args)
        {
            if (!Active)
                return;
        }
    }
}
