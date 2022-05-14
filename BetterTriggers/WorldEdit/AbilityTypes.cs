using CASCLib;
using Model.War3Data;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using War3Net.Build.Extensions;
using War3Net.IO.Slk;

namespace BetterTriggers.WorldEdit
{
    public class AbilityTypes
    {
        private static List<AbilityType> abilities;
        private static List<AbilityType> abilitiesBase;
        private static List<AbilityType> abilitiesCustom;

        internal static List<AbilityType> GetAll()
        {
            return abilities;
        }

        internal static List<AbilityType> GetBase()
        {
            return abilitiesBase;
        }

        internal static List<AbilityType> GetCustom()
        {
            return abilitiesCustom;
        }

        internal static void Load()
        {
            abilities = new List<AbilityType>();
            abilitiesBase = new List<AbilityType>();
            abilitiesCustom = new List<AbilityType>();

            var units = (CASCFolder)Casc.GetWar3ModFolder().Entries["units"];

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
                    DisplayName = (string)row.GetValue(2), // We want to replace this display name with locales
                };

                if (ability.AbilCode == null)
                    continue;

                abilities.Add(ability);
                abilitiesBase.Add(ability);
            }

            // Custom abilities
            Stream s = new FileStream(Path.Combine(CustomMapData.mapPath, "war3map.w3a"), FileMode.Open);
            BinaryReader reader = new BinaryReader(s);
            var customAbilities = BinaryReaderExtensions.ReadAbilityObjectData(reader, true);

            for (int i = 0; i < customAbilities.NewAbilities.Count; i++)
            {
                var customAbility = customAbilities.NewAbilities[i];
                var ability = new AbilityType()
                {
                    AbilCode = customAbility.ToString(),
                };
                abilities.Add(ability);
                abilitiesCustom.Add(ability);
            }
        }
    }
}
