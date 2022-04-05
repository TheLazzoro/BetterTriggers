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
    public class ItemData
    {
        private static List<ItemType> items;

        public static List<ItemType> GetItemsAll()
        {
            return items;
        }

        internal static void Load()
        {
            items = new List<ItemType>();

            var units = (CASCFolder)Casc.GetWar3ModFolder().Entries["units"];

            CASCFile abilityData = (CASCFile)units.Entries["itemdata.slk"];
            var file = Casc.GetCasc().OpenFile(abilityData.FullName);
            SylkParser sylkParser = new SylkParser();
            SylkTable table = sylkParser.Parse(file);
            for (int i = 1; i < table.Count(); i++)
            {
                var row = table.ElementAt(i);
                ItemType buff = new ItemType()
                {
                    ItemCode = (string)row.GetValue(0),
                    DisplayName = (string)row.GetValue(1), // We want to replace this display name with locales
                };

                if (buff.ItemCode == null)
                    continue;

                items.Add(buff);
            }
        }
    }
}
