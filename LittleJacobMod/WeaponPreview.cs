using System;
using GTA;
using GTA.Native;
using GTA.Math;
using LittleJacobMod.Saving;
using LittleJacobMod.Interface;
using LittleJacobMod.Utils;

class WeaponPreview : Script
{
    Prop _weaponHandle;
    bool _doComponentReload;
    bool _doObjectSpawn;
    bool _doComponentChange;
    bool _compFromStorage;
    uint _hash = 453432689;
    uint _new = (uint)WeaponComponentHash.Invalid;
    ComponentIndex _skipIndex;

    public WeaponPreview()
    {
        Menu.ComponentSelected += Menu_ComponentSelected;
        Menu.SpawnWeaponObject += Menu_SpawnWeaponObject;
        Menu.CamoColorChanged += Menu_CamoColorChanged;
        Menu.TintChanged += Menu_TintChanged;

        LittleJacob.TrunkStateChanged += LittleJacob_TrunkStateChanged;

        Tick += WeaponPreview_Tick;
    }

    private void WeaponPreview_Tick(object sender, EventArgs e)
    {
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

    private void LoadAttachments(uint hash, bool luxe, bool luxOn)
    {
        if (LoadoutSaving.IsWeaponInStore(hash))
        {
            var storedWeapon = LoadoutSaving.GetStoreReference(hash);

            if (!luxe && IsLuxe(storedWeapon.Camo))
                luxe = true;

            if (SkipComponent(storedWeapon.Camo, ComponentIndex.Livery))
            {
                if (!luxe)
                {
                    GiveWeaponComponentToObject(storedWeapon.Camo, true);
                    uint slide = TintsAndCamos.ReturnSlide(storedWeapon.Camo);

                    if (slide != (uint)WeaponComponentHash.Invalid)
                    {
                        GiveWeaponComponentToObject(slide, true);
                        Function.Call(Hash._SET_WEAPON_OBJECT_LIVERY_COLOR, _weaponHandle.Handle, slide, storedWeapon.GetCamoColor());
                    }

                    Function.Call(Hash._SET_WEAPON_OBJECT_LIVERY_COLOR, _weaponHandle.Handle, storedWeapon.Camo, storedWeapon.GetCamoColor());
                } else if (!luxOn)
                    GiveWeaponComponentToObject(storedWeapon.Camo, false);
                else if (luxOn)
                    GiveWeaponComponentToObject(storedWeapon.Camo, true);
            }

            if (!luxe)
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
    }

    private bool SkipComponent(uint component, ComponentIndex index)
    {
        return component != (uint)WeaponComponentHash.Invalid && (_compFromStorage || (!_compFromStorage && index != _skipIndex));
    }

    private void Menu_SpawnWeaponObject(object sender, uint hash)
    {
        _doObjectSpawn = true;
        _hash = hash;
        _compFromStorage = true;
    }

    private void SpawnWeaponObject(uint hash, bool luxe, uint luxeHash)
    {
        bool luxOn = false;
        int luxModel = 0;
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
        _weaponHandle.Rotation = new Vector3(_weaponHandle.Rotation.X, _weaponHandle.Rotation.Y, _weaponHandle.Rotation.Z - 10);
        Function.Call(Hash.REMOVE_WEAPON_ASSET, hash);

        LoadAttachments(hash, luxe, luxOn);

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

    int LoadComponentModel(uint component)
    {
        int componentModel = Function.Call<int>(Hash.GET_WEAPON_COMPONENT_TYPE_MODEL, component);

        if (componentModel != 0)
        {
            Function.Call(Hash.REQUEST_MODEL, componentModel);

            while (!Function.Call<bool>(Hash.HAS_MODEL_LOADED, componentModel))
            {
                Wait(1);
            }

            return componentModel;
        }

        return 0;
    }

    private void GiveWeaponComponentToObject(uint component, bool force)
    {
        bool isLuxe = IsLuxe(component);

        if (!force && (component == (uint)WeaponComponentHash.Invalid || isLuxe))
        {
            SpawnWeaponObject(_hash, isLuxe, component);
            return;
        }

        int componentModel = Function.Call<int>(Hash.GET_WEAPON_COMPONENT_TYPE_MODEL, component);
        if (componentModel != 0)
        {
            Function.Call(Hash.REQUEST_MODEL, componentModel);

            while (!Function.Call<bool>(Hash.HAS_MODEL_LOADED, componentModel))
            {
                Wait(1);
            }

            Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_WEAPON_OBJECT, _weaponHandle.Handle, component);
            Function.Call(Hash.SET_MODEL_AS_NO_LONGER_NEEDED, componentModel);
        }
    }

    public void DeleteWeaponObject()
    {
        if (_weaponHandle != null && _weaponHandle.Handle != 0)
        {
            _weaponHandle.Delete();
        }
    }

    bool IsLuxe(uint component)
    {
        return component == (uint)WeaponComponentHash.AdvancedRifleVarmodLuxe
             || component == (uint)WeaponComponentHash.APPistolVarmodLuxe
             || component == (uint)WeaponComponentHash.AssaultRifleVarmodLuxe
             || component == (uint)WeaponComponentHash.AssaultSMGVarmodLowrider
             || component == (uint)WeaponComponentHash.BullpupRifleVarmodLow
             || component == (uint)WeaponComponentHash.CarbineRifleVarmodLuxe
             || component == (uint)WeaponComponentHash.CombatMGVarmodLowrider
             || component == (uint)WeaponComponentHash.CombatPistolVarmodLowrider
             || component == (uint)WeaponComponentHash.HeavyPistolVarmodLuxe
             || component == (uint)WeaponComponentHash.MarksmanRifleVarmodLuxe
             || component == (uint)WeaponComponentHash.MGVarmodLowrider
             || component == (uint)WeaponComponentHash.MicroSMGVarmodLuxe
             || component == (uint)WeaponComponentHash.Pistol50VarmodLuxe
             || component == (uint)WeaponComponentHash.PistolVarmodLuxe
             || component == (uint)WeaponComponentHash.PumpShotgunVarmodLowrider
             || component == (uint)WeaponComponentHash.SMGVarmodLuxe
             || component == (uint)WeaponComponentHash.SNSPistolVarmodLowrider
             || component == (uint)WeaponComponentHash.SpecialCarbineVarmodLowrider;
    }
}
