using BetterTriggers.Locales;
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

        /// <summary>
        /// Loads all War3 data from CASC into memory.
        /// </summary>
        public static void Initialize()
        {
            if (hasLoaded)
                return;

            Settings.Load();
            Locale.Load();
            Natives.Load();
            UnitTypes.Load();
            AbilityTypes.Load();
            BuffData.Load();
            DestructibleTypes.Load();
            UpgradeTypes.Load();
            ItemData.Load();

            hasLoaded = true;
        }
    }
}
