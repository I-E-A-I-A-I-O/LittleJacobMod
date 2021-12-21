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

        public static List<string> NV2Colors = new List<string>()
        {
            "Black",
            "Sage",
            "Beige",
            "Stone",
            "White",
            "Beige Digital",
            "Green Digital",
            "Desert Digital"
        };

        public static List<string> HelmColors = new List<string>()
        {
            "Black",
            "Moss",
            "Brown",
            "White",
            "Green Camo",
            "Orange Camo",
            "Purple Camo",
            "Pink Camo",
            "Leopard",
            "Brown Digital",
            "Tiger",
            "Pink Pattern",
            "Peach Digital",
            "Fall",
            "Dark Woodland",
            "Crosshatch",
            "Green Pattern",
            "Gray Woodland",
            "Aqua",
            "Splinter",
            "Contrast",
            "Cobble",
            "Brushstroke",
            "Flecktarn",
            "Black & Red",
            "Zebra"
        };

        public static uint ReturnSlide(uint camo)
        {
            switch(camo)
            {
                case (uint)WeaponComponentHash.PistolMk2Camo:
                    return (uint)WeaponComponentHash.PistolMk2CamoSlide;

                case (uint)WeaponComponentHash.PistolMk2Camo02:
                    return (uint)WeaponComponentHash.PistolMk2Camo02Slide;

                case (uint)WeaponComponentHash.PistolMk2Camo03:
                    return (uint)WeaponComponentHash.PistolMk2Camo03Slide;

                case (uint)WeaponComponentHash.PistolMk2Camo04:
                    return (uint)WeaponComponentHash.PistolMk2Camo04Slide;

                case (uint)WeaponComponentHash.PistolMk2Camo05:
                    return (uint)WeaponComponentHash.PistolMk2Camo05Slide;

                case (uint)WeaponComponentHash.PistolMk2Camo06:
                    return (uint)WeaponComponentHash.PistolMk2Camo06Slide;

                case (uint)WeaponComponentHash.PistolMk2Camo07:
                    return (uint)WeaponComponentHash.PistolMk2Camo07Slide;

                case (uint)WeaponComponentHash.PistolMk2Camo08:
                    return (uint)WeaponComponentHash.PistolMk2Camo08Slide;

                case (uint)WeaponComponentHash.PistolMk2Camo09:
                    return (uint)WeaponComponentHash.PistolMk2Camo09Slide;

                case (uint)WeaponComponentHash.PistolMk2Camo10:
                    return (uint)WeaponComponentHash.PistolMk2Camo10Slide;

                case (uint)WeaponComponentHash.PistolMk2CamoIndependence01:
                    return (uint)WeaponComponentHash.PistolMk2CamoIndependence01Slide;

                case (uint)WeaponComponentHash.SNSPistolMk2Camo:
                    return (uint)WeaponComponentHash.SNSPistolMk2CamoSlide;

                case (uint)WeaponComponentHash.SNSPistolMk2Camo02:
                    return (uint)WeaponComponentHash.SNSPistolMk2Camo02Slide;

                case (uint)WeaponComponentHash.SNSPistolMk2Camo03:
                    return (uint)WeaponComponentHash.SNSPistolMk2Camo03Slide;

                case (uint)WeaponComponentHash.SNSPistolMk2Camo04:
                    return (uint)WeaponComponentHash.SNSPistolMk2Camo04Slide;

                case (uint)WeaponComponentHash.SNSPistolMk2Camo05:
                    return (uint)WeaponComponentHash.SNSPistolMk2Camo05Slide;

                case (uint)WeaponComponentHash.SNSPistolMk2Camo06:
                    return (uint)WeaponComponentHash.SNSPistolMk2Camo06Slide;

                case (uint)WeaponComponentHash.SNSPistolMk2Camo07:
                    return (uint)WeaponComponentHash.SNSPistolMk2Camo07Slide;

                case (uint)WeaponComponentHash.SNSPistolMk2Camo08:
                    return (uint)WeaponComponentHash.SNSPistolMk2Camo08Slide;

                case (uint)WeaponComponentHash.SNSPistolMk2Camo09:
                    return (uint)WeaponComponentHash.SNSPistolMk2Camo09Slide;

                case (uint)WeaponComponentHash.SNSPistolMk2Camo10:
                    return (uint)WeaponComponentHash.SNSPistolMk2Camo10Slide;

                case (uint)WeaponComponentHash.SNSPistolMk2CamoIndependence01:
                    return (uint)WeaponComponentHash.SNSPistolMk2CamoIndependence01Slide;

                default:
                    return (uint)WeaponComponentHash.Invalid;
            }
        }
    }
}
