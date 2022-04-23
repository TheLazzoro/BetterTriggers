using CASCLib;
using Model.War3Data;
using System.Collections.Generic;
using System.Linq;
using War3Net.IO.Slk;

namespace BetterTriggers.WorldEdit
{
    public class AbilityTypes
    {
        private static List<AbilityType> abilities;

        public static List<AbilityType> GetAbilitiesAll()
        {
            return abilities;
        }

        internal static void Load()
        {
            abilities = new List<AbilityType>();

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
            }
        }
    }
}
