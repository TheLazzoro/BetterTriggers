using BetterTriggers.Utility;
using BetterTriggers.WorldEdit;
using System.IO;

namespace BetterTriggers
{
    public class Init
    {
        public static bool HasLoaded { get; set; }

        /// <summary>
        /// Loads all War3 data from CASC.
        /// </summary>
        public static void Initialize(bool isTest)
        {
            if (HasLoaded)
                return;

            Settings.Load();
            Locale.Load();
            TriggerData.Load(isTest);
            ScriptData.Load(isTest);

            UnitTypes.LoadFromCASC(isTest);
            AbilityTypes.LoadFromCASC(isTest);
            BuffTypes.LoadFromCASC(isTest);
            DestructibleTypes.LoadFromCASC(isTest);
            DoodadTypes.LoadFromCASC(isTest);
            ItemTypes.LoadFromCASC(isTest);
            UpgradeTypes.LoadFromCASC(isTest);

            ModelData.Load(isTest); // requires above

            HasLoaded = true;
        }
    }
}
