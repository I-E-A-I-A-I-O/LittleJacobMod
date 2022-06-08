using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using LittleJacobMod.Utils.Types;

namespace LittleJacobMod.Utils
{
    internal class FileParser
    {
        public static List<Weapon> DesearilizeJsonContents(string path)
        {
            string contents = File.ReadAllText(path);
            List<Weapon> weapons = JsonConvert.DeserializeObject<List<Weapon>>(contents);

            if (weapons != null) return weapons;

            return null;
        }
    }
}
