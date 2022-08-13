using BetterTriggers.Models.War3Data;
using CASCLib;
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
    public class BuffTypes
    {
        private static Dictionary<string, BuffType> buffs;
        private static Dictionary<string, BuffType> buffsBase;
        private static Dictionary<string, BuffType> buffsCustom;

        internal static List<BuffType> GetAll()
        {
            return buffs.Select(kvp => kvp.Value).ToList();
        }

        internal static List<BuffType> GetBase()
        {
            return buffsBase.Select(kvp => kvp.Value).ToList();
        }

        internal static List<BuffType> GetCustom()
        {
            return buffsCustom.Select(kvp => kvp.Value).ToList();
        }

        public static BuffType GetBuffType(string buffcode)
        {
            BuffType buffType;
            buffs.TryGetValue(buffcode, out buffType);
            return buffType;
        }

        internal static string GetName(string buffcode)
        {
            BuffType buffType = GetBuffType(buffcode);
            if (buffType == null)
                return null;

            string name = buffType.DisplayName;
            if (buffType.EditorSuffix != null)
                name += " " + buffType.EditorSuffix;

            return name;
        }

        internal static void Load(bool isTest = false)
        {
            buffs = new Dictionary<string, BuffType>();
            buffsBase = new Dictionary<string, BuffType>();
            buffsCustom = new Dictionary<string, BuffType>();

            Stream buffdata;

            if (isTest)
            {
                buffdata = new FileStream(Path.Combine(Directory.GetCurrentDirectory(), "TestResources/abilitybuffdata.slk"), FileMode.Open);
            }
            else
            {
                var units = (CASCFolder)Casc.GetWar3ModFolder().Entries["units"];
                /* TODO:
                 * We are loading too many buffs from this.
                 * There are 'buffs' for other stuff which are not
                 * actual buffs that show up in the object editor.
                */
                CASCFile abilityData = (CASCFile)units.Entries["abilitybuffdata.slk"];
                buffdata = Casc.GetCasc().OpenFile(abilityData.FullName);
            }

            SylkParser sylkParser = new SylkParser();
            SylkTable table = sylkParser.Parse(buffdata);
            int count = table.Count();
            for (int i = 1; i < count; i++)
            {
                var row = table.ElementAt(i);
                string buffcode = (string)row.GetValue(0);
                BuffType buff = new BuffType()
                {
                    BuffCode = buffcode,
                    DisplayName = Locale.GetDisplayName(buffcode),
                    EditorSuffix = Locale.GetEditorSuffix(buffcode),
                };

                if (buff.BuffCode == null)
                    continue;

                buffs.TryAdd(buff.BuffCode, buff);
                buffsBase.TryAdd(buff.BuffCode, buff);
            }

            string filePath = "war3map.w3h";
            if (!File.Exists(Path.Combine(CustomMapData.mapPath, filePath)))
                return;

            while (CustomMapData.IsMapSaving())
            {
                Thread.Sleep(1000);
            }

            // Custom buffs
            using (Stream s = new FileStream(Path.Combine(CustomMapData.mapPath, filePath), FileMode.Open, FileAccess.Read))
            {
                BinaryReader reader = new BinaryReader(s);
                var customBuffs = War3Net.Build.Extensions.BinaryReaderExtensions.ReadBuffObjectData(reader, true);

                for (int i = 0; i < customBuffs.BaseBuffs.Count; i++)
                {
                    var baseBuff = customBuffs.BaseBuffs[i];
                    SetCustomFields(baseBuff, Int32Extensions.ToRawcode(baseBuff.OldId));
                }
                
                for (int i = 0; i < customBuffs.NewBuffs.Count; i++)
                {
                    var customBuff = customBuffs.NewBuffs[i];
                    BuffType baseBuff = GetBuffType(Int32Extensions.ToRawcode(customBuff.OldId));
                    string name = baseBuff.DisplayName;
                    var buff = new BuffType()
                    {
                        BuffCode = customBuff.ToString().Substring(0, 4),
                        DisplayName = name,
                    };
                    buffs.TryAdd(buff.BuffCode, buff);
                    buffsCustom.TryAdd(buff.BuffCode, buff);
                    SetCustomFields(customBuff, buff.BuffCode);
                }
            }
        }

        private static void SetCustomFields(SimpleObjectModification modified, string buffcode)
        {
            BuffType buffType = GetBuffType(buffcode);
            string displayName = buffType.DisplayName;
            string editorSuffix = buffType.EditorSuffix;

            foreach (var modification in modified.Modifications)
            {
                if (Int32Extensions.ToRawcode(modification.Id) == "ftip")
                    displayName = MapStrings.GetString(modification.ValueAsString);
                else if (Int32Extensions.ToRawcode(modification.Id) == "fnam")
                    displayName = MapStrings.GetString(modification.ValueAsString);
                else if (Int32Extensions.ToRawcode(modification.Id) == "fnsf")
                    editorSuffix = MapStrings.GetString(modification.ValueAsString);
            }

            buffType.DisplayName = displayName;
            buffType.EditorSuffix = editorSuffix;
        }
    }
}
