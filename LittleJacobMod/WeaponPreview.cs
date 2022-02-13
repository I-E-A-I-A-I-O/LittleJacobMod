using System;
using GTA;
using GTA.Math;
using GTA.Native;
using LittleJacobMod.Interface;
using LittleJacobMod.Saving;
using LittleJacobMod.Utils;

namespace LittleJacobMod;

internal class WeaponPreview : Script
{
    private Prop _weaponHandle;
    private bool _doComponentReload;
    private bool _doObjectSpawn;
    private bool _doComponentChange;
    private bool _compFromStorage;
    private uint _hash;
    private uint _new = (uint)WeaponComponentHash.Invalid;
    private float _currentTime;
    private float _oldTime;
    private Vector3 _rotation;
    private string _skipIndex;
    private readonly Controls _yawRight;
    private readonly Controls _yawLeft;
    private readonly Controls _pitchUp;
    private readonly Controls _pitchDown;

    public WeaponPreview()
    {
        ScriptSettings settings = ScriptSettings.Load("scripts\\LittleJacobMod.ini");
        _yawRight = settings.GetValue("Controls", "RotateDown", Controls.INPUT_VEH_FLY_ROLL_RIGHT_ONLY);
        _yawLeft = settings.GetValue("Controls", "RotateUp", Controls.INPUT_VEH_FLY_ROLL_LEFT_ONLY);
        _pitchUp = settings.GetValue("Controls", "RotateBack", Controls.INPUT_VEH_FLY_PITCH_UP_ONLY);
        _pitchDown = settings.GetValue("Controls", "RotateFront", Controls.INPUT_VEH_FLY_PITCH_DOWN_ONLY);
        Menu.ComponentSelected += Menu_ComponentSelected;
        Menu.SpawnWeaponObject += Menu_SpawnWeaponObject;
        Menu.CamoColorChanged += Menu_CamoColorChanged;
        Menu.TintChanged += Menu_TintChanged;

        LittleJacob.TrunkStateChanged += LittleJacob_TrunkStateChanged;

        Tick += WeaponPreview_Tick;
    }

    private void WeaponPreview_Tick(object sender, EventArgs e)
    {
        _oldTime = _currentTime;
        _currentTime = Game.GameTime;
        float deltaTime = _currentTime - _oldTime;

        if (_doComponentReload)
        {
            SpawnWeaponObject(_hash, false, 0);
            _doComponentReload = false;
            _compFromStorage = false;
        }

        if (_doObjectSpawn)
        {
            SpawnWeaponObject(_hash, false, 0);
            _doObjectSpawn = false;
            _compFromStorage = false;
        }

        if (_doComponentChange)
        {
            GiveWeaponComponentToObject(_new, false);

            if (_new != (uint)WeaponComponentHash.Invalid)
            {
                var slide = TintsAndCamos.ReturnSlide(_new);

                if (slide != (uint)WeaponComponentHash.Invalid)
                {
                    GiveWeaponComponentToObject(slide, true);
                }
            }

            _doComponentChange = false;
        }

        if (_weaponHandle == null || _weaponHandle.Handle == 0)
            return;

        if (Game.LastInputMethod != InputMethod.MouseAndKeyboard)
            return;

        if (Function.Call<bool>(Hash.IS_CONTROL_PRESSED, 0, (int)_yawLeft))
        {
            float y = _weaponHandle.Rotation.Y;
            y -= 1.0f * 0.05f * deltaTime;
            _rotation = new Vector3(_weaponHandle.Rotation.X, y, _weaponHandle.Rotation.Z);
            _weaponHandle.Rotation = _rotation;
        }

        if (Function.Call<bool>(Hash.IS_CONTROL_PRESSED, 0, (int)_yawRight))
        {
            float y = _weaponHandle.Rotation.Y;
            y += 1.0f * 0.05f * deltaTime;
            _rotation = new Vector3(_weaponHandle.Rotation.X, y, _weaponHandle.Rotation.Z);
            _weaponHandle.Rotation = _rotation;
        }

        if (Function.Call<bool>(Hash.IS_CONTROL_PRESSED, 0, (int)_pitchUp))
        {
            float x = _weaponHandle.Rotation.X;
            x -= 1.0f * 0.05f * deltaTime;
            _rotation = new Vector3(x, _weaponHandle.Rotation.Y, _weaponHandle.Rotation.Z);
            _weaponHandle.Rotation = _rotation;
        }

        if (!Function.Call<bool>(Hash.IS_CONTROL_PRESSED, 0, (int) _pitchDown)) return;
        {
            float x = _weaponHandle.Rotation.X;
            x += 1.0f * 0.05f * deltaTime;
            _rotation = new Vector3(x, _weaponHandle.Rotation.Y, _weaponHandle.Rotation.Z);
            _weaponHandle.Rotation = _rotation;
        }
    }

