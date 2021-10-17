using System;
using System.Collections.Generic;
using GTA;
using System.Windows.Forms;
using LittleJacobMod.Loading;
using LittleJacobMod.Interface;
using LittleJacobMod.Utils;

public class Main : Script
{
    string openMenuControl;
    Keys openMenuKey;
    PhoneContact ifruit;
    bool menuOpened;
    LittleJacobMod.Interface.Menu menu;
    public static bool jacobActive;
    public static LittleJacob LittleJacob { get; set; }

    public Main()
    {
        var settings = ScriptSettings.Load("scripts\\LittleJacobMod.ini");
        openMenuControl = settings.GetValue("Controls", "OpenMenu", "E");

        if (!Enum.TryParse(openMenuControl, out openMenuKey))
        {
            openMenuControl = "E";
            openMenuKey = Keys.E;
            GTA.UI.Notification.Show("Little Jacob mod: ~r~Failed~w~ to load the 'OpenMenu' key from the ini file. Using default value 'E'");
        }

        ifruit = new PhoneContact();
        menu = new LittleJacobMod.Interface.Menu();

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
        if (jacobActive)
        {
            LittleJacob.DeleteBlip();
            Game.Player.Character.CanSwitchWeapons = true;
            Game.Player.Character.Task.ClearAll();
            LittleJacob.DeleteJacob();
        }
    }

    void OnTick(object o, EventArgs e)
    {
        if (!jacobActive)
        {
            return;
        }

        if (!LittleJacob.Spawned && LittleJacob.IsPlayerInArea())
        {
            LittleJacob.Spawn();
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
            Game.Player.Character.Task.ClearAll();
            Game.Player.Character.CanSwitchWeapons = true;
            LittleJacob.ToggleTrunk();
            LittleJacob.DriveAway();
            LittleJacob.DeleteBlip();
        } else if (LittleJacob.Left && !LittleJacob.IsNearby())
        {
            LittleJacob.DeleteJacob();
            jacobActive = false;
        }
    }

    void KeyboardControls(object o, KeyEventArgs e)
    {
        if (!jacobActive)
        {
            return;
        }

        if (e.KeyCode == openMenuKey)
        {
            if (LittleJacob.Spawned && !LittleJacob.Left && LittleJacob.PlayerNearTrunk() && !menu.Pool.AreAnyVisible && !menuOpened)
            {
                LittleJacob.ToggleTrunk();
                menuOpened = true;
                Game.Player.Character.Task.StandStill(1800000000);
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


    }
}
