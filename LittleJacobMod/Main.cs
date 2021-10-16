using System;
using System.Collections.Generic;
using GTA;
using System.Windows.Forms;

public class Main : Script
{
    string openMenuControl;
    Keys openMenuKey;

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
    }
}
