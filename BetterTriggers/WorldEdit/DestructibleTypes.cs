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
using System.Threading;
using System.Threading.Tasks;
using War3Net.Build.Extensions;
using War3Net.IO.Slk;

namespace BetterTriggers.WorldEdit
{
    public class DestructibleTypes
    {
        private static List<DestructibleType> destructibles;
        private static List<DestructibleType> destructiblesBase;
        private static List<DestructibleType> destructiblesCustom;

        internal static List<DestructibleType> GetAll()
        {
            return destructibles;
        }

        internal static List<DestructibleType> GetBase()
        {
            return destructiblesBase;
        }

        internal static List<DestructibleType> GetCustom()
        {
            return destructiblesCustom;
        }

        internal static void Load()
        {
            destructibles = new List<DestructibleType>();
            destructiblesBase = new List<DestructibleType>();
            destructiblesCustom = new List<DestructibleType>();

            var units = (CASCFolder)Casc.GetWar3ModFolder().Entries["units"];

            // Parse ini file
            CASCFile destSkins = (CASCFile)units.Entries["destructableskin.txt"];
            var file = Casc.GetCasc().OpenFile(destSkins.FullName);
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

                var destructible = new DestructibleType()
                {
                    DestCode = id,
                    DisplayName = name,
                    Model = model,
                };
                destructibles.Add(destructible);
                destructiblesBase.Add(destructible);
            }


            // Read custom destructible definition data
            string filePath = "war3map.w3b";
            if (!File.Exists(Path.Combine(CustomMapData.mapPath, filePath)))
                return;

            while (CustomMapData.IsMapSaving())
            {
                Thread.Sleep(1000);
            }

            using (Stream s = new FileStream(Path.Combine(CustomMapData.mapPath, filePath), FileMode.Open, FileAccess.Read))
            {
                BinaryReader binaryReader = new BinaryReader(s);
                var customDestructibles = BinaryReaderExtensions.ReadMapDestructableObjectData(binaryReader);

                for (int i = 0; i < customDestructibles.NewDestructables.Count; i++)
                {
                    var dest = customDestructibles.NewDestructables[i];
                    DestructibleType destructible = new DestructibleType()
                    {
                        DestCode = dest.ToString().Substring(0, 4),
                        DisplayName = dest.ToString(), // We want to replace this display name with locales
                    };
                    destructibles.Add(destructible);
                    destructiblesCustom.Add(destructible);
                }
            }
        }
    }
}
