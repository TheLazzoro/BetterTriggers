using BetterTriggers.Models.War3Data;
using BetterTriggers.WorldEdit.GameDataReader;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using War3Net.Build.Object;
using War3Net.Common.Extensions;
using War3Net.IO.Slk;

namespace BetterTriggers.WorldEdit
{
    public class AbilityTypes
    {
        private static Dictionary<string, AbilityType> abilities;
        private static Dictionary<string, AbilityType> abilitiesBaseEdited = new();
        private static Dictionary<string, AbilityType> abilitiesCustom = new();

        public static List<AbilityType> GetAll()
        {
            List<AbilityType> list = new List<AbilityType>();
            var enumerator = abilities.GetEnumerator();
            while (enumerator.MoveNext())
            {
                AbilityType abilityType;
                var key = enumerator.Current.Key;
                if (abilitiesBaseEdited.ContainsKey(key))
                {
                    abilitiesBaseEdited.TryGetValue(key, out abilityType);
                    list.Add(abilityType);
                }
                else
                {
                    abilities.TryGetValue(key, out abilityType);
                    list.Add(abilityType);
                }
            }

            list.AddRange(abilitiesCustom.Select(kvp => kvp.Value).ToList());

            return list;
        }

        public static AbilityType GetAbilityType(string abilcode)
        {
            AbilityType abilType;
            abilitiesCustom.TryGetValue(abilcode, out abilType);

            if (abilType == null)
                abilitiesBaseEdited.TryGetValue(abilcode, out abilType);

            if (abilType == null)
                abilities.TryGetValue(abilcode, out abilType);

            if (abilType == null)
                abilType = new AbilityType()
                {
                    AbilCode = abilcode,
                    DisplayName = "<Unknown Ability>"
                };

            return abilType;
        }

        internal static string GetName(string abilcode)
        {
            AbilityType abilityType = GetAbilityType(abilcode);
            if (abilityType == null)
                return "<Empty Name>";
            else if (abilityType.DisplayName == null)
                return "<Empty Name>";

            string name = abilityType.DisplayName;
            if (abilityType.EditorSuffix != null)
                name += " " + abilityType.EditorSuffix;

            return name;
        }

        internal static void LoadFromGameStorage(bool isTest)
        {
            abilities = new Dictionary<string, AbilityType>();

            Stream abilitydata;
            if (isTest)
            {
                abilitydata = new FileStream(Path.Combine(Directory.GetCurrentDirectory(), "TestResources/abilitydata.slk"), FileMode.Open);
            }
            else
            {
                abilitydata = WarcraftStorageReader.OpenFile(@"units\abilitydata.slk");
            }

            SylkParser sylkParser = new SylkParser();
            SylkTable table = sylkParser.Parse(abilitydata);
            int count = table.Count();
            for (int i = 1; i < count; i++)
            {
                var row = table.ElementAt(i);
                string abilcode = (string)row.GetValue(0);
                AbilityType ability = new AbilityType()
                {
                    AbilCode = abilcode,
                    DisplayName = Locale.GetDisplayName(abilcode),
                    EditorSuffix = Locale.GetEditorSuffix(abilcode),
                };

                if (ability.AbilCode == null)
                    continue;

                abilities.TryAdd(ability.AbilCode, ability);
            }
        }

        internal static void Load()
        {
            abilitiesCustom = new Dictionary<string, AbilityType>();
            abilitiesBaseEdited = new Dictionary<string, AbilityType>();

            var customAbilities = CustomMapData.MPQMap.AbilityObjectData;
            if (customAbilities == null)
                return;

            for (int i = 0; i < customAbilities.BaseAbilities.Count; i++)
            {
                var baseAbility = customAbilities.BaseAbilities[i];
                AbilityType baseAbil = GetAbilityType(Int32Extensions.ToRawcode(baseAbility.OldId));
                var ability = new AbilityType()
                {
                    AbilCode = baseAbility.ToString().Substring(0, 4),
                    DisplayName = baseAbil.DisplayName,
                    EditorSuffix = baseAbil.EditorSuffix,
                };
                abilitiesBaseEdited.TryAdd(ability.AbilCode, ability);
                SetCustomFields(baseAbility, Int32Extensions.ToRawcode(baseAbility.OldId));
            }

            for (int i = 0; i < customAbilities.NewAbilities.Count; i++)
            {
                var customAbility = customAbilities.NewAbilities[i];
                AbilityType baseAbil = GetAbilityType(Int32Extensions.ToRawcode(customAbility.OldId));
                string name = baseAbil.DisplayName;
                string editorSuffix = baseAbil.EditorSuffix;
                var ability = new AbilityType()
                {
                    AbilCode = customAbility.ToString().Substring(0, 4),
                    DisplayName = name,
                    EditorSuffix = editorSuffix,
                };
                abilitiesCustom.TryAdd(ability.AbilCode, ability);
                SetCustomFields(customAbility, ability.AbilCode);
            }
        }

        private static void SetCustomFields(LevelObjectModification modified, string abilcode)
        {
            AbilityType abilityType = GetAbilityType(abilcode);
            string displayName = abilityType.DisplayName;
            string editorSuffix = abilityType.EditorSuffix;

            foreach (var modification in modified.Modifications)
            {
                if (Int32Extensions.ToRawcode(modification.Id) == "anam")
                    displayName = MapStrings.GetString(modification.ValueAsString);
                else if (Int32Extensions.ToRawcode(modification.Id) == "ansf")
                    editorSuffix = MapStrings.GetString(modification.ValueAsString);
            }

            abilityType.DisplayName = displayName;
            abilityType.EditorSuffix = editorSuffix;
        }
    }
}
