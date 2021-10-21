using System;
using GTA;
using System.Windows.Forms;
using LittleJacobMod.Interface;
using LittleJacobMod.Utils;
using LittleJacobMod.Saving;
using LittleJacobMod.Saving.Utils;
using GTA.Native;
using System.Collections.Generic;

public class Main : Script
{
    string openMenuControl;
    Keys openMenuKey;
    PhoneContact ifruit;
    bool menuOpened, saveTriggered;
    static bool timerStarted;
    LittleJacobMod.Interface.Menu menu;
    int timerCurrent, timerStart;
    PedHash currentPed;
    public static bool JacobActive { get; set; }
    public static bool SavingEnabled { get; private set; }
    public static LittleJacob LittleJacob { get; set; }
    public static bool TimerStarted { get { return timerStarted; } set { timerStarted = value; } }
    /*Dictionary<string, string> animsDic = new Dictionary<string, string>()
    {
        { "anim@mp_bedmid@left_var_01",  }
    };*/

    public Main()
    {
        Migrator.Migrate();

        var settings = ScriptSettings.Load("scripts\\LittleJacobMod.ini");
        openMenuControl = settings.GetValue("Controls", "OpenMenu", "E");
        SavingEnabled = settings.GetValue("Gameplay", "EnableSaving", true);

        if (!Enum.TryParse(openMenuControl, out openMenuKey))
        {
            openMenuControl = "E";
            openMenuKey = Keys.E;
            GTA.UI.Notification.Show("Little Jacob mod: ~r~Failed~w~ to load the 'OpenMenu' key from the ini file. Using default value 'E'");
        }

        ifruit = new PhoneContact();
        menu = new LittleJacobMod.Interface.Menu();

        if (SavingEnabled)
        {
            if (Game.IsLoading)
            {
                Tick += WaitForGameLoad;
            }
            else
            {
                currentPed = (PedHash)Game.Player.Character.Model.Hash;
                //LoadoutSaving.PerformLoad();
                Tick += ModelWatcher;
            }
            Tick += AutoSaveWatch;
            Tick += WeaponUse;
        }

        KeyUp += KeyboardControls;
        Tick += GamepadControls;
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
            if (!saveTriggered)
            {
                LoadoutSaving.PerformSave(currentPed);
                saveTriggered = true;
            }
            return;
        }

