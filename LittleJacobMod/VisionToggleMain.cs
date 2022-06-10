namespace LittleJacobMod;
using System;
using GTA;
using GTA.Native;
using Utils;

internal class VisionToggleMain : Script
{
    private readonly Controls _toggleVisorKey;

    public VisionToggleMain()
    {
        var settings = ScriptSettings.Load("scripts\\LittleJacobMod.ini");
        _toggleVisorKey = settings.GetValue("Controls", "ToggleVisor", Controls.INPUT_VEH_FLY_UNDERCARRIAGE);

        Tick += VisionToggleMain_Tick;
    }

    private void VisionToggleMain_Tick(object sender, EventArgs e)
    {
        var pedType = Main.IsMPped();

        if (pedType == -1)
        {
            return;
        }

        if (Game.Player.IsDead)
        {
            Game.IsNightVisionActive = false;
            Game.IsThermalVisionActive = false;
        }

        var helmIndx = Function.Call<int>(Hash.GET_PED_PROP_INDEX, Main.PPID, 0);
        var helmColor = Function.Call<int>(Hash.GET_PED_PROP_TEXTURE_INDEX, Main.PPID, 0);

        if (!IsHelmetValid(helmIndx, pedType))
        {
            return;
        }

        if (Function.Call<bool>(Hash.IS_CONTROL_JUST_PRESSED, 0, (int)_toggleVisorKey))
        {
            if (IsOffHelmet(helmIndx, pedType) && CanActivate())
            {
                if (HelmetType(helmIndx, pedType) == 1)
                {
                    Game.IsNightVisionActive = true;
                } else
                {
                    Game.IsThermalVisionActive = true;
                }

                Function.Call(Hash.SET_PED_PROP_INDEX, Main.PPID, 0, helmIndx - 1, helmColor, 1);
                return;
            }

            if (IsOnHelmet(helmIndx, pedType) && CanDeactivate())
            {
                if (HelmetType(helmIndx, pedType) == 1)
                {
                    Game.IsNightVisionActive = false;
                }
                else
                {
                    Game.IsThermalVisionActive = false;
                }

                Function.Call(Hash.SET_PED_PROP_INDEX, Main.PPID, 0, helmIndx + 1, helmColor, 1);
                return;
            }
        }

        if (!IsOnHelmet(helmIndx, pedType) || !ShouldDeactiveVision()) return;
        if (HelmetType(helmIndx, pedType) == 1)
        {
            Game.IsNightVisionActive = false;
        }
        else
        {
            Game.IsThermalVisionActive = false;
        }

        Function.Call(Hash.SET_PED_PROP_INDEX, Main.PPID, 0, helmIndx + 1, helmColor, 1);
    }

    private static bool CanActivate()
    {
        return !Game.Player.Character.IsSwimming && !Game.Player.Character.IsSwimmingUnderWater && !Game.Player.Character.IsInParachuteFreeFall && !Game.Player.Character.IsFalling && !Game.Player.Character.IsInVehicle() && !Game.Player.IsAiming;
    }

    private static bool CanDeactivate()
    {
        return !Game.Player.IsAiming;
    }

    private static bool ShouldDeactiveVision()
    {
        return Game.Player.Character.IsInVehicle() || Game.Player.Character.IsSwimmingUnderWater;
    }

    private static bool IsOffHelmet(int helmet, int pedType)
    {
        switch (pedType)
        {
            case 0:
                return helmet == 117 || helmet == 119 || helmet == 148;
            case 1:
                return helmet == 116 || helmet == 118 || helmet == 147;
            default:
                return false;
        }
    }

    private static bool IsOnHelmet(int helmet, int pedType)
    {
        switch (pedType)
        {
            case 0:
                return helmet == 116 || helmet == 118 || helmet == 147;
            case 1:
                return helmet == 115 || helmet == 117 || helmet == 146;
            default:
                return false;
        }
    }

    private static bool IsHelmetValid(int helmet, int pedType)
    {
        return IsOnHelmet(helmet, pedType) || IsOffHelmet(helmet, pedType);
    }

    private static int HelmetType(int helmet, int pedType)
    {
        if (pedType == 0)
        {
            if (helmet == 116 || helmet == 117 || helmet == 147 || helmet == 148)
            {
                return 1;
            }

            return 0;
        }

        if (helmet == 115 || helmet == 116 || helmet == 146 || helmet == 147)
        {
            return 1;
        }

        return 0;
    }
}
