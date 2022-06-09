using System;
using GTA;
using LittleJacobMod.Interface;
using LittleJacobMod.Utils;
using LittleJacobMod.Saving;
using GTA.Native;
using GTA.Math;

public class Main : Script
{
    private readonly PhoneContact _ifruit;
    private readonly Menu _menu;
    private Camera _trunkCam;
    private Camera _faceCam;
    public static CallMenu CallMenu { get; private set; }
    public static Camera Camera { get; private set; }
    public static bool JacobActive { get; set; }
    public static LittleJacob LittleJacob { get; set; }
    public static bool TimerStarted { get; set; }
    private static int TimerStart { get; set; }
    private static int TimerCurrent { get; set; }
    public static bool MenuOpened { get; private set; }
    public static PedHash JacobHash { get; private set; }
    public static VehicleHash JacobsCarHash { get; private set; }
    public static Controls OpenMenuKey { get; private set; }
    public static bool MissionFlag;
    private bool _processMenu = true;
    public static int PPID { get; set; }
    public static bool MenuCreated { get; private set; }
    private int _scaleform;
    private bool _scaleformFading;
    private static bool _scaleformRequested;
    private int _scaleformSt;
    private static string _scaleformTitle;
    private static string _scaleformSubtitle;
    public static bool ScaleformActive { get; private set; }
    private static int _scaleformType;
    private static uint _weaponHash;

