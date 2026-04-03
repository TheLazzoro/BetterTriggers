using BetterTriggers.Containers;
using BetterTriggers.Models.War3Data;
using BetterTriggers.Utility;
using BetterTriggers.WorldEdit.GameDataReader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using War3Net.Build.Object;
using War3Net.Common.Extensions;
using War3Net.IO.Slk;

namespace BetterTriggers.WorldEdit
{
    public class DoodadTypes
    {
        private static Dictionary<string, DoodadType> doodads;
        private Dictionary<string, DoodadType> doodadsBaseEdited;
        private Dictionary<string, DoodadType> doodadsCustom;

        public List<DoodadType> GetAll()
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

        internal static DoodadType GetDoodadType(DoodadTypes? doodadTypes, string doodcode)
        {
            DoodadType? doodad = null;
            if (doodadTypes != null)
            {
                doodadTypes.doodadsCustom.TryGetValue(doodcode, out doodad);
                if (doodad == null)
                    doodadTypes.doodadsBaseEdited.TryGetValue(doodcode, out doodad);
            }

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

        internal static string GetName(DoodadTypes? doodadTypes, string doodcode)
        {
            DoodadType doodType = GetDoodadType(doodadTypes, doodcode);
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

        internal void Load(Project project, string fullMapPath)
        {
            doodadsBaseEdited = new Dictionary<string, DoodadType>();
            doodadsCustom = new Dictionary<string, DoodadType>();


            // Read custom doodad definition data
            string filePath = "war3map.w3d";
            if (!File.Exists(Path.Combine(fullMapPath, filePath)))
                return;

            DoodadObjectData customDoodads;
            customDoodads = project.MPQMap.DoodadObjectData;
            if (customDoodads == null)
                return;

            for (int i = 0; i < customDoodads.BaseDoodads.Count; i++)
            {
                var dood = customDoodads.BaseDoodads[i];
                DoodadType baseDood = GetDoodadType(project.DoodadTypes, Int32Extensions.ToRawcode(dood.OldId));
                DoodadType doodad = new DoodadType()
                {
                    DoodCode = dood.ToString().Substring(0, 4),
                    DisplayName = baseDood.DisplayName,
                };
                doodadsBaseEdited.Add(doodad.DoodCode, doodad);
                SetCustomFields(project, dood, Int32Extensions.ToRawcode(dood.OldId));
            }

            for (int i = 0; i < customDoodads.NewDoodads.Count; i++)
            {
                var dood = customDoodads.NewDoodads[i];
                DoodadType baseDood = GetDoodadType(project.DoodadTypes, Int32Extensions.ToRawcode(dood.OldId));
                DoodadType doodad = new DoodadType()
                {
                    DoodCode = dood.ToString().Substring(0, 4),
                    DisplayName = baseDood.DisplayName,
                };
                doodadsCustom.Add(doodad.DoodCode, doodad);
                SetCustomFields(project, dood, doodad.DoodCode);
            }
        }

        private void SetCustomFields(Project project, VariationObjectModification modified, string buffcode)
        {
            DoodadType doodType = GetDoodadType(project.DoodadTypes, buffcode);
            string displayName = doodType.DisplayName;
            foreach (var modification in modified.Modifications)
            {
                if (Int32Extensions.ToRawcode(modification.Id) == "dnam")
                    displayName = project.MapStrings.GetString(modification.ValueAsString);
            }
            doodType.DisplayName = displayName;
        }

    }
}
