using CASCLib;
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
    public class BuffTypes
    {
        private static List<BuffType> buffs;
        private static List<BuffType> buffsBase;
        private static List<BuffType> buffsCustom;

        internal static List<BuffType> GetAll()
        {
            return buffs;
        }

        internal static List<BuffType> GetBase()
        {
            return buffsBase;
        }

        internal static List<BuffType> GetCustom()
        {
            return buffsCustom;
        }

        internal static void Load()
        {
            buffs = new List<BuffType>();
            buffsBase = new List<BuffType>();
            buffsCustom = new List<BuffType>();

            var units = (CASCFolder)Casc.GetWar3ModFolder().Entries["units"];

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
                    DisplayName = (string)row.GetValue(2), // We want to replace this display name with locales
                };

                if (buff.BuffCode == null)
                    continue;

                buffs.Add(buff);
                buffsBase.Add(buff);
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
                var customBuffs = BinaryReaderExtensions.ReadBuffObjectData(reader, true);

                for (int i = 0; i < customBuffs.NewBuffs.Count; i++)
                {
                    var customBuff = customBuffs.NewBuffs[i];
                    var buff = new BuffType()
                    {
                        BuffCode = customBuff.ToString(),
                    };
                    buffs.Add(buff);
                    buffsCustom.Add(buff);
                }
            }
        }
    }
}
