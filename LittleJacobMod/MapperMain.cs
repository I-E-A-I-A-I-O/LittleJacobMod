using System;
using GTA;
using GTA.Native;
using GTA.Math;
using LittleJacobMod.Saving;
using LittleJacobMod.Utils;

internal class MapperMain : Script
{
    private readonly Vector3 _gunRange1 = new Vector3(9.053967f, -1097.277f, 28.79702f);
    private readonly Vector3 _gunRange2 = new Vector3(826.2507f, -2162.014f, 28.61901f);
    private bool _saveTriggered;
    private bool _missionFlag;
    private bool _updating;
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
        DeliverySaving.Load(!firstStart);
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

    private void AutoSaveWatch(object o, EventArgs e)
    {
        bool atRange = IsPlayerAtGunRange();

        if (!Function.Call<bool>(Hash.GET_MISSION_FLAG) && _missionFlag && !atRange)
        {
            LoadoutSaving.PerformLoad();
            _missionFlag = false;
        }
        else if (Function.Call<bool>(Hash.GET_MISSION_FLAG) || atRange)
        {
            if (!_missionFlag)
            {
                _missionFlag = true;
            }
            return;
        }

        if (Function.Call<bool>(Hash.IS_AUTO_SAVE_IN_PROGRESS))
        {
            if (!_saveTriggered)
            {
                LoadoutSaving.PerformSave(CurrentPed);
                _saveTriggered = true;
            }
            return;
        }

        if (_saveTriggered)
        {
            _saveTriggered = false;
        }

        if (Main.JacobActive) return;
        if (!LoadoutSaving.Busy && !Function.Call<bool>(Hash.IS_PLAYER_SWITCH_IN_PROGRESS) && !Game.Player.IsDead)
        {
            LoadoutSaving.UpdateWeaponMap(_updating);
        }

        if (Timers.AutoSaveTimer() && !LoadoutSaving.Busy && !Function.Call<bool>(Hash.IS_PLAYER_SWITCH_IN_PROGRESS) && !Game.Player.IsDead)
        {
            LoadoutSaving.PerformSave(CurrentPed);
        }
    }

    private void ModelWatcher(object o, EventArgs e)
    {
        if (Function.Call<bool>(Hash.IS_PED_MODEL, Main.PPID, CurrentPed) || _missionFlag)
        {
            return;
        }

        var newModel = Function.Call<uint>(Hash.GET_ENTITY_MODEL, Main.PPID);
        if (CurrentPed == newModel)
        {
            GTA.UI.Screen.ShowHelpTextThisFrame("Little Jacob Mod: Infinite loop detected.\n" +
                                                "If you're using 'Character Swap', reset the model hashes" +
                                                " to solve the issue.", false);
            return;
        }

        _updating = true;
        Game.IsNightVisionActive = false;
        Game.IsThermalVisionActive = false;
        LoadoutSaving.PerformSave(CurrentPed);
        CurrentPed = newModel;
        LoadoutSaving.PerformLoad();
        _updating = false;
    }
}
