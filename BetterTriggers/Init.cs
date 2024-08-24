using BetterTriggers.WorldEdit;
using System;

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
            EditorSettings.Load();
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
            UnitTypes.LoadFromGameStorage(isTest);
            NextData = "Abilities";
            OnNextData?.Invoke();
            AbilityTypes.LoadFromGameStorage(isTest);
            NextData = "Buffs";
            OnNextData?.Invoke();
            BuffTypes.LoadFromGameStorage(isTest);
            NextData = "Destructibles";
            OnNextData?.Invoke();
            DestructibleTypes.LoadFromGameStorage(isTest);
            NextData = "Doodads";
            OnNextData?.Invoke();
            DoodadTypes.LoadFromGameStorage(isTest);
            NextData = "Items";
            OnNextData?.Invoke();
            ItemTypes.LoadFromGameStorage(isTest);
            NextData = "Upgrades";
            OnNextData?.Invoke();
            UpgradeTypes.LoadFromGameStorage(isTest);

            NextData = "Misc Data";
            OnNextData?.Invoke();
            ModelData.Load(isTest); // requires above

            HasLoaded = true;
        }
    }
}
