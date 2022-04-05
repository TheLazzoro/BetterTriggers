using CASCLib;
using Model.War3Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using War3Net.IO.Slk;

namespace BetterTriggers.WorldEdit
{
    public class BuffData
    {
        private static List<BuffType> buffs;

        public static List<BuffType> GetBuffsAll()
        {
            return buffs;
        }

        internal static void Load()
        {
            buffs = new List<BuffType>();

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
            }
        }
    }
}
