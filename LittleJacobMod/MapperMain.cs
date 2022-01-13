using System;
using System.Collections.Generic;
using GTA;
using GTA.Native;
using GTA.Math;
using LittleJacobMod.Saving;
using LittleJacobMod.Utils;

class MapperMain : Script
{
    Vector3 _gunRange1 = new Vector3(9.053967f, -1097.277f, 28.79702f);
    Vector3 _gunRange2 = new Vector3(826.2507f, -2162.014f, 28.61901f);
    bool SaveTriggered;
    bool MissionFlag;
    bool updating;
    public static uint CurrentPed;

    public MapperMain()
    {
        /*if (Game.IsLoading)
        {*/
            Tick += WaitForGameLoad;
        //}
        /*else
        {
            Initialize(false);
        }*/
    }

    private void Initialize(bool firstStart)
    {
        int id = Function.Call<int>(Hash.PLAYER_PED_ID);
        CurrentPed = Function.Call<uint>(Hash.GET_ENTITY_MODEL, id);

        if (id == 0 || CurrentPed == 0)
            return;

        if (firstStart)
        {
            Tick -= WaitForGameLoad;
        }

        Main.PPID = id;
        LoadoutSaving.PerformLoad(!firstStart);
        HelmetState.Load(!firstStart);
        MissionSaving.Load(!firstStart);
        Tick += ModelWatcher;
        Tick += AutoSaveWatch;
    }

    private void WaitForGameLoad(object o, EventArgs e)
    {
        if (!Game.IsLoading)
        {
            Initialize(true);
        }
    }

    private bool IsPlayerAtGunRange()
    {
        return Game.Player.Character.IsInRange(_gunRange1, 12) || Game.Player.Character.IsInRange(_gunRange2, 12);
    }

    void AutoSaveWatch(object o, EventArgs e)
    {
        bool atRange = IsPlayerAtGunRange();

        if (!Function.Call<bool>(Hash.GET_MISSION_FLAG) && MissionFlag && !atRange)
        {
            LoadoutSaving.PerformLoad();
            MissionFlag = false;
        }
        else if (Function.Call<bool>(Hash.GET_MISSION_FLAG) || atRange)
        {
            if (!MissionFlag)
            {
                MissionFlag = true;
            }
            return;
        }

        if (Function.Call<bool>(Hash.IS_AUTO_SAVE_IN_PROGRESS))
        {
            if (!SaveTriggered)
            {
                LoadoutSaving.PerformSave(CurrentPed);
                SaveTriggered = true;
            }
            return;
        }

        if (SaveTriggered)
        {
            SaveTriggered = false;
        }

        if (!Main.JacobActive)
        {
            if (!LoadoutSaving.Busy && !Function.Call<bool>(Hash.IS_PLAYER_SWITCH_IN_PROGRESS) && !Game.Player.IsDead)
            {
                LoadoutSaving.UpdateWeaponMap(updating);
            }

            if (Timers.AutoSaveTimer() && !LoadoutSaving.Busy && !Function.Call<bool>(Hash.IS_PLAYER_SWITCH_IN_PROGRESS) && !Game.Player.IsDead)
            {
                LoadoutSaving.PerformSave(CurrentPed);
            }
        }
    }

    void ModelWatcher(object o, EventArgs e)
    {
        if (Function.Call<bool>(Hash.IS_PED_MODEL, Main.PPID, CurrentPed) || MissionFlag)
        {
            return;
        }

        updating = true;
        Game.IsNightVisionActive = false;
        Game.IsThermalVisionActive = false;
        LoadoutSaving.PerformSave(CurrentPed);
        CurrentPed = Function.Call<uint>(Hash.GET_ENTITY_MODEL, Main.PPID);
        LoadoutSaving.PerformLoad();
        updating = false;
    }
}
