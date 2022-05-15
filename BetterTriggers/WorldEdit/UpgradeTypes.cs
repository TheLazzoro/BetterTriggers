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
    public class UpgradeTypes
    {
        private static List<UpgradeType> upgrades;
        private static List<UpgradeType> upgradesBase;
        private static List<UpgradeType> upgradesCustom;

        internal static List<UpgradeType> GetAll()
        {
            return upgrades;
        }

        internal static List<UpgradeType> GetUpgradesBase()
        {
            return upgradesBase;
        }

        internal static List<UpgradeType> GetUpgradesCustom()
        {
            return upgradesCustom;
        }

        internal static void Load()
        {
            upgrades = new List<UpgradeType>();
            upgradesBase = new List<UpgradeType>();
            upgradesCustom = new List<UpgradeType>();

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
                upgradesBase.Add(upgrade);
            }

            string filePath = "war3map.w3q";
            if (!File.Exists(Path.Combine(CustomMapData.mapPath, filePath)))
                return;

            while (CustomMapData.IsMapSaving())
            {
                Thread.Sleep(1000);
            }

            // Custom upgrades
            using (Stream s = new FileStream(Path.Combine(CustomMapData.mapPath, filePath), FileMode.Open, FileAccess.Read))
            {
                BinaryReader reader = new BinaryReader(s);
                var customUpgrades = BinaryReaderExtensions.ReadUpgradeObjectData(reader, true);

                for (int i = 0; i < customUpgrades.NewUpgrades.Count; i++)
                {
                    var customUpgrade = customUpgrades.NewUpgrades[i];
                    var upgrade = new UpgradeType()
                    {
                        UpgradeCode = customUpgrade.ToString(),
                    };
                    upgrades.Add(upgrade);
                    upgradesCustom.Add(upgrade);
                }
            }
        }
    }
}
