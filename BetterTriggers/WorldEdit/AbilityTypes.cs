using BetterTriggers.Models.War3Data;
using CASCLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using War3Net.Build.Extensions;
using War3Net.Common.Extensions;
using War3Net.IO.Slk;

namespace BetterTriggers.WorldEdit
{
    public class AbilityTypes
    {
        private static Dictionary<string, AbilityType> abilities;
        private static Dictionary<string, AbilityType> abilitiesBase;
        private static Dictionary<string, AbilityType> abilitiesCustom;

        internal static List<AbilityType> GetAll()
        {
            return abilities.Select(kvp => kvp.Value).ToList();
        }

        internal static List<AbilityType> GetBase()
        {
            return abilitiesBase.Select(kvp => kvp.Value).ToList();
        }

        internal static List<AbilityType> GetCustom()
        {
            return abilitiesCustom.Select(kvp => kvp.Value).ToList();
        }

        public static AbilityType GetAbilityType(string abilcode)
        {
            AbilityType abilType;
            abilities.TryGetValue(abilcode, out abilType);
            return abilType;
        }

        internal static string GetName(string abilcode)
        {
            return GetAbilityType(abilcode).DisplayName;
        }

        internal static void Load()
        {
            abilities = new Dictionary<string, AbilityType>();
            abilitiesBase = new Dictionary<string, AbilityType>();
            abilitiesCustom = new Dictionary<string, AbilityType>();

            var units = (CASCFolder)Casc.GetWar3ModFolder().Entries["units"];

            /* TODO:
             * We are loading too many abilities from this.
             * There are 'abilities' for 'Chaos Conversions' and other stuff
             * which are not actual abilites that show up in the object editor.
            */
            CASCFile abilityData = (CASCFile)units.Entries["abilitydata.slk"]; 
            var file = Casc.GetCasc().OpenFile(abilityData.FullName);
            SylkParser sylkParser = new SylkParser();
            SylkTable table = sylkParser.Parse(file);
            for (int i = 1; i < table.Count(); i++)
            {
                var row = table.ElementAt(i);
                AbilityType ability = new AbilityType()
                {
                    AbilCode = (string)row.GetValue(0),
                    DisplayName = Locale.Translate((string)row.GetValue(0)),
                };

                if (ability.AbilCode == null)
                    continue;

                abilities.TryAdd(ability.AbilCode, ability);
                abilitiesBase.TryAdd(ability.AbilCode, ability);
            }

            string filePath = "war3map.w3a";
            if (!File.Exists(Path.Combine(CustomMapData.mapPath, filePath)))
                return;

            while (CustomMapData.IsMapSaving())
            {
                Thread.Sleep(1000);
            }

            // Custom abilities
            using (Stream s = new FileStream(Path.Combine(CustomMapData.mapPath, filePath), FileMode.Open, FileAccess.Read))
            {
                BinaryReader reader = new BinaryReader(s);
                var customAbilities = War3Net.Build.Extensions.BinaryReaderExtensions.ReadAbilityObjectData(reader, true);

                for (int i = 0; i < customAbilities.NewAbilities.Count; i++)
                {
                    var customAbility = customAbilities.NewAbilities[i];

                    AbilityType baseAbil = GetAbilityType(Int32Extensions.ToRawcode(customAbility.OldId));
                    string name = baseAbil.DisplayName;
                    foreach (var modified in customAbility.Modifications)
                    {
                        if (Int32Extensions.ToRawcode(modified.Id) == "anam")
                            name = MapStrings.GetString(modified.ValueAsString);
                    }

                    var ability = new AbilityType()
                    {
                        AbilCode = customAbility.ToString().Substring(0, 4),
                        DisplayName = name,
                    };
                    abilities.TryAdd(ability.AbilCode, ability);
                    abilitiesCustom.TryAdd(ability.AbilCode, ability);
                }
            }
        }

    }
}
