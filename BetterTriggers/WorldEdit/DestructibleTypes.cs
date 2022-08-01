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
using War3Net.Common.Extensions;
using War3Net.IO.Slk;

namespace BetterTriggers.WorldEdit
{
    public class DestructibleTypes
    {
        private static Dictionary<string, DestructibleType> destructibles;
        private static Dictionary<string, DestructibleType> destructiblesBase;
        private static Dictionary<string, DestructibleType> destructiblesCustom;

        internal static List<DestructibleType> GetAll()
        {
            return destructibles.Select(kvp => kvp.Value).ToList();
        }

        internal static List<DestructibleType> GetBase()
        {
            return destructiblesBase.Select(kvp => kvp.Value).ToList();
        }

        internal static List<DestructibleType> GetCustom()
        {
            return destructiblesCustom.Select(kvp => kvp.Value).ToList();
        }

        public static DestructibleType GetDestType(string unitcode)
        {
            DestructibleType destType;
            destructibles.TryGetValue(unitcode, out destType);
            return destType;
        }

        // TODO: Doesn't return comments with the name (e.g. 'Diagonal 1' or 'Vertical')
        internal static string GetName(string destcode)
        {
            DestructibleType destType = GetDestType(destcode);
            if (destType == null)
                return null;

            return destType.DisplayName;
        }

        internal static void Load()
        {
            destructibles = new Dictionary<string, DestructibleType>();
            destructiblesBase = new Dictionary<string, DestructibleType>();
            destructiblesCustom = new Dictionary<string, DestructibleType>();

            var units = (CASCFolder)Casc.GetWar3ModFolder().Entries["units"];

            // Parse ini file
            CASCFile destSkins = (CASCFile)units.Entries["destructableskin.txt"];
            var file = Casc.GetCasc().OpenFile(destSkins.FullName);
            var reader = new StreamReader(file);
            var text = reader.ReadToEnd();

            var data = IniFileConverter.GetIniData(text);

            var sections = data.Sections.GetEnumerator();
            while (sections.MoveNext())
            {
                var id = sections.Current.SectionName;
                var keys = sections.Current.Keys;
                var name = Locale.Translate(keys["Name"]);
                var model = keys["file"];

                var destructible = new DestructibleType()
                {
                    DestCode = id,
                    DisplayName = name,
                    Model = model,
                };
                destructibles.TryAdd(destructible.DestCode, destructible);
                destructiblesBase.TryAdd(destructible.DestCode, destructible);
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
                var customDestructibles = War3Net.Build.Extensions.BinaryReaderExtensions.ReadMapDestructableObjectData(binaryReader);

                for (int i = 0; i < customDestructibles.NewDestructables.Count; i++)
                {
                    var dest = customDestructibles.NewDestructables[i];

                    DestructibleType baseDest = GetDestType(Int32Extensions.ToRawcode(dest.OldId));
                    string name = baseDest.DisplayName;
                    foreach (var modified in dest.Modifications)
                    {
                        if (Int32Extensions.ToRawcode(modified.Id) == "bnam")
                            name = MapStrings.GetString(modified.ValueAsString);
                    }

                    DestructibleType destructible = new DestructibleType()
                    {
                        DestCode = dest.ToString().Substring(0, 4),
                        DisplayName = name,
                    };
                    destructibles.TryAdd(destructible.DestCode, destructible);
                    destructiblesCustom.TryAdd(destructible.DestCode, destructible);
                }
            }
        }

    }
}