    public Main()
    {
        var settings = ScriptSettings.Load("scripts\\LittleJacobMod.ini");
        OpenMenuKey = settings.GetValue("Controls", "OpenMenu", Controls.INPUT_CONTEXT);
        JacobHash = settings.GetValue("Gameplay", "JacobModel", PedHash.Soucent03AMY);
        JacobsCarHash = settings.GetValue("Gameplay", "JacobsCarModel", VehicleHash.Virgo2);
        LoadoutSaving.WeaponsLoaded += LoadoutSaving_WeaponsLoaded;
        _ifruit = new PhoneContact();
        _menu = new Menu();
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

    private void SetScaleformWeaponText(string title, string weaponName, uint weaponHash)
    {
        Function.Call(Hash.BEGIN_SCALEFORM_MOVIE_METHOD, _scaleform, "SHOW_WEAPON_PURCHASED");
        Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_TEXTURE_NAME_STRING, title);
        Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_TEXTURE_NAME_STRING, weaponName);
        Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_INT, weaponHash);
        Function.Call(Hash.END_SCALEFORM_MOVIE_METHOD);
    }
    
    private void SetScaleFormText(string title, string description)
    {
        Function.Call(Hash.BEGIN_SCALEFORM_MOVIE_METHOD, _scaleform, "SHOW_SHARD_CENTERED_TOP_MP_MESSAGE");
        Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_TEXTURE_NAME_STRING, title);
        Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_TEXTURE_NAME_STRING, description);
        Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_INT, 5);
        Function.Call(Hash.END_SCALEFORM_MOVIE_METHOD);
    }

    private void FadeOutScaleform()
    {
        Function.Call(Hash.CALL_SCALEFORM_MOVIE_METHOD, _scaleform, "SHARD_ANIM_OUT");
        Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_INT, 5);
        Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_INT, 2000);
        Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_BOOL, true);
        Function.Call(Hash.END_SCALEFORM_MOVIE_METHOD);
    }

    private void RequestScaleform()
    {
        _scaleform = Function.Call<int>(Hash.REQUEST_SCALEFORM_MOVIE, "mp_big_message_freemode");

        while (!Function.Call<bool>(Hash.HAS_SCALEFORM_MOVIE_LOADED, _scaleform))
            Wait(1);
    }

    private void FreeScaleform()
    {
        var s = _scaleform;
        unsafe
        {
            Function.Call(Hash.SET_SCALEFORM_MOVIE_AS_NO_LONGER_NEEDED, &s);
        }
    }

    public static void ShowScaleform(string title, string subtitle, int type, uint weaponHash = 0)
    {
        if (ScaleformActive && _scaleformType != 1)
            return;

        _scaleformTitle = title;
        _scaleformSubtitle = subtitle;
        _scaleformType = type;
        _weaponHash = weaponHash;
        _scaleformRequested = false;
        ScaleformActive = true;
    }

    private void ScaleformTick(object o, EventArgs e)
    {
        if (!ScaleformActive)
            return;

        if (!_scaleformRequested)
        {
            switch (_scaleformType)
            {
                case 0:
                    RequestScaleform();
                    SetScaleFormText(_scaleformTitle, _scaleformSubtitle);
                    break;
                case 1:
                    SetScaleformWeaponText(_scaleformTitle, _scaleformSubtitle, _weaponHash);
                    break;
            }
            _scaleformRequested = true;
            _scaleformSt = Game.GameTime;
        }

        Function.Call(Hash.DRAW_SCALEFORM_MOVIE_FULLSCREEN, _scaleform, 232, 207, 20, 255);

        switch (Game.GameTime - _scaleformSt)
        {
            case >= 4000 when !_scaleformFading:
                FadeOutScaleform();
                _scaleformFading = true;
                break;
            case >= 6000:
                ScaleformActive = false;
                _scaleformFading = false;
                _scaleformRequested = false;
                if (_scaleformType != 1)
                    FreeScaleform();
                break;
        }
    }

    private void LoadoutSaving_WeaponsLoaded(object sender, EventArgs e)
    {
        _processMenu = false;
        _menu.ReloadOptions();
        _processMenu = true;
    }

    private void DeleteCameras()
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
        if (!JacobActive) return;
        DeleteCameras();
        LittleJacob.DeleteBlip();
        Game.Player.Character.CanSwitchWeapons = true;
        Game.Player.Character.Task.ClearAll();
        LittleJacob.DeleteJacob();
    }

    private void MenuTick(object o, EventArgs e)
    {
        _ifruit.Phone.Update();
        CallMenu.Pool.Process();
        CallMenu.ProcessTimer();

        if (_processMenu)
        {
            _menu.Pool.Process();
        }
    }

    private void OnTick(object o, EventArgs e)
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
            switch (LittleJacob.Spawned)
            {
                case true when !LittleJacob.Left:
                    _menu.Pool.HideAll();
                    DeleteCameras();
                    MenuOpened = false;
                    Game.Player.Character.CanSwitchWeapons = true;
                    LittleJacob.ToggleTrunk();
                    LittleJacob.DriveAway();
                    LittleJacob.DeleteBlip();
                    FreeScaleform();
                    LoadoutSaving.PerformSave(MapperMain.CurrentPed);
                    HelmetState.Save();
                    break;
                case false:
                    GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Default, "Little Jacob", "Meetin", "my friend told me the police is after u. we cant meet like this, call me again when you lose them. Peace");
                    LittleJacob.DeleteBlip();
                    TimerStarted = false;
                    JacobActive = false;
                    break;
            }

            return;
        }

        if (LittleJacob.Spawned && !LittleJacob.Left && !LittleJacob.IsPlayerInArea())
        {
            _menu.Pool.HideAll();
            DeleteCameras();
            MenuOpened = false;
            Game.Player.Character.CanSwitchWeapons = true;
            LittleJacob.DeleteBlip();
            LittleJacob.DriveAway();
            FreeScaleform();
            return;
        }

        if (MenuOpened && !LittleJacob.PlayerNearTrunk())
        {
            _menu.Pool.HideAll();
            DeleteCameras();
            MenuOpened = false;
            Game.Player.Character.CanSwitchWeapons = true;
            LittleJacob.ToggleTrunk();
            LittleJacob.DeleteBlip();
            FreeScaleform();
            LittleJacob.DriveAway();
            return;
        }

        switch (LittleJacob.Spawned)
        {
            case true when !LittleJacob.Left && LittleJacob.Jacob.IsDead:
            {
                _menu.Pool.HideAll();
                DeleteCameras();
                MenuOpened = false;
                Game.Player.Character.CanSwitchWeapons = true;
                LittleJacob.ToggleTrunk();
                LittleJacob.DeleteBlip();
                FreeScaleform();
                LoadoutSaving.PerformSave(MapperMain.CurrentPed);
                HelmetState.Save();

                if (Function.Call<bool>(Hash.IS_PED_MODEL, PPID, (uint)PedHash.Trevor))
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
            case false when LittleJacob.IsPlayerInArea():
            {
                LittleJacob.Spawn();
                RequestScaleform();
                if (TimerStarted)
                {
                    TimerStarted = false;
                }

                break;
            }
            case false when !LittleJacob.IsPlayerInArea():
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
                    }
                }

                break;
            }
            case true when LittleJacob.PlayerNearTrunk() && !_menu.Pool.AreAnyVisible && !MenuOpened && !LittleJacob.Left:
                GTA.UI.Screen.ShowHelpTextThisFrame($"Press ~{OpenMenuKey}~ to purchase weapons", false);
                break;
            default:
            {
                if (MenuOpened && !_menu.Pool.AreAnyVisible)
                {
                    DeleteCameras();
                    MenuOpened = false;
                    Game.Player.Character.CanSwitchWeapons = true;
                    LittleJacob.ToggleTrunk();
                    LittleJacob.DriveAway();
                    LittleJacob.DeleteBlip();
                    FreeScaleform();
                    LoadoutSaving.PerformSave(MapperMain.CurrentPed);
                    HelmetState.Save();
                } else if (LittleJacob.Left && !LittleJacob.IsNearby())
                {
                    LittleJacob.DeleteJacob();
                    JacobActive = false;
                }

                break;
            }
        }
    }

    private void ControlWatch(object o, EventArgs e)
    {
        if (!JacobActive)
        {
            return;
        }

        if (!Function.Call<bool>(Hash.IS_CONTROL_JUST_PRESSED, 0, (int) OpenMenuKey)) return;
        if (!LittleJacob.Spawned || LittleJacob.Left || !LittleJacob.PlayerNearTrunk() || _menu.Pool.AreAnyVisible ||
            MenuOpened) return;
        Camera = Function.Call<Camera>(Hash.CREATE_CAM, "DEFAULT_SCRIPTED_CAMERA", 0);
        _trunkCam = Function.Call<Camera>(Hash.CREATE_CAM, "DEFAULT_SCRIPTED_CAMERA", 0);
        _faceCam = Function.Call<Camera>(Hash.CREATE_CAM, "DEFAULT_SCRIPTED_CAMERA", 0);
        var rearPos = Game.Player.Character.RearPosition;
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
        _menu.ShowMainMenu();
    }

    public static bool IsMainCharacter()
    {
        return Function.Call<bool>(Hash.IS_PED_MODEL, PPID, (uint)PedHash.Michael) || Function.Call<bool>(Hash.IS_PED_MODEL, PPID, (uint)PedHash.Franklin) || Function.Call<bool>(Hash.IS_PED_MODEL, PPID, (uint)PedHash.Trevor);
    }

    public static int IsMPped()
    {
        if (Function.Call<bool>(Hash.IS_PED_MODEL, PPID, (uint)PedHash.FreemodeMale01))
        {
            return 0;
        }

        if (Function.Call<bool>(Hash.IS_PED_MODEL, PPID, (uint)PedHash.FreemodeFemale01))
        {
            return 1;
        }

        return -1;
    }

    private void MoveCamera(int posIndex)
    {
        Camera.InterpTo(posIndex == 0 ? _trunkCam : _faceCam, 2000, 1, 1);
    }
}
