using System;
using GTA;
using LittleJacobMod.Interface;
using LittleJacobMod.Utils;
using LittleJacobMod.Saving;
using LittleJacobMod.Saving.Utils;
using GTA.Native;

public class Main : Script
{
    PhoneContact ifruit;
    public Menu menu;
    static public Camera cam;
    public static bool JacobActive { get; set; }
    public static bool SavingEnabled { get; private set; }
    public static LittleJacob LittleJacob { get; set; }
    public static bool TimerStarted { get; set; }
    static int TimerStart { get; set; }
    static int TimerCurrent { get; set; }
    public static bool MenuOpened { get; private set; }
    bool SaveTriggered { get; set; }
    public static PedHash JacobHash { get; private set; }
    public static Controls OpenMenuKey { get; private set; }
    public static PedHash CurrentPed { get; private set; }

    public Main()
    {
        Migrator.Migrate();

        var settings = ScriptSettings.Load("scripts\\LittleJacobMod.ini");
        OpenMenuKey = settings.GetValue("Controls", "OpenMenu", Controls.INPUT_CONTEXT);
        SavingEnabled = settings.GetValue("Gameplay", "EnableSaving", true);
        JacobHash = settings.GetValue("Gameplay", "JacobModel", PedHash.Soucent03AMY);

        ifruit = new PhoneContact();
        menu = new Menu();

        if (SavingEnabled)
        {
            Mapper.Initialize();
            if (Game.IsLoading)
            {
                Tick += WaitForGameLoad;
            }
            else
            {
                CurrentPed = (PedHash)Game.Player.Character.Model.Hash;
                //LoadoutSaving.PerformLoad();
                Tick += ModelWatcher;
            }
            Tick += AutoSaveWatch;
            Tick += WeaponUse;
        }

        Tick += ControlWatch;
        Tick += (o, e) =>
        {
            ifruit.Phone.Update();
            menu.Pool.Process();
        };
        Tick += OnTick;
        Aborted += Main_Aborted;
    }

    private void Main_Aborted(object sender, EventArgs e)
    {
        if (JacobActive)
        {
            LittleJacob.DeleteBlip();
            Game.Player.Character.CanSwitchWeapons = true;
            Game.Player.Character.Task.ClearAll();
            LittleJacob.DeleteJacob();
        }
    }

    void AutoSaveWatch(object o, EventArgs e)
    {
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

        if (!JacobActive)
        {
            if (!LoadoutSaving.Busy && !Function.Call<bool>(Hash.IS_PLAYER_SWITCH_IN_PROGRESS) && !Game.Player.IsDead)
            {
                LoadoutSaving.UpdateWeaponMap();
            }

            if (Timers.AutoSaveTimer() && !LoadoutSaving.Busy && !Function.Call<bool>(Hash.IS_PLAYER_SWITCH_IN_PROGRESS) && !Game.Player.IsDead)
            {
                LoadoutSaving.PerformSave(CurrentPed);
            }
        }
    }

    void WeaponUse(object o, EventArgs e)
    {
        if (Game.Player.Character.IsShooting)
        {
            var currentWeapon = Game.Player.Character.Weapons.Current;
            if (LoadoutSaving.IsWeaponInStore(currentWeapon.Hash))
            {
                LoadoutSaving.UpdateAmmo(currentWeapon.Hash, currentWeapon.Ammo);
            }
        }
    }

    void WaitForGameLoad(object o, EventArgs e)
    {
        if (Game.IsLoading)
        {
            return;
        }
        CurrentPed = (PedHash)Game.Player.Character.Model.Hash;
        LoadoutSaving.PerformLoad();
        Tick -= WaitForGameLoad;
        Tick += ModelWatcher;
    }

    void ModelWatcher(object o, EventArgs e)
    {
        if (Function.Call<bool>(Hash.IS_PED_MODEL, Game.Player.Character.Handle, CurrentPed))
        {
            return;
        }

        LoadoutSaving.PerformSave(CurrentPed);
        CurrentPed = (PedHash)Game.Player.Character.Model.Hash;
        LoadoutSaving.PerformLoad();
    }

    void OnTick(object o, EventArgs e)
    {
        if (!JacobActive)
        {
            return;
        }

        LittleJacob.ProcessVoice();

        if (LittleJacob.Spawned && !LittleJacob.Left)
        {
            if (cam != null && cam.Handle != 0 && cam.IsActive)
            {
                Function.Call(Hash.DRAW_LIGHT_WITH_RANGE, LittleJacob.Vehicle.RearPosition.X + (cam.Direction.X / 2), LittleJacob.Vehicle.RearPosition.Y + (cam.Direction.Y / 2), LittleJacob.Vehicle.RearPosition.Z + 0.3f, 255, 255, 255, 1.5f, 0.5f);
            }
        }

        if (Game.Player.WantedLevel > 0)
        {
            if (LittleJacob.Spawned && !LittleJacob.Left)
            {
                menu.Pool.HideAll();
                cam.IsActive = false;
                cam.Delete();
                Function.Call(Hash.RENDER_SCRIPT_CAMS, 0, 1, 3000, 1, 0);
                MenuOpened = false;
                Game.Player.Character.CanSwitchWeapons = true;
                LittleJacob.ToggleTrunk();
                LittleJacob.DriveAway();
                LittleJacob.DeleteBlip();
                LoadoutSaving.PerformSave(CurrentPed);
            } else if (!LittleJacob.Spawned)
            {
                GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Default, "Little Jacob", "Meetin", "my friend told me the police is after u. we cant meet like this, call me again when you lose them. Peace");
                LittleJacob.DeleteBlip();
                TimerStarted = false;
                JacobActive = false;
            }
            return;
        }

