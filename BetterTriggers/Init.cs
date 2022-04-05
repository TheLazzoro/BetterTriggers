using BetterTriggers.WorldEdit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers
{
    public class Init
    {
        private static bool hasLoaded;

        public static void Initialize()
        {
            if (hasLoaded)
                return;

            UnitData.Load();
            AbilityData.Load();
            BuffData.Load();
            DestructibleData.Load();
            UpgradeData.Load();
            ItemData.Load();

            hasLoaded = true;
        }
    }
}
