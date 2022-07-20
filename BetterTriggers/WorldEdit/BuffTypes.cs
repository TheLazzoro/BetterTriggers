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

            return buffType.DisplayName;
        }

        internal static void Load()
        {
            buffs = new Dictionary<string, BuffType>();
            buffsBase = new Dictionary<string, BuffType>();
            buffsCustom = new Dictionary<string, BuffType>();

            var units = (CASCFolder)Casc.GetWar3ModFolder().Entries["units"];

            /* TODO:
             * We are loading too many buffs from this.
             * There are 'buffs' for other stuff which are not
             * actual buffs that show up in the object editor.
            */
            CASCFile abilityData = (CASCFile)units.Entries["abilitybuffdata.slk"];
            var file = Casc.GetCasc().OpenFile(abilityData.FullName);
            SylkParser sylkParser = new SylkParser();
            SylkTable table = sylkParser.Parse(file);
            for (int i = 1; i < table.Count(); i++)
            {
                var row = table.ElementAt(i);
                BuffType buff = new BuffType()
                {
                    BuffCode = (string)row.GetValue(0),
                    DisplayName = Locale.Translate((string)row.GetValue(0)),
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

                for (int i = 0; i < customBuffs.NewBuffs.Count; i++)
                {
                    var customBuff = customBuffs.NewBuffs[i];

                    BuffType baseBuff = GetBuffType(Int32Extensions.ToRawcode(customBuff.OldId));
                    string name = baseBuff.DisplayName;
                    foreach (var modified in customBuff.Modifications)
                    {
                        if (Int32Extensions.ToRawcode(modified.Id) == "ftip")
                            name = MapStrings.GetString(modified.ValueAsString);
                    }

                    var buff = new BuffType()
                    {
                        BuffCode = customBuff.ToString().Substring(0, 4),
                        DisplayName = name,
                    };
                    buffs.TryAdd(buff.BuffCode, buff);
                    buffsCustom.TryAdd(buff.BuffCode, buff);
                }
            }
        }

    }
}
