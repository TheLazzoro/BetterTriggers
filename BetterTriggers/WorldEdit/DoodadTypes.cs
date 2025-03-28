﻿using BetterTriggers.Containers;
using BetterTriggers.Models.War3Data;
using BetterTriggers.Utility;
using BetterTriggers.WorldEdit.GameDataReader;
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
using System.Xml.Linq;
using War3Net.Build.Extensions;
using War3Net.Build.Object;
using War3Net.Common.Extensions;
using War3Net.IO.Slk;
using static System.Net.Mime.MediaTypeNames;

namespace BetterTriggers.WorldEdit
{
    public class DoodadTypes
    {
        private static Dictionary<string, DoodadType> doodads;
        private static Dictionary<string, DoodadType> doodadsBaseEdited;
        private static Dictionary<string, DoodadType> doodadsCustom;

        public static List<DoodadType> GetAll()
        {
            List<DoodadType> list = new List<DoodadType>();
            var enumerator = doodads.GetEnumerator();
            while (enumerator.MoveNext())
            {
                DoodadType doodadType;
                var key = enumerator.Current.Key;
                if (doodadsBaseEdited.ContainsKey(key))
                {
                    doodadsBaseEdited.TryGetValue(key, out doodadType);
                    list.Add(doodadType);
                }
                else
                {
                    doodads.TryGetValue(key, out doodadType);
                    list.Add(doodadType);
                }
            }

            list.AddRange(doodadsCustom.Select(kvp => kvp.Value).ToList());

            return list;
        }

        internal static List<DoodadType> GetBase()
        {
            return doodads.Select(kvp => kvp.Value).ToList();
        }

        internal static DoodadType GetDoodadType(string doodcode)
        {
            DoodadType doodad;
            doodadsCustom.TryGetValue(doodcode, out doodad);

            if (doodad == null)
                doodadsBaseEdited.TryGetValue(doodcode, out doodad);

            if (doodad == null)
                doodads.TryGetValue(doodcode, out doodad);

            if (doodad == null)
                doodad = new DoodadType()
                {
                    DoodCode = doodcode,
                    DisplayName = "<Unknown Doodad>"
                };

            return doodad;
        }

        internal static string GetName(string doodcode)
        {
            DoodadType doodType = GetDoodadType(doodcode);
            if (doodType == null)
                return null;

            return doodType.DisplayName;
        }

        internal static void LoadFromGameStorage(bool isTest)
        {
            doodads = new Dictionary<string, DoodadType>();

            string text;

            if (!isTest && WarcraftStorageReader.GameVersion < new Version(1, 32))
            {
                LoadFromMpq();
                return;
            }

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
                using (Stream doodadskin = WarcraftStorageReader.OpenFile(@"doodads\doodadskins.txt"))
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
            }
        }

        private static void LoadFromMpq()
        {
            SylkParser sylkParser = new SylkParser();
            SylkTable table;
            using (Stream doodads = WarcraftStorageReader.OpenFile(@"doodads\doodads.slk"))
            {
                table = sylkParser.Parse(doodads);
            }

            var count = table.Count();
            for (int i = 1; i < count; i++)
            {
                var row = table.ElementAt(i);
                var doodad = new DoodadType()
                {
                    DoodCode = (string)row.GetValue(0),
                    Model = (string)row.GetValue(4),
                    DisplayName = Locale.Translate((string)row.GetValue(6)),
                };

                doodads.Add(doodad.DoodCode, doodad);
            }
        }

        internal static void Load(string fullMapPath)
        {
            doodadsBaseEdited = new Dictionary<string, DoodadType>();
            doodadsCustom = new Dictionary<string, DoodadType>();


            // Read custom doodad definition data
            string filePath = "war3map.w3d";
            if (!File.Exists(Path.Combine(fullMapPath, filePath)))
                return;

            DoodadObjectData customDoodads;
            customDoodads = CustomMapData.MPQMap.DoodadObjectData;
            if (customDoodads == null)
                return;

            for (int i = 0; i < customDoodads.BaseDoodads.Count; i++)
            {
                var dood = customDoodads.BaseDoodads[i];
                DoodadType baseDood = GetDoodadType(Int32Extensions.ToRawcode(dood.OldId));
                DoodadType doodad = new DoodadType()
                {
                    DoodCode = dood.ToString().Substring(0, 4),
                    DisplayName = baseDood.DisplayName,
                };
                doodadsBaseEdited.Add(doodad.DoodCode, doodad);
                SetCustomFields(dood, Int32Extensions.ToRawcode(dood.OldId));
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
                doodadsCustom.Add(doodad.DoodCode, doodad);
                SetCustomFields(dood, doodad.DoodCode);
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