        if (LittleJacob.Spawned && !LittleJacob.Left && !LittleJacob.IsPlayerInArea())
        {
            menu.Pool.HideAll();
            cam.IsActive = false;
            cam.Delete();
            Function.Call(Hash.RENDER_SCRIPT_CAMS, 0, 1, 3000, 1, 0);
            MenuOpened = false;
            Game.Player.Character.CanSwitchWeapons = true;
            LittleJacob.DeleteBlip();
            LittleJacob.DriveAway();
            return;
        }

        if (MenuOpened && !LittleJacob.PlayerNearTrunk())
        {
            menu.Pool.HideAll();
            cam.IsActive = false;
            cam.Delete();
            Function.Call(Hash.RENDER_SCRIPT_CAMS, 0, 1, 3000, 1, 0);
            MenuOpened = false;
            Game.Player.Character.CanSwitchWeapons = true;
            LittleJacob.ToggleTrunk();
            LittleJacob.DeleteBlip();
            LittleJacob.DriveAway();
            return;
        }

        if (LittleJacob.Spawned && !LittleJacob.Left && LittleJacob.Jacob.IsDead)
        {
            menu.Pool.HideAll();
            cam.IsActive = false;
            cam.Delete();
            Function.Call(Hash.RENDER_SCRIPT_CAMS, 0, 1, 3000, 1, 0);
            MenuOpened = false;
            Game.Player.Character.CanSwitchWeapons = true;
            LittleJacob.ToggleTrunk();
            LittleJacob.DeleteBlip();
            LoadoutSaving.PerformSave(CurrentPed);
            LittleJacob.Terminate();
            return;
        }

        if (!LittleJacob.Spawned && LittleJacob.IsPlayerInArea())
        {
            LittleJacob.Spawn();
            if (TimerStarted)
            {
                TimerStarted = false;
            }
        } else if (!LittleJacob.Spawned && !LittleJacob.IsPlayerInArea())
        {
            if (!TimerStarted)
            {
                TimerStarted = true;
                TimerStart = Game.GameTime;
            } else
            {
                TimerCurrent = Game.GameTime;
                if (TimerCurrent - TimerStart >= 180000)
                {
                    GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Default, "Little Jacob", "Meetin", "where u at? if you aint comin u shoulda say something. das not cool");
                    TimerStarted = false;
                    JacobActive = false;
                    LittleJacob.DeleteBlip();
                    return;
                }
            }
        } else if (LittleJacob.Spawned && LittleJacob.PlayerNearTrunk() && !menu.Pool.AreAnyVisible && !MenuOpened && !LittleJacob.Left)
        {
            GTA.UI.Screen.ShowHelpTextThisFrame($"Press ~{OpenMenuKey}~ to purchase weapons", false);
        } else if (MenuOpened && !menu.Pool.AreAnyVisible)
        {
            cam.IsActive = false;
            cam.Delete();
            Function.Call(Hash.RENDER_SCRIPT_CAMS, 0, 1, 3000, 1, 0);
            MenuOpened = false;
            Game.Player.Character.CanSwitchWeapons = true;
            LittleJacob.ToggleTrunk();
            LittleJacob.DriveAway();
            LittleJacob.DeleteBlip();
            LoadoutSaving.PerformSave(CurrentPed);
        } else if (LittleJacob.Left && !LittleJacob.IsNearby())
        {
            LittleJacob.DeleteJacob();
            JacobActive = false;
        }
    }

    void ControlWatch(object o, EventArgs e)
    {
        if (!JacobActive)
        {
            return;
        }

        if (Function.Call<bool>(Hash.IS_CONTROL_JUST_RELEASED, 0, (int)OpenMenuKey))
        {
            if (LittleJacob.Spawned && !LittleJacob.Left && LittleJacob.PlayerNearTrunk() && !menu.Pool.AreAnyVisible && !MenuOpened)
            {
                cam = Function.Call<Camera>(Hash.CREATE_CAM, "DEFAULT_SCRIPTED_CAMERA", 0);
                cam.Position = new GTA.Math.Vector3(LittleJacob.Vehicle.RearPosition.X, LittleJacob.Vehicle.RearPosition.Y, LittleJacob.Vehicle.RearPosition.Z + 0.6f);
                cam.IsActive = true;
                cam.PointAt(LittleJacob.Vehicle.FrontPosition);
                Function.Call(Hash.RENDER_SCRIPT_CAMS, 1, 1, 3000, 1, 0);

                LittleJacob.ToggleTrunk();
                MenuOpened = true;
                Game.Player.Character.CanSwitchWeapons = false;
                menu.ShowMainMenu();
            }
        }
    }
}
