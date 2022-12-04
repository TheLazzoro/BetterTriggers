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
using War3Net.Build.Object;
using War3Net.Common.Extensions;
using War3Net.IO.Slk;

namespace BetterTriggers.WorldEdit
{
    internal class DoodadTypes
    {
        private static Dictionary<string, DoodadType> doodads;
        private static Dictionary<string, DoodadType> doodadsBase;
        private static Dictionary<string, DoodadType> doodadsCustom;

        internal static List<DoodadType> GetAll()
        {
            return doodads.Select(kvp => kvp.Value).ToList();
        }

        internal static List<DoodadType> GetBase()
        {
            return doodadsBase.Select(kvp => kvp.Value).ToList();
        }

        internal static List<DoodadType> GetCustom()
        {
            return doodadsCustom.Select(kvp => kvp.Value).ToList();
        }

        internal static DoodadType GetDoodadType(string doodcode) 
        {
            DoodadType doodad;
            doodads.TryGetValue(doodcode, out doodad);
            return doodad;
        }

        internal static string GetName(string doodcode)
        {
            DoodadType doodType = GetDoodadType(doodcode);
            if (doodType == null)
                return null;

            return doodType.DisplayName;
        }

        internal static void Load(bool isTest = false)
        {
            doodads = new Dictionary<string, DoodadType>();
            doodadsBase = new Dictionary<string, DoodadType>();
            doodadsCustom = new Dictionary<string, DoodadType>();

            string text;

            if (isTest)
            {
                using (Stream doodadskin = new FileStream(Path.Combine(Directory.GetCurrentDirectory(), "TestResources/doodadskins.txt"), FileMode.Open))
                {
                    var reader = new StreamReader(doodadskin);
                    text = reader.ReadToEnd();
                }
            }
            else
            {
                var folderDoodads = (CASCFolder)Casc.GetWar3ModFolder().Entries["doodads"];
                CASCFile doodadSkins = (CASCFile)folderDoodads.Entries["doodadskins.txt"];
                using (Stream doodadskin = Casc.GetCasc().OpenFile(doodadSkins.FullName))
                {
                    var reader = new StreamReader(doodadskin);
                    text = reader.ReadToEnd();
                }
            }

            // Parse ini file
            var data = IniFileConverter.GetIniData(text);

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
                    DisplayName = Locale.Translate(name),
                    Model = model,
                };
                doodads.Add(id, doodad);
                doodadsBase.Add(id, doodad);
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
                var customDoodads = War3Net.Build.Extensions.BinaryReaderExtensions.ReadMapDoodadObjectData(binaryReader);

                for (int i = 0; i < customDoodads.BaseDoodads.Count; i++)
                {
                    var baseDood = customDoodads.BaseDoodads[i];
                    SetCustomFields(baseDood, Int32Extensions.ToRawcode(baseDood.OldId));
                }

                for (int i = 0; i < customDoodads.NewDoodads.Count; i++)
                {
                    var dood = customDoodads.NewDoodads[i];
                    DoodadType baseDood = GetDoodadType(Int32Extensions.ToRawcode(dood.OldId));
                    DoodadType doodad = new DoodadType()
                    {
                        DoodCode = dood.ToString().Substring(0, 4),
                        DisplayName = baseDood.DisplayName,
                    };
                    doodads.Add(doodad.DoodCode, doodad);
                    doodadsCustom.Add(doodad.DoodCode, doodad);
                    SetCustomFields(dood, doodad.DoodCode);
                }
            }
        }

        private static void SetCustomFields(VariationObjectModification modified, string buffcode)
        {
            DoodadType doodType = GetDoodadType(buffcode);
            string displayName = doodType.DisplayName;
            foreach (var modification in modified.Modifications)
            {
                if (Int32Extensions.ToRawcode(modification.Id) == "dnam")
                    displayName = MapStrings.GetString(modification.ValueAsString);
            }
            doodType.DisplayName = displayName;
        }

    }
}