        if (saveTriggered)
        {
            saveTriggered = false;
        }
    }

    void WeaponUse(object o, EventArgs e)
    {
        if (Game.Player.Character.IsShooting)
        {
            var currentWeapon = Game.Player.Character.Weapons.Current;
            if (LoadoutSaving.IsWeaponInStore(currentWeapon.Hash))
            {
                LoadoutSaving.SetAmmo(currentWeapon.Hash, currentWeapon.Ammo);
            }
        }
    }

    void WaitForGameLoad(object o, EventArgs e)
    {
        if (Game.IsLoading)
        {
            return;
        }
        currentPed = (PedHash)Game.Player.Character.Model.Hash;
        LoadoutSaving.PerformLoad();
        Tick -= WaitForGameLoad;
        Tick += ModelWatcher;
    }

    void ModelWatcher(object o, EventArgs e)
    {
        if (currentPed == (PedHash)Game.Player.Character.Model.Hash)
        {
            return;
        }

        LoadoutSaving.PerformSave(currentPed);

        currentPed = (PedHash)Game.Player.Character.Model.Hash;
        LoadoutSaving.RemoveWeapons(!LoadoutSaving.IsPedMainPlayer(Game.Player.Character));
        LoadoutSaving.PerformLoad();
    }

    void OnTick(object o, EventArgs e)
    {
        if (!JacobActive)
        {
            return;
        }

        if (Game.Player.WantedLevel > 0)
        {
            if (LittleJacob.Spawned && !LittleJacob.Left)
            {
                menu.Pool.HideAll();
                menuOpened = false;
                Game.Player.Character.CanSwitchWeapons = true;
                LittleJacob.ToggleTrunk();
                LittleJacob.DriveAway();
                LittleJacob.DeleteBlip();
                Game.DoAutoSave();
                LittleJacob.DeleteJacob();
            } else if (!LittleJacob.Spawned)
            {
                GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Default, "Little Jacob", "Meetin", "my friend told me the police is after u. we cant meet like this, call me again when you lose them. Peace");
                LittleJacob.DeleteBlip();
                timerStarted = false;
                JacobActive = false;
            }
            return;
        }

        if (LittleJacob.Spawned && !LittleJacob.Left && LittleJacob.Jacob.IsDead)
        {
            menu.Pool.HideAll();
            menuOpened = false;
            Game.Player.Character.CanSwitchWeapons = true;
            LittleJacob.ToggleTrunk();
            LittleJacob.DeleteBlip();
            Game.DoAutoSave();
            LittleJacob.DeleteJacob();
            return;
        }

        if (!LittleJacob.Spawned && LittleJacob.IsPlayerInArea())
        {
            LittleJacob.Spawn();
            if (timerStarted)
            {
                timerStarted = false;
            }
        } else if (!LittleJacob.Spawned && !LittleJacob.IsPlayerInArea())
        {
            if (!timerStarted)
            {
                timerStarted = true;
                timerStart = Game.GameTime;
            } else
            {
                timerCurrent = Game.GameTime;
                if (timerCurrent - timerStart >= 180000)
                {
                    GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Default, "Little Jacob", "Meetin", "where u at? if you aint comin u shoulda say something. das not cool");
                    timerStarted = false;
                    JacobActive = false;
                    LittleJacob.DeleteBlip();
                    return;
                }
            }
        } else if (LittleJacob.Spawned && LittleJacob.PlayerNearTrunk() && !menu.Pool.AreAnyVisible && !menuOpened && !LittleJacob.Left)
        {
            if (Game.LastInputMethod == InputMethod.MouseAndKeyboard)
            {
                GTA.UI.Screen.ShowHelpTextThisFrame($"Press ~g~{openMenuControl}~w~ to purchase weapons", false);
            } else
            {
                GTA.UI.Screen.ShowHelpTextThisFrame("Press DPad Left to purchase weapons", false);
            }
        } else if (menuOpened && !menu.Pool.AreAnyVisible)
        {
            menuOpened = false;
            Game.Player.Character.CanSwitchWeapons = true;
            LittleJacob.ToggleTrunk();
            LittleJacob.DriveAway();
            LittleJacob.DeleteBlip();
            Game.DoAutoSave();
        } else if (LittleJacob.Left && !LittleJacob.IsNearby())
        {
            LittleJacob.DeleteJacob();
            JacobActive = false;
        }
    }

    void KeyboardControls(object o, KeyEventArgs e)
    {
        if (!JacobActive)
        {
            return;
        }

        if (e.KeyCode == openMenuKey)
        {
            if (LittleJacob.Spawned && !LittleJacob.Left && LittleJacob.PlayerNearTrunk() && !menu.Pool.AreAnyVisible && !menuOpened)
            {
                LittleJacob.ToggleTrunk();
                menuOpened = true;
                Game.Player.Character.CanSwitchWeapons = false;
                menu.ShowMainMenu();
            }
        }
    }

    void GamepadControls(object o, EventArgs e)
    {
        if (Game.LastInputMethod == InputMethod.MouseAndKeyboard)
        {
            return;
        }

        if (!JacobActive)
        {
            return;
        }

        if (Game.IsControlJustReleased(GTA.Control.ScriptPadLeft))
        {
            if (LittleJacob.Spawned && !LittleJacob.Left && LittleJacob.PlayerNearTrunk() && !menu.Pool.AreAnyVisible && !menuOpened)
            {
                LittleJacob.ToggleTrunk();
                menuOpened = true;
                Game.Player.Character.CanSwitchWeapons = false;
                menu.ShowMainMenu();
            }
        }
    }
}