    private void LittleJacob_TrunkStateChanged(object sender, bool opened)
    {
        if (!opened && _weaponHandle != null && _weaponHandle.Handle != 0)
        {
            DeleteWeaponObject();
        }
    }

    private void Menu_TintChanged(object sender, int e)
    {
        Function.Call(Hash.SET_WEAPON_OBJECT_TINT_INDEX, _weaponHandle.Handle, e);
    }

    private void Menu_CamoColorChanged(object sender, CamoColorEventArgs e)
    {
        Function.Call(Hash._SET_WEAPON_OBJECT_LIVERY_COLOR, _weaponHandle.Handle, e.Camo, e.ColorIndex);

        var slide = TintsAndCamos.ReturnSlide(e.Camo);

        if (slide != (uint)WeaponComponentHash.Invalid)
        {
            Function.Call(Hash._SET_WEAPON_OBJECT_LIVERY_COLOR, _weaponHandle.Handle, slide, e.ColorIndex);
        }
    }

    private void LoadAttachments(uint hash, bool luxOn)
    {
        if (!LoadoutSaving.IsWeaponInStore(hash)) return;
        var storedWeapon = LoadoutSaving.GetStoreReference(hash);

        if (SkipComponent(storedWeapon.Varmod, ComponentIndex.Varmod))
        {
            if (!luxOn)
                GiveWeaponComponentToObject(storedWeapon.Varmod, false);
        }

        if (SkipComponent(storedWeapon.Camo, ComponentIndex.Livery))
        {
            GiveWeaponComponentToObject(storedWeapon.Camo, true);
            uint slide = TintsAndCamos.ReturnSlide(storedWeapon.Camo);

            if (slide != (uint)WeaponComponentHash.Invalid)
            {
                GiveWeaponComponentToObject(slide, true);
                Function.Call(Hash._SET_WEAPON_OBJECT_LIVERY_COLOR, _weaponHandle.Handle, slide, storedWeapon.GetCamoColor());
            }

            Function.Call(Hash._SET_WEAPON_OBJECT_LIVERY_COLOR, _weaponHandle.Handle, storedWeapon.Camo, storedWeapon.GetCamoColor());
        }

        Function.Call(Hash.SET_WEAPON_OBJECT_TINT_INDEX, _weaponHandle.Handle, storedWeapon.GetTintIndex());

        if (SkipComponent(storedWeapon.Barrel, ComponentIndex.Barrel))
        {
            GiveWeaponComponentToObject(storedWeapon.Barrel, true);
        }

        if (SkipComponent(storedWeapon.Clip, ComponentIndex.Clip))
        {
            GiveWeaponComponentToObject(storedWeapon.Clip, true);
        }

        if (SkipComponent(storedWeapon.Flashlight, ComponentIndex.Flashlight))
        {
            GiveWeaponComponentToObject(storedWeapon.Flashlight, true);
        }

        if (SkipComponent(storedWeapon.Grip, ComponentIndex.Grip))
        {
            GiveWeaponComponentToObject(storedWeapon.Grip, true);
        }

        if (SkipComponent(storedWeapon.Scope, ComponentIndex.Scope))
        {
            GiveWeaponComponentToObject(storedWeapon.Scope, true);
        }

        if (SkipComponent(storedWeapon.Muzzle, ComponentIndex.Muzzle))
        {
            GiveWeaponComponentToObject(storedWeapon.Muzzle, true);
        }
    }

    private bool SkipComponent(uint component, string index)
    {
        return component != (uint)WeaponComponentHash.Invalid && (_compFromStorage || (!_compFromStorage && index != _skipIndex));
    }

    private void Menu_SpawnWeaponObject(object sender, uint hash)
    {
        if (_hash != hash)
            _rotation = Vector3.Zero;

        _hash = hash;
        _doObjectSpawn = true;
        _compFromStorage = true;
    }

