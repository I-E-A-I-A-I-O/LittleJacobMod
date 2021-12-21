using System;
using LittleJacobMod.Utils;
using GTA;
using GTA.Native;

class VisionToggleMain : Script
{
    Controls ToggleVisorKey;

    public VisionToggleMain()
    {
        var settings = ScriptSettings.Load("scripts\\LittleJacobMod.ini");
        ToggleVisorKey = settings.GetValue("Controls", "ToggleVisor", Controls.INPUT_VEH_FLY_UNDERCARRIAGE);

        Tick += VisionToggleMain_Tick;
    }

    private void VisionToggleMain_Tick(object sender, EventArgs e)
    {
        int pedType = Main.IsMPPed();

        if (pedType == -1)
        {
            return;
        }

        if (Game.Player.IsDead)
        {
            Game.IsNightVisionActive = false;
            Game.IsThermalVisionActive = false;
        }

        int helmIndx = Function.Call<int>(Hash.GET_PED_PROP_INDEX, Main.PPID, 0);
        int helmColor = Function.Call<int>(Hash.GET_PED_PROP_TEXTURE_INDEX, Main.PPID, 0);

        if (!IsHelmetValid(helmIndx, pedType))
        {
            return;
        }

        if (Function.Call<bool>(Hash.IS_CONTROL_JUST_PRESSED, 0, (int)ToggleVisorKey))
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

        if (IsOnHelmet(helmIndx, pedType) && ShouldDeactiveVision())
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

    bool CanActivate()
    {
        return !Game.Player.Character.IsSwimming && !Game.Player.Character.IsSwimmingUnderWater && !Game.Player.Character.IsInParachuteFreeFall && !Game.Player.Character.IsFalling && !Game.Player.Character.IsInVehicle() && !Game.Player.IsAiming;
    }

    bool CanDeactivate()
    {
        return !Game.Player.IsAiming;
    }

    bool ShouldDeactiveVision()
    {
        return Game.Player.Character.IsInVehicle() || Game.Player.Character.IsSwimmingUnderWater;
    }

    bool IsOffHelmet(int helmet, int pedType)
    {
        if (pedType == 0)
        {
            return helmet == 117 || helmet == 119 || helmet == 148;
        }

        if (pedType == 1)
        {
            return helmet == 116 || helmet == 118 || helmet == 147;
        }

        return false;
    }

    bool IsOnHelmet(int helmet, int pedType)
    {
        if (pedType == 0)
        {
            return helmet == 116 || helmet == 118 || helmet == 147;
        }

        if (pedType == 1)
        {
            return helmet == 115 || helmet == 117 || helmet == 146;
        }

        return false;
    }

    bool IsHelmetValid(int helmet, int pedType)
    {
        return IsOnHelmet(helmet, pedType) || IsOffHelmet(helmet, pedType);
    }

    int HelmetType(int helmet, int pedType)
    {
        if (pedType == 0)
        {
            if (helmet == 116 || helmet == 117 || helmet == 147 || helmet == 148)
            {
                return 1;
            } else
            {
                return 0;
            }
        } else
        {
            if (helmet == 115 || helmet == 116 || helmet == 146 || helmet == 147)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}
