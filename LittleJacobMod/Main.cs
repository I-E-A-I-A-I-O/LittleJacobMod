using System;
using GTA;
using LittleJacobMod.Interface;
using LittleJacobMod.Utils;
using LittleJacobMod.Saving;
using GTA.Native;
using GTA.Math;

public class Main : Script
{
    PhoneContact ifruit;
    public Menu menu;
    Camera _trunkCam;
    Camera _faceCam;
    public static CallMenu CallMenu { get; private set; }
    public static Camera Camera { get; private set; }
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
    public bool _processMenu = true;
    public static int PPID { get; set; }
    public static bool MenuCreated { get; private set; } = false;
    int _scaleform;
    bool _scaleformFading;
    bool _scaleformRequested;
    int _scaleformST;
    static string _scaleformTitle;
    static string _scaleformSubtitle;
    public static bool ScaleformActive { get; private set; }
    static int _scaleformType;

    public Main()
    {
        var settings = ScriptSettings.Load("scripts\\LittleJacobMod.ini");
        OpenMenuKey = settings.GetValue("Controls", "OpenMenu", Controls.INPUT_CONTEXT);
        SavingEnabled = settings.GetValue("Gameplay", "EnableSaving", true);
        JacobHash = settings.GetValue("Gameplay", "JacobModel", PedHash.Soucent03AMY);
        JacobsCarHash = settings.GetValue("Gameplay", "JacobsCarModel", VehicleHash.Virgo2);
        LoadoutSaving.WeaponsLoaded += LoadoutSaving_WeaponsLoaded;
        ifruit = new PhoneContact();
        menu = new Menu();
        MenuCreated = true;
        CallMenu = new CallMenu();

        Menu.HelmetMenuChanged += (o, e) =>
        {
            MoveCamera(e ? 1 : 0);
        };

        Tick += ControlWatch;
        Tick += OnTick;
        Tick += MenuTick;
        Tick += ScaleformTick;
        Aborted += Main_Aborted;
    }

