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
        private static Dictionary<string, BuffType> buffsBaseEdited = new();
        private static Dictionary<string, BuffType> buffsCustom = new();

        internal static List<BuffType> GetAll()
        {
            List<BuffType> list = new List<BuffType>();
            var enumerator = buffs.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BuffType buffType;
                var key = enumerator.Current.Key;
                if (buffsBaseEdited.ContainsKey(key))
                {
                    buffsBaseEdited.TryGetValue(key, out buffType);
                    list.Add(buffType);
                }
                else
                {
                    buffs.TryGetValue(key, out buffType);
                    list.Add(buffType);
                }
            }

            list.AddRange(buffsCustom.Select(kvp => kvp.Value).ToList());

            return list;
        }

        public static BuffType GetBuffType(string buffcode)
        {
            BuffType buffType;
            buffsCustom.TryGetValue(buffcode, out buffType);

            if (buffType == null)
                buffsBaseEdited.TryGetValue(buffcode, out buffType);

            if (buffType == null)
                buffs.TryGetValue(buffcode, out buffType);

            if(buffType == null)
                buffType = new BuffType()
                {
                    BuffCode = buffcode,
                    DisplayName = "<Unknown Buff>"
                };

            return buffType;
        }

        internal static string GetName(string buffcode)
        {
            BuffType buffType = GetBuffType(buffcode);
            if (buffType == null)
                return "<Empty Name>";
            else if (buffType.DisplayName == null)
                return "<Empty Name>";

            string name = buffType.DisplayName;
            if (buffType.EditorSuffix != null)
                name += " " + buffType.EditorSuffix;

            return name;
        }

        internal static void LoadFromCASC(bool isTest)
        {
            buffs = new Dictionary<string, BuffType>();

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
            }

            buffdata.Close();
        }

        internal static void Load()
        {
            buffsBaseEdited = new Dictionary<string, BuffType>();
            buffsCustom = new Dictionary<string, BuffType>();

            BuffObjectData customBuffs;
            customBuffs = CustomMapData.MPQMap.BuffObjectData;
            if (customBuffs == null)
                return;

            for (int i = 0; i < customBuffs.BaseBuffs.Count; i++)
            {
                var buff = customBuffs.BaseBuffs[i];
                BuffType baseBuff = GetBuffType(Int32Extensions.ToRawcode(buff.OldId));
                var b = new BuffType()
                {
                    BuffCode = baseBuff.ToString().Substring(0, 4),
                    DisplayName = baseBuff.DisplayName,
                };
                buffsBaseEdited.TryAdd(b.BuffCode, b);
                SetCustomFields(buff, b.BuffCode);
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
                buffsCustom.TryAdd(buff.BuffCode, buff);
                SetCustomFields(customBuff, buff.BuffCode);
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
