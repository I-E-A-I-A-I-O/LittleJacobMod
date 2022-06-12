using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using LittleJacobMod.Utils.Types;

namespace LittleJacobMod.Utils
{
    internal static class FileParser
    {
        public static List<Weapon>? DeserializeJsonContents(string path)
        {
            var contents = File.ReadAllText(path);
            var weapons = JsonConvert.DeserializeObject<List<Weapon>>(contents);

            return weapons ?? null;
        }
    }
}
