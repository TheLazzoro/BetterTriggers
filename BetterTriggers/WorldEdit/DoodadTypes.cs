using BetterTriggers.Models.War3Data;
using BetterTriggers.Utility;
using CASCLib;
using IniParser.Model;
using IniParser.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using War3Net.Build.Extensions;
using War3Net.IO.Slk;

namespace BetterTriggers.WorldEdit
{
    internal class DoodadTypes
    {
        private static List<DoodadType> doodads;
        private static List<DoodadType> doodadsBase;
        private static List<DoodadType> doodadsCustom;

        internal static List<DoodadType> GetAll()
        {
            return doodads;
        }

        internal static List<DoodadType> GetBase()
        {
            return doodadsBase;
        }

        internal static List<DoodadType> GetCustom()
        {
            return doodadsCustom;
        }

        internal static void Load()
        {
            doodads = new List<DoodadType>();
            doodadsBase = new List<DoodadType>();
            doodadsCustom = new List<DoodadType>();

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

                var doodad = new DoodadType()
                {
                    DoodCode = id,
                    DisplayName = name,
                    Model = model,
                };
                doodads.Add(doodad);
                doodadsBase.Add(doodad);
            }


            // Read custom doodad definition data
            string filePath = "war3map.w3d";
            if (!File.Exists(Path.Combine(CustomMapData.mapPath, filePath)))
                return;

            while (CustomMapData.IsMapSaving())
            {
                Thread.Sleep(1000);
            }

            using (Stream s = new FileStream(Path.Combine(CustomMapData.mapPath, filePath), FileMode.Open, FileAccess.Read))
            {
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
                    doodadsCustom.Add(destructible);
                }
            }
        }
    }
}
