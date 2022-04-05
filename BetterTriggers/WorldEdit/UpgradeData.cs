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
    public class UpgradeData
    {
        private static List<UpgradeType> upgrades;

        public static List<UpgradeType> GetUpgradesAll()
        {
            return upgrades;
        }

        internal static void Load()
        {
            upgrades = new List<UpgradeType>();

            var units = (CASCFolder)Casc.GetWar3ModFolder().Entries["units"];

            CASCFile abilityData = (CASCFile)units.Entries["upgradedata.slk"];
            var file = Casc.GetCasc().OpenFile(abilityData.FullName);
            SylkParser sylkParser = new SylkParser();
            SylkTable table = sylkParser.Parse(file);
            for (int i = 1; i < table.Count(); i++)
            {
                var row = table.ElementAt(i);
                UpgradeType upgrade = new UpgradeType()
                {
                    UpgradeCode = (string)row.GetValue(0),
                    DisplayName = (string)row.GetValue(1), // We want to replace this display name with locales
                };

                if (upgrade.UpgradeCode == null)
                    continue;

                upgrades.Add(upgrade);
            }
        }
    }
}
