using System;
using GTA;
using LittleJacobMod.Interface;
using LittleJacobMod.Utils;
using LittleJacobMod.Saving;
using LittleJacobMod.Saving.Utils;
using GTA.Native;
using GTA.Math;

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
    public static PedHash JacobHash { get; private set; }
    public static VehicleHash JacobsCarHash { get; private set; }
    public static Controls OpenMenuKey { get; private set; }
    public static bool MissionFlag;
    public bool _processMenu;
    public static int PPID { get; private set; }

    public Main()
    {
        Migrator.Migrate();

        var settings = ScriptSettings.Load("scripts\\LittleJacobMod.ini");
        OpenMenuKey = settings.GetValue("Controls", "OpenMenu", Controls.INPUT_CONTEXT);
        SavingEnabled = settings.GetValue("Gameplay", "EnableSaving", true);
        JacobHash = settings.GetValue("Gameplay", "JacobModel", PedHash.Soucent03AMY);
        JacobsCarHash = settings.GetValue("Gameplay", "JacobsCarModel", VehicleHash.Virgo2);
        LoadoutSaving.WeaponsLoaded += LoadoutSaving_WeaponsLoaded;
        ifruit = new PhoneContact();
        menu = new Menu();

        if (Game.IsLoading)
        {
            Tick += WaitForGameLoad;
        }
        else
        {
            Initialize();
        }

        Menu.HelmetMenuChanged += (o, e) =>
        {
            MoveCamera(e ? 1 : 0);
        };

        Tick += ControlWatch;
        Tick += OnTick;
        Aborted += Main_Aborted;
    }

    private void LoadoutSaving_WeaponsLoaded(object sender, EventArgs e)
    {
        _processMenu = false;
        menu.ReloadOptions();
        _processMenu = true;
    }

    private void Main_Aborted(object sender, EventArgs e)
    {
        if (JacobActive)
        {
            if (cam != null)
            {
                cam.IsActive = false;
                cam.Delete();
                Function.Call(Hash.RENDER_SCRIPT_CAMS, 0, 1, 3000, 1, 0);
            }

            LittleJacob.DeleteBlip();
            Game.Player.Character.CanSwitchWeapons = true;
            Game.Player.Character.Task.ClearAll();
            LittleJacob.DeleteJacob();
        }
    }

    void WaitForGameLoad(object o, EventArgs e)
    {
        if (!Game.IsLoading)
        {
            Initialize(true);
        }
    }

    void Initialize(bool firstStart = false)
    {
        if (firstStart)
        {
            Tick -= WaitForGameLoad;
        }

        PPID = Function.Call<int>(Hash.PLAYER_PED_ID);
        MapperMain.CurrentPed = Function.Call<uint>(Hash.GET_ENTITY_MODEL, PPID);
        LoadoutSaving.PerformLoad(!firstStart);
        HelmetState.Load(!firstStart);
    }

    void OnTick(object o, EventArgs e)
    {
        PPID = Function.Call<int>(Hash.PLAYER_PED_ID);
        ifruit.Phone.Update();

        if (_processMenu)
        {
            menu.Pool.Process();
        }

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

                if (cam != null)
                {
                    cam.IsActive = false;
                    cam.Delete();
                    Function.Call(Hash.RENDER_SCRIPT_CAMS, 0, 1, 3000, 1, 0);
                }

                MenuOpened = false;
                Game.Player.Character.CanSwitchWeapons = true;
                LittleJacob.ToggleTrunk();
                LittleJacob.DriveAway();
                LittleJacob.DeleteBlip();
                LoadoutSaving.PerformSave(MapperMain.CurrentPed);
                HelmetState.Save();
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

            if (cam != null)
            {
                cam.IsActive = false;
                cam.Delete();
                Function.Call(Hash.RENDER_SCRIPT_CAMS, 0, 1, 3000, 1, 0);
            }

            MenuOpened = false;
            Game.Player.Character.CanSwitchWeapons = true;
            LittleJacob.DeleteBlip();
            LittleJacob.DriveAway();
            return;
        }

        if (MenuOpened && !LittleJacob.PlayerNearTrunk())
        {
            menu.Pool.HideAll();

            if (cam != null)
            {
                cam.IsActive = false;
                cam.Delete();
                Function.Call(Hash.RENDER_SCRIPT_CAMS, 0, 1, 3000, 1, 0);
            }

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

            if (cam != null)
            {
                cam.IsActive = false;
                cam.Delete();
                Function.Call(Hash.RENDER_SCRIPT_CAMS, 0, 1, 3000, 1, 0);
            }

            MenuOpened = false;
            Game.Player.Character.CanSwitchWeapons = true;
            LittleJacob.ToggleTrunk();
            LittleJacob.DeleteBlip();
            LoadoutSaving.PerformSave(MapperMain.CurrentPed);
            HelmetState.Save();
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
            if (cam != null)
            {
                cam.IsActive = false;
                cam.Delete();
                Function.Call(Hash.RENDER_SCRIPT_CAMS, 0, 1, 3000, 1, 0);
            }

            MenuOpened = false;
            Game.Player.Character.CanSwitchWeapons = true;
            LittleJacob.ToggleTrunk();
            LittleJacob.DriveAway();
            LittleJacob.DeleteBlip();
            LoadoutSaving.PerformSave(MapperMain.CurrentPed);
            HelmetState.Save();
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
                MoveCamera(0);
                cam.IsActive = true;
                Function.Call(Hash.RENDER_SCRIPT_CAMS, 1, 1, 3000, 1, 0);

                LittleJacob.ToggleTrunk();
                MenuOpened = true;
                Game.Player.Character.CanSwitchWeapons = false;
                menu.ShowMainMenu();
            }
        }
    }

    public static bool IsMainCharacter()
    {
        return Function.Call<bool>(Hash.IS_PED_MODEL, PPID, PedHash.Michael) || Function.Call<bool>(Hash.IS_PED_MODEL, PPID, PedHash.Franklin) || Function.Call<bool>(Hash.IS_PED_MODEL, PPID, PedHash.Trevor);
    }

    public static int IsMPPed()
    {
        if (Function.Call<bool>(Hash.IS_PED_MODEL, PPID, PedHash.FreemodeMale01))
        {
            return 0;
        }

        if (Function.Call<bool>(Hash.IS_PED_MODEL, PPID, PedHash.FreemodeFemale01))
        {
            return 1;
        }

        return -1;
    }

    void MoveCamera(int posIndex)
    {
        if (posIndex == 0)
        {
            cam.Detach();
            cam.Position = new Vector3(LittleJacob.Vehicle.RearPosition.X, LittleJacob.Vehicle.RearPosition.Y, LittleJacob.Vehicle.RearPosition.Z + 0.6f);
            cam.PointAt(LittleJacob.Vehicle.FrontPosition);
        } 
        else
        {
            var rearPos = Game.Player.Character.RearPosition;
            cam.AttachTo(Game.Player.Character.Bones[Bone.SkelHead], new Vector3(0.25f, 1, 0.7f));
            cam.PointAt(new Vector3(rearPos.X, rearPos.Y, rearPos.Z + 0.7f));
        }
    }
}