    private void SpawnWeaponObject(uint hash, bool luxe, uint luxeHash)
    {
        var luxOn = false;
        var luxModel = 0;
        DeleteWeaponObject();
        Function.Call(Hash.REQUEST_WEAPON_ASSET, hash, 31, 0);

        while (!Function.Call<bool>(Hash.HAS_WEAPON_ASSET_LOADED, hash))
        {
            Wait(1);
        }

        if (luxe)
        {
            luxModel = LoadComponentModel(luxeHash);
            _weaponHandle = Function.Call<Prop>(Hash.CREATE_WEAPON_OBJECT, hash, 1, Main.LittleJacob.Vehicle.RearPosition.X + (Main.Camera.Direction.X / 1.4f), Main.LittleJacob.Vehicle.RearPosition.Y + (Main.Camera.Direction.Y / 1.4f), Main.LittleJacob.Vehicle.RearPosition.Z + 0.15f, true, 1, luxModel, 0, 1);
            luxOn = true;
        } else
        {
            _weaponHandle = Function.Call<Prop>(Hash.CREATE_WEAPON_OBJECT, hash, 1, Main.LittleJacob.Vehicle.RearPosition.X + (Main.Camera.Direction.X / 1.4f), Main.LittleJacob.Vehicle.RearPosition.Y + (Main.Camera.Direction.Y / 1.4f), Main.LittleJacob.Vehicle.RearPosition.Z + 0.15f, true, 1, 0);
        }

        _weaponHandle.PositionNoOffset = new Vector3(Main.LittleJacob.Vehicle.RearPosition.X + (Main.Camera.Direction.X / 1.2f), Main.LittleJacob.Vehicle.RearPosition.Y + (Main.Camera.Direction.Y / 1.2f), Main.LittleJacob.Vehicle.RearPosition.Z + 0.4f);
        _weaponHandle.HasGravity = false;
        _weaponHandle.IsCollisionEnabled = false;
        _weaponHandle.Heading = Main.Camera.ForwardVector.ToHeading();
        float z = _weaponHandle.Rotation.Z - 10;
        _weaponHandle.Rotation = new Vector3(_rotation.X, _rotation.Y, z);
        Function.Call(Hash.REMOVE_WEAPON_ASSET, hash);
        LoadAttachments(hash, luxOn);

        if (luxOn)
            Function.Call(Hash.SET_MODEL_AS_NO_LONGER_NEEDED, luxModel);
    }

    private void Menu_ComponentSelected(object sender, ComponentPreviewEventArgs component)
    {
        _doComponentChange = true;
        _new = component.PreviewComponent;
        _skipIndex = component.ComponentIndex;
        _hash = component.WeaponHash;
    }

    private int LoadComponentModel(uint component)
    {
        int componentModel = Function.Call<int>(Hash.GET_WEAPON_COMPONENT_TYPE_MODEL, component);

        if (componentModel == 0) return 0;
        Function.Call(Hash.REQUEST_MODEL, componentModel);

        while (!Function.Call<bool>(Hash.HAS_MODEL_LOADED, componentModel))
        {
            Wait(1);
        }

        return componentModel;

    }

    private void GiveWeaponComponentToObject(uint component, bool force)
    {
        var isLuxe = _skipIndex == ComponentIndex.Varmod && component != (uint)WeaponComponentHash.Invalid;

        if (!force && (component == (uint)WeaponComponentHash.Invalid || isLuxe))
        {
            SpawnWeaponObject(_hash, isLuxe, component);
            return;
        }

        var componentModel = Function.Call<int>(Hash.GET_WEAPON_COMPONENT_TYPE_MODEL, component);
        if (componentModel == 0) return;
        Function.Call(Hash.REQUEST_MODEL, componentModel);

        while (!Function.Call<bool>(Hash.HAS_MODEL_LOADED, componentModel))
        {
            Wait(1);
        }

        Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_WEAPON_OBJECT, _weaponHandle.Handle, component);
        Function.Call(Hash.SET_MODEL_AS_NO_LONGER_NEEDED, componentModel);
    }

    private void DeleteWeaponObject()
    {
        if (_weaponHandle != null && _weaponHandle.Handle != 0)
        {
            _weaponHandle.Delete();
        }
    }
}