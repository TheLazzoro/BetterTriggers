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
using System.Windows.Documents;
using War3Net.Build.Extensions;
using War3Net.Build.Object;
using War3Net.Common.Extensions;
using War3Net.IO.Slk;
using static System.Net.Mime.MediaTypeNames;

namespace BetterTriggers.WorldEdit
{
    public class DestructibleTypes
    {
        private static Dictionary<string, DestructibleType> destructibles;
        private static Dictionary<string, DestructibleType> destructiblesBaseEdited;
        private static Dictionary<string, DestructibleType> destructiblesCustom;

        public static List<DestructibleType> GetAll()
        {
            List<DestructibleType> list = new List<DestructibleType>();
            if (destructibles == null)
                return list;

            var enumerator = destructibles.GetEnumerator();
            while (enumerator.MoveNext())
            {
                DestructibleType destType;
                var key = enumerator.Current.Key;
                if (destructiblesBaseEdited.ContainsKey(key))
                {
                    destructiblesBaseEdited.TryGetValue(key, out destType);
                    list.Add(destType);
                }
                else
                {
                    destructibles.TryGetValue(key, out destType);
                    list.Add(destType);
                }
            }

            list.AddRange(destructiblesCustom.Select(kvp => kvp.Value).ToList());

            return list;
        }

        internal static List<DestructibleType> GetBase()
        {
            return destructibles.Select(kvp => kvp.Value).ToList();
        }

        public static DestructibleType GetDestType(string destcode)
        {
            DestructibleType destType;
            destructiblesCustom.TryGetValue(destcode, out destType);

            if (destType == null)
                destructiblesBaseEdited.TryGetValue(destcode, out destType);

            if (destType == null)
                destructibles.TryGetValue(destcode, out destType);

            if (destType == null)
                destType = new DestructibleType()
                {
                    DestCode = destcode,
                    DisplayName = "<Unknown Destructible>"
                };

            return destType;
        }

        // TODO: Doesn't return comments with the name (e.g. 'Diagonal 1' or 'Vertical')
        internal static string GetName(string destcode)
        {
            DestructibleType destType = GetDestType(destcode);
            if (destType == null)
                return null;

            string name = destType.DisplayName;
            if (destType.EditorSuffix != null)
                name += " " + destType.EditorSuffix;

            return name;
        }

        internal static void LoadFromGameStorage(bool isTest)
        {
            destructibles = new Dictionary<string, DestructibleType>();

            SylkParser sylkParser = new SylkParser();
            SylkTable table;
            StreamReader reader;
            string text;

            if (!isTest && WarcraftStorageReader.GameVersion < new Version(1, 32))
            {
                LoadFromMpq();
                return;
            }

            if (isTest)
            {
                using (Stream destructibleskin = new FileStream(Path.Combine(Directory.GetCurrentDirectory(), "TestResources/destructableskin.txt"), FileMode.Open))
                using (Stream destructibleDataSLK = new FileStream(Path.Combine(Directory.GetCurrentDirectory(), "TestResources/destructabledata.slk"), FileMode.Open))
                {
                    table = sylkParser.Parse(destructibleDataSLK); ;
                    reader = new StreamReader(destructibleskin);
                    text = reader.ReadToEnd();
                }
            }
            else
            {
                using (Stream destructibleskin = WarcraftStorageReader.OpenFile(@"units\destructableskin.txt"))
                using (Stream destructibleDataSLK = WarcraftStorageReader.OpenFile(@"units\destructabledata.slk"))
                {
                    table = sylkParser.Parse(destructibleDataSLK);
                    reader = new StreamReader(destructibleskin);
                    text = reader.ReadToEnd();
                }
            }


            int count = table.Count();
            for (int i = 1; i < count; i++)
            {
                var row = table.ElementAt(i);
                DestructibleType destType = new DestructibleType()
                {
                    DestCode = (string)row.GetValue(0),
                    DisplayName = Locale.Translate((string)row.GetValue(6)),
                    EditorSuffix = Locale.Translate((string)row.GetValue(7)),
                };

                destructibles.TryAdd(destType.DestCode, destType);
            }

            // Add 'model' from 'destructableskin.txt'

            var data = IniFileConverter.GetIniData(text);
            var destTypesList = GetBase();
            for (int i = 0; i < destTypesList.Count; i++)
            {
                var destType = destTypesList[i];
                var section = data[destType.DestCode];
                string model = section["file"];
                destType.Model = model;
            }
        }

        private static void LoadFromMpq()
        {
            SylkParser sylkParser = new SylkParser();
            SylkTable table;
            StreamReader reader;

            using (Stream destructibleDataSLK = WarcraftStorageReader.OpenFile(@"units\destructabledata.slk"))
            {
                table = sylkParser.Parse(destructibleDataSLK);
            }

            int count = table.Count();
            for (int i = 1; i < count; i++)
            {
                var row = table.ElementAt(i);
                DestructibleType destType = new DestructibleType()
                {
                    DestCode = (string)row.GetValue(0),
                    Model = (string)row.GetValue(4),
                    DisplayName = Locale.Translate((string)row.GetValue(10)),
                    EditorSuffix = Locale.Translate((string)row.GetValue(11)),
                };

                destructibles.TryAdd(destType.DestCode, destType);
            }
        }

        internal static void Load()
        {
            destructiblesBaseEdited = new Dictionary<string, DestructibleType>();
            destructiblesCustom = new Dictionary<string, DestructibleType>();

            DestructableObjectData customDestructibles;
            customDestructibles = CustomMapData.MPQMap.DestructableObjectData;
            if (customDestructibles == null)
                return;

            for (int i = 0; i < customDestructibles.BaseDestructables.Count; i++)
            {
                var dest = customDestructibles.BaseDestructables[i];
                DestructibleType baseDest = GetDestType(Int32Extensions.ToRawcode(dest.OldId));
                string name = baseDest.DisplayName;
                DestructibleType destructible = new DestructibleType()
                {
                    DestCode = dest.ToString().Substring(0, 4),
                    DisplayName = name,
                };
                destructiblesBaseEdited.TryAdd(destructible.DestCode, destructible);
                SetCustomFields(dest, Int32Extensions.ToRawcode(dest.OldId));
            }

            for (int i = 0; i < customDestructibles.NewDestructables.Count; i++)
            {
                var dest = customDestructibles.NewDestructables[i];
                DestructibleType baseDest = GetDestType(Int32Extensions.ToRawcode(dest.OldId));
                string name = baseDest.DisplayName;
                DestructibleType destructible = new DestructibleType()
                {
                    DestCode = dest.ToString().Substring(0, 4),
                    DisplayName = name,
                };
                destructiblesCustom.TryAdd(destructible.DestCode, destructible);
                SetCustomFields(dest, destructible.DestCode);
            }
        }

        private static void SetCustomFields(SimpleObjectModification modified, string buffcode)
        {
            DestructibleType buffType = GetDestType(buffcode);
            string displayName = buffType.DisplayName;
            string editorSuffix = buffType.EditorSuffix;
            foreach (var modification in modified.Modifications)
            {
                if (Int32Extensions.ToRawcode(modification.Id) == "bnam")
                    displayName = MapStrings.GetString(modification.ValueAsString);
                else if (Int32Extensions.ToRawcode(modification.Id) == "bsuf")
                    editorSuffix = MapStrings.GetString(modification.ValueAsString);
            }
            buffType.DisplayName = displayName;
            buffType.EditorSuffix = editorSuffix;
        }
    }
}
