using BetterTriggers.Locales;
using BetterTriggers.WorldEdit;

namespace BetterTriggers
{
    public class Init
    {
        private static bool hasLoaded;

        /// <summary>
        /// Loads all War3 data from CASC.
        /// </summary>
        public static void Initialize()
        {
            if (hasLoaded)
                return;

            Settings.Load();
            Locale.Load();
            Natives.Load();

            CustomMapData.Load();

            hasLoaded = true;
        }
    }
}
