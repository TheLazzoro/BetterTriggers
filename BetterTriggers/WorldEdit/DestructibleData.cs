using CASCLib;
using Model.War3Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using War3Net.IO.Slk;

namespace BetterTriggers.WorldEdit
{
    public class DestructibleData
    {
        private static List<DestructibleType> destructibles;

        public static List<DestructibleType> GetDestructiblesAll()
        {
            return destructibles;
        }

        internal static void Load()
        {
            destructibles = new List<DestructibleType>();

            var units = (CASCFolder)Casc.GetWar3ModFolder().Entries["units"];

            CASCFile abilityData = (CASCFile)units.Entries["destructabledata.slk"];
            var file = Casc.GetCasc().OpenFile(abilityData.FullName);
            SylkParser sylkParser = new SylkParser();
            SylkTable table = sylkParser.Parse(file);
            for (int i = 1; i < table.Count(); i++)
            {
                var row = table.ElementAt(i);
                DestructibleType destructible = new DestructibleType()
                {
                    DestCode = (string)row.GetValue(0),
                    DisplayName = (string)row.GetValue(6), // We want to replace this display name with locales
                };

                if (destructible.DestCode == null)
                    continue;

                destructibles.Add(destructible);
            }
        }
    }
}
