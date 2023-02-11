using BetterTriggers.Utility;
using BetterTriggers.WorldEdit;
using System;
using System.IO;

namespace BetterTriggers
{
    public class Init
    {
        public static bool HasLoaded { get; set; }
        public static event Action OnNextData;
        public static string NextData;

        /// <summary>
        /// Loads all War3 data from CASC.
        /// </summary>
        public static void Initialize(bool isTest)
        {
            if (HasLoaded)
                return;

            NextData = "Settings";
            OnNextData?.Invoke();
            Settings.Load();
            NextData = "Locale";
            OnNextData?.Invoke();
            Locale.Load();
            NextData = "Trigger Data";
            OnNextData?.Invoke();
            TriggerData.Load(isTest);
            NextData = "Script Data";
            OnNextData?.Invoke();
            ScriptData.Load(isTest);

            NextData = "Units";
            OnNextData?.Invoke();
            UnitTypes.LoadFromCASC(isTest);
            NextData = "Abilities";
            OnNextData?.Invoke();
            AbilityTypes.LoadFromCASC(isTest);
            NextData = "Buffs";
            OnNextData?.Invoke();
            BuffTypes.LoadFromCASC(isTest);
            NextData = "Destructibles";
            OnNextData?.Invoke();
            DestructibleTypes.LoadFromCASC(isTest);
            NextData = "Doodads";
            OnNextData?.Invoke();
            DoodadTypes.LoadFromCASC(isTest);
            NextData = "Items";
            OnNextData?.Invoke();
            ItemTypes.LoadFromCASC(isTest);
            NextData = "Upgrades";
            OnNextData?.Invoke();
            UpgradeTypes.LoadFromCASC(isTest);

            NextData = "Misc Data";
            OnNextData?.Invoke();
            ModelData.Load(isTest); // requires above

            HasLoaded = true;
        }
    }
}
