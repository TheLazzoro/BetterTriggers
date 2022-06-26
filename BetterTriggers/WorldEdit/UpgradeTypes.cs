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
    public class UpgradeTypes
    {
        private static Dictionary<string, UpgradeType> upgrades;
        private static Dictionary<string, UpgradeType> upgradesBase;
        private static Dictionary<string, UpgradeType> upgradesCustom;

        internal static List<UpgradeType> GetAll()
        {
            return upgrades.Select(kvp => kvp.Value).ToList();
        }

        internal static List<UpgradeType> GetUpgradesBase()
        {
            return upgradesBase.Select(kvp => kvp.Value).ToList();
        }

        internal static List<UpgradeType> GetUpgradesCustom()
        {
            return upgradesCustom.Select(kvp => kvp.Value).ToList();
        }

        public static UpgradeType GetUpgradeType(string upgradecode)
        {
            UpgradeType upgradeType;
            upgrades.TryGetValue(upgradecode, out upgradeType);
            return upgradeType;
        }

        internal static string GetName(string upgradecode)
        {
            return GetUpgradeType(upgradecode).DisplayName;
        }

        internal static void Load()
        {
            upgrades = new Dictionary<string, UpgradeType>();
            upgradesBase = new Dictionary<string, UpgradeType>();
            upgradesCustom = new Dictionary<string, UpgradeType>();

            var units = (CASCFolder)Casc.GetWar3ModFolder().Entries["units"];

            /* TODO:
             * We are loading too many upgrades from this.
             * There are 'upgrades' for 'Chaos Conversions' and other stuff
             * which are not actual upgrades that show up in the object editor.
            */
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
                    DisplayName = Locale.Translate((string)row.GetValue(0)),
                };

                if (upgrade.UpgradeCode == null)
                    continue;

                upgrades.TryAdd(upgrade.UpgradeCode, upgrade);
                upgradesBase.TryAdd(upgrade.UpgradeCode, upgrade);
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
                var customUpgrades = War3Net.Build.Extensions.BinaryReaderExtensions.ReadUpgradeObjectData(reader, true);

                for (int i = 0; i < customUpgrades.NewUpgrades.Count; i++)
                {
                    var customUpgrade = customUpgrades.NewUpgrades[i];

                    UpgradeType baseUpgrade = GetUpgradeType(Int32Extensions.ToRawcode(customUpgrade.OldId));
                    string name = baseUpgrade.DisplayName;
                    foreach (var modified in customUpgrade.Modifications)
                    {
                        if (Int32Extensions.ToRawcode(modified.Id) == "gnam")
                            name = MapStrings.GetString(modified.ValueAsString);
                    }

                    var upgrade = new UpgradeType()
                    {
                        UpgradeCode = customUpgrade.ToString().Substring(0, 4),
                        DisplayName = name,
                    };
                    upgrades.TryAdd(upgrade.UpgradeCode, upgrade);
                    upgradesCustom.TryAdd(upgrade.UpgradeCode, upgrade);
                }
            }
        }

    }
}
