using CASCLib;
using Model.War3Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using War3Net.Build.Extensions;
using War3Net.IO.Slk;

namespace BetterTriggers.WorldEdit
{
    public class DestructibleTypes
    {
        private static List<DestructibleType> destructibles;

        public static List<DestructibleType> GetDestructiblesTypesAll()
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

            // Read custom destructible definition data
            string path = @"C:\Users\Lasse Dam\Desktop\test2.w3x\war3map.w3b";
            if (!File.Exists(path))
                return;

            Stream s = new FileStream(path, FileMode.Open);
            BinaryReader reader = new BinaryReader(s);
            var customDestructibles = BinaryReaderExtensions.ReadMapDestructableObjectData(reader);

            for (int i = 0; i < customDestructibles.NewDestructables.Count; i++)
            {
                var dest = customDestructibles.NewDestructables[i];
                DestructibleType destructible = new DestructibleType()
                {
                    DestCode = dest.ToString().Substring(0, 4),
                    DisplayName = dest.ToString(), // We want to replace this display name with locales
                };
                destructibles.Add(destructible);
            }
        }
    }
}
