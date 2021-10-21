using System;
using GTA;
using System.IO;

namespace LittleJacobMod.Saving.Utils 
{
    internal static class Migrator
    {
        static string OldPath => $"{Directory.GetCurrentDirectory()}\\scripts\\LittleJacobMod\\Ladouts"; 
        static string NewPath => $"{Directory.GetCurrentDirectory()}\\scripts\\LittleJacobMod\\Loadouts";

        public static void Migrate()
        {
            if (!Directory.Exists(OldPath))
            {
                return;
            }

            if (!Directory.Exists(NewPath))
            {
                Directory.CreateDirectory(NewPath);
            }

            var dirContents = Directory.GetFiles(OldPath);

            if (dirContents.Length == 0)
            {
                return;
            }

            foreach (string file in dirContents)
            {
                var splitPath = file.Split('\\');
                var fileName = splitPath[splitPath.Length - 1];
                File.Move(file, $"{NewPath}\\{fileName}");
            }

            Directory.Delete(OldPath);
        }
    }
}
