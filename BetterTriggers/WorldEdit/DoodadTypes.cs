using BetterTriggers.Utility;
using CASCLib;
using IniParser.Model;
using IniParser.Parser;
using Model.War3Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using War3Net.Build.Extensions;
using War3Net.IO.Slk;

namespace BetterTriggers.WorldEdit
{
    public class DoodadTypes
    {
        private static List<DoodadType> doodads;

        public static List<DoodadType> GetDoodadTypesAll()
        {
            return doodads;
        }

        internal static void Load()
        {
            doodads = new List<DoodadType>();

            var folderDoodads = (CASCFolder)Casc.GetWar3ModFolder().Entries["doodads"];

            // Parse ini file
            CASCFile doodadSkins = (CASCFile)folderDoodads.Entries["doodadskins.txt"];
            var file = Casc.GetCasc().OpenFile(doodadSkins.FullName);
            var reader = new StreamReader(file);
            var text = reader.ReadToEnd();

            var iniFile = IniFileConverter.Convert(text);
            IniDataParser parser = new IniDataParser();
            parser.Configuration.AllowDuplicateSections = true;
            parser.Configuration.AllowDuplicateKeys = true;
            IniData data = parser.Parse(iniFile);


            var sections = data.Sections.GetEnumerator();
            while (sections.MoveNext())
            {
                var id = sections.Current.SectionName;
                var keys = sections.Current.Keys;
                var name = keys["Name"];
                var model = keys["file"];

                doodads.Add(new DoodadType()
                {
                    DoodCode = id,
                    DisplayName = name,
                    Model = model,
                });
            }


            // Read custom doodad definition data
            string path = @"C:\Users\Lasse Dam\Desktop\test2.w3x\war3map.w3d";
            if (!File.Exists(path))
                return;

            Stream s = new FileStream(path, FileMode.Open);
            BinaryReader binaryReader = new BinaryReader(s);
            var customDestructibles = BinaryReaderExtensions.ReadMapDoodadObjectData(binaryReader);

            for (int i = 0; i < customDestructibles.NewDoodads.Count; i++)
            {
                var dood = customDestructibles.NewDoodads[i];
                DoodadType destructible = new DoodadType()
                {
                    DoodCode = dood.ToString().Substring(0, 4),
                    DisplayName = dood.ToString(), // We want to replace this display name with locales
                };
                doodads.Add(destructible);
            }
        }
    }
}
