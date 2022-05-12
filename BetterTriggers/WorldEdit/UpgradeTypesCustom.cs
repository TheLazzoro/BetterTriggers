using Model.War3Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using War3Net.Build.Extensions;

namespace BetterTriggers.WorldEdit
{
    internal class UpgradeTypesCustom
    {
        private static List<UpgradeType> upgradeTypes;
        
        internal static List<UpgradeType> Load( )
        {
            upgradeTypes = new List<UpgradeType>();
            Stream s = new FileStream(Path.Combine(CustomMapData.mapPath, "war3map.w3q"), FileMode.Open);
            BinaryReader reader = new BinaryReader(s);
            var customUpgrades = BinaryReaderExtensions.ReadUpgradeObjectData(reader, true);

            for(int i = 0; i < customUpgrades.NewUpgrades.Count; i++)
            {
                var customUpgrade = customUpgrades.NewUpgrades[i];
                upgradeTypes.Add(new UpgradeType()
                {
                    UpgradeCode = customUpgrade.ToString(),
                });
            }

            return upgradeTypes;
        }
    }
}
