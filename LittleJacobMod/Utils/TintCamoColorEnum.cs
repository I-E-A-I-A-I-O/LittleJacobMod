using System;
using System.Collections.Generic;
using GTA;

namespace LittleJacobMod.Utils
{
    public static class TintsAndCamos
    {
        public static List<string> Tints = new List<string>
        {
            "Black",
            "Green",
            "Gold",
            "Pink",
            "Army",
            "LSPD",
            "Orange",
            "Platinum"
        };

        public static List<string> TintsMk2 = new List<string>
        {
            "Classic Black",
            "Classic Gray",
            "Classic Two-Tone",
            "Classic White",
            "Classic Beige",
            "Classic Green",
            "Classic Blue",
            "Classic Earth",
            "Classic Brown & Black",
            "Red Contrast",
            "Blue Constrast",
            "Yellow Constrast",
            "Orange Contrast",
            "Bold Pink",
            "Bold Purple & Yellow",
            "Bold Orange",
            "Bold Green & Purple",
            "Bold Red Features",
            "Bold Green Features",
            "Bold Cyan Features",
            "Bold Yellow Features",
            "Bold Red & White",
            "Bold Blue & White",
            "Metallic Gold",
            "Metallic Platinum",
            "Metallic Gray & Lilac",
            "Metallic Purple & Lime",
            "Metallic Red",
            "Metallic Green",
            "Metallic Blue",
            "Metallic White & Aqua",
            "Metallic Red & Yellow"
        };

        public static List<string> CamoColor = new List<string>
        {
            "Gray",
            "Dark Gray",
            "Black",
            "White",
            "Blue",
            "Cyan",
            "Aqua",
            "Cool Blue",
            "Dark Blue",
            "Royal Blue",
            "Plum",
            "Dark Purple",
            "Purple",
            "Red",
            "Wine Red",
            "Magenta",
            "Pink",
            "Salmon",
            "Hot Pink",
            "Rust Orange",
            "Brown",
            "Earth",
            "Orange",
            "Light Orange",
            "Dark Yellow",
            "Yellow",
            "Light Brown",
            "Lime Green",
            "Olive",
            "Moss",
            "Turquoise",
            "Dark Green",
        };

        public static WeaponComponentHash ReturnSlide(WeaponComponentHash camo)
        {
            switch(camo)
            {
                case WeaponComponentHash.PistolMk2Camo:
                    return WeaponComponentHash.PistolMk2CamoSlide;

                case WeaponComponentHash.PistolMk2Camo02:
                    return WeaponComponentHash.PistolMk2Camo02Slide;

                case WeaponComponentHash.PistolMk2Camo03:
                    return WeaponComponentHash.PistolMk2Camo03Slide;

                case WeaponComponentHash.PistolMk2Camo04:
                    return WeaponComponentHash.PistolMk2Camo04Slide;

                case WeaponComponentHash.PistolMk2Camo05:
                    return WeaponComponentHash.PistolMk2Camo05Slide;

                case WeaponComponentHash.PistolMk2Camo06:
                    return WeaponComponentHash.PistolMk2Camo06Slide;

                case WeaponComponentHash.PistolMk2Camo07:
                    return WeaponComponentHash.PistolMk2Camo07Slide;

                case WeaponComponentHash.PistolMk2Camo08:
                    return WeaponComponentHash.PistolMk2Camo08Slide;

                case WeaponComponentHash.PistolMk2Camo09:
                    return WeaponComponentHash.PistolMk2Camo09Slide;

                case WeaponComponentHash.PistolMk2Camo10:
                    return WeaponComponentHash.PistolMk2Camo10Slide;

                case WeaponComponentHash.PistolMk2CamoIndependence01:
                    return WeaponComponentHash.PistolMk2CamoIndependence01Slide;

                case WeaponComponentHash.SNSPistolMk2Camo:
                    return WeaponComponentHash.SNSPistolMk2CamoSlide;

                case WeaponComponentHash.SNSPistolMk2Camo02:
                    return WeaponComponentHash.SNSPistolMk2Camo02Slide;

                case WeaponComponentHash.SNSPistolMk2Camo03:
                    return WeaponComponentHash.SNSPistolMk2Camo03Slide;

                case WeaponComponentHash.SNSPistolMk2Camo04:
                    return WeaponComponentHash.SNSPistolMk2Camo04Slide;

                case WeaponComponentHash.SNSPistolMk2Camo05:
                    return WeaponComponentHash.SNSPistolMk2Camo05Slide;

                case WeaponComponentHash.SNSPistolMk2Camo06:
                    return WeaponComponentHash.SNSPistolMk2Camo06Slide;

                case WeaponComponentHash.SNSPistolMk2Camo07:
                    return WeaponComponentHash.SNSPistolMk2Camo07Slide;

                case WeaponComponentHash.SNSPistolMk2Camo08:
                    return WeaponComponentHash.SNSPistolMk2Camo08Slide;

                case WeaponComponentHash.SNSPistolMk2Camo09:
                    return WeaponComponentHash.SNSPistolMk2Camo09Slide;

                case WeaponComponentHash.SNSPistolMk2Camo10:
                    return WeaponComponentHash.SNSPistolMk2Camo10Slide;

                case WeaponComponentHash.SNSPistolMk2CamoIndependence01:
                    return WeaponComponentHash.SNSPistolMk2CamoIndependence01Slide;

                default:
                    return WeaponComponentHash.Invalid;
            }
        }
    }
}