    void SetScaleFormText(string title, string description)
    {
        Function.Call(Hash.BEGIN_SCALEFORM_MOVIE_METHOD, _scaleform, "SHOW_SHARD_CENTERED_TOP_MP_MESSAGE");
        Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_TEXTURE_NAME_STRING, title);
        Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_TEXTURE_NAME_STRING, description);
        Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_INT, 5);
        Function.Call(Hash.END_SCALEFORM_MOVIE_METHOD);
    }

    void FadeOutScaleform()
    {
        Function.Call(Hash.CALL_SCALEFORM_MOVIE_METHOD, _scaleform, "SHARD_ANIM_OUT");
        Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_INT, 5);
        Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_INT, 3000);
        Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_BOOL, true);
        Function.Call(Hash.END_SCALEFORM_MOVIE_METHOD);
    }

    int RequestScaleform()
    {
        _scaleform = Function.Call<int>(Hash.REQUEST_SCALEFORM_MOVIE, "mp_big_message_freemode");

        while (!Function.Call<bool>(Hash.HAS_SCALEFORM_MOVIE_LOADED, _scaleform))
            Wait(1);

        return _scaleform;
    }

    void FreeScaleform()
    {
        int s = _scaleform;
        unsafe
        {
            Function.Call(Hash.SET_SCALEFORM_MOVIE_AS_NO_LONGER_NEEDED, &s);
        }
    }

    public static void ShowScaleform(string title, string subtitle, int type)
    {
        if (ScaleformActive)
            return;

        _scaleformTitle = title;
        _scaleformSubtitle = subtitle;
        _scaleformType = type;
        ScaleformActive = true;
    }

    void ScaleformTick(object o, EventArgs e)
    {
        if (!ScaleformActive)
            return;

        if (!_scaleformRequested)
        {
            RequestScaleform();
            SetScaleFormText(_scaleformTitle, _scaleformSubtitle);
            _scaleformRequested = true;
            _scaleformST = Game.GameTime;
        }

        Function.Call(Hash.DRAW_SCALEFORM_MOVIE_FULLSCREEN, _scaleform, 232, 207, 20, 255);

        if (Game.GameTime - _scaleformST >= 8000 && !_scaleformFading)
        {
            FadeOutScaleform();
            _scaleformFading = true;
        }
        else if (Game.GameTime - _scaleformST >= 12000)
        {
            ScaleformActive = false;
            _scaleformFading = false;
            _scaleformRequested = false;
            FreeScaleform();
        }
    }

    private void LoadoutSaving_WeaponsLoaded(object sender, EventArgs e)
    {
        _processMenu = false;
        menu.ReloadOptions();
        _processMenu = true;
    }

    void DeleteCameras()
    {
        if (_trunkCam != null)
        {
            _trunkCam.IsActive = false;
            _trunkCam.Delete();
        }

        if (_faceCam != null)
        {
            _faceCam.IsActive = false;
            _faceCam.Delete();
        }

        if (Camera != null)
        {
            Camera.IsActive = false;
            Camera.Delete();
        }

        Function.Call(Hash.RENDER_SCRIPT_CAMS, 0, 1, 3000, 1, 0);
    }

    private void Main_Aborted(object sender, EventArgs e)
    {
        if (JacobActive)
        {
            DeleteCameras();
            LittleJacob.DeleteBlip();
            Game.Player.Character.CanSwitchWeapons = true;
            Game.Player.Character.Task.ClearAll();
            LittleJacob.DeleteJacob();
        }
    }

    void MenuTick(object o, EventArgs e)
    {
        ifruit.Phone.Update();
        CallMenu.Pool.Process();
        CallMenu.ProcessTimer();

        if (_processMenu)
        {
            menu.Pool.Process();
        }
    }

    void OnTick(object o, EventArgs e)
    {
        PPID = Function.Call<int>(Hash.PLAYER_PED_ID);

        if (!JacobActive)
        {
            return;
        }

        //LittleJacob.ProcessVoice();

        if (LittleJacob.Spawned && !LittleJacob.Left)
        {
            if (Camera != null && Camera.Handle != 0 && Camera.IsActive)
            {
                Function.Call(Hash.DRAW_LIGHT_WITH_RANGE, LittleJacob.Vehicle.RearPosition.X + (_trunkCam.Direction.X / 2), LittleJacob.Vehicle.RearPosition.Y + (_trunkCam.Direction.Y / 2), LittleJacob.Vehicle.RearPosition.Z + 0.3f, 255, 255, 255, 1.5f, 0.5f);
            }
        }

        if (Game.Player.WantedLevel > 0)
        {
            if (LittleJacob.Spawned && !LittleJacob.Left)
            {
                menu.Pool.HideAll();
                DeleteCameras();
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
            DeleteCameras();
            MenuOpened = false;
            Game.Player.Character.CanSwitchWeapons = true;
            LittleJacob.DeleteBlip();
            LittleJacob.DriveAway();
            return;
        }

        if (MenuOpened && !LittleJacob.PlayerNearTrunk())
        {
            menu.Pool.HideAll();
            DeleteCameras();
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
            DeleteCameras();
            MenuOpened = false;
            Game.Player.Character.CanSwitchWeapons = true;
            LittleJacob.ToggleTrunk();
            LittleJacob.DeleteBlip();
            LoadoutSaving.PerformSave(MapperMain.CurrentPed);
            HelmetState.Save();

            if (Function.Call<bool>(Hash.IS_PED_MODEL, PPID, PedHash.Trevor))
            {
                if (LittleJacob.Jacob.Killer.Handle == PPID && !MissionSaving.TUnlocked)
                {
                    GTA.UI.Notification.Show("~g~Little Jacob mod~w~: 20% discount unlocked for Trevor");
                    MissionSaving.TUnlocked = true;
                    MissionSaving.Save();
                }
            }

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
            DeleteCameras();
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

        if (Function.Call<bool>(Hash.IS_CONTROL_JUST_PRESSED, 0, (int)OpenMenuKey))
        {
            if (LittleJacob.Spawned && !LittleJacob.Left && LittleJacob.PlayerNearTrunk() && !menu.Pool.AreAnyVisible && !MenuOpened)
            {
                Camera = Function.Call<Camera>(Hash.CREATE_CAM, "DEFAULT_SCRIPTED_CAMERA", 0);
                _trunkCam = Function.Call<Camera>(Hash.CREATE_CAM, "DEFAULT_SCRIPTED_CAMERA", 0);
                _faceCam = Function.Call<Camera>(Hash.CREATE_CAM, "DEFAULT_SCRIPTED_CAMERA", 0);
                Vector3 rearPos = Game.Player.Character.RearPosition;
                _faceCam.AttachTo(Game.Player.Character.Bones[Bone.SkelHead], new Vector3(0.25f, 1, 0.7f));
                _faceCam.PointAt(new Vector3(rearPos.X, rearPos.Y, rearPos.Z + 0.7f));
                _trunkCam.Position = new Vector3(LittleJacob.Vehicle.RearPosition.X, LittleJacob.Vehicle.RearPosition.Y, LittleJacob.Vehicle.RearPosition.Z + 0.6f);
                _trunkCam.PointAt(LittleJacob.Vehicle.FrontPosition);
                Camera.Position = _trunkCam.Position;
                Camera.PointAt(LittleJacob.Vehicle.FrontPosition);
                _faceCam.IsActive = false;
                _trunkCam.IsActive = false;
                MoveCamera(0);
                Camera.IsActive = true;
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
            Camera.InterpTo(_trunkCam, 2000, 1, 1);
        } 
        else
        {
            Camera.InterpTo(_faceCam, 2000, 1, 1);
        }
    }
}
