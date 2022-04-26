using BetterTriggers.Utility;
using CASCLib;
using IniParser.Model;
using IniParser.Parser;
using Model.War3Data;
using System;
using System.Collections.Generic;
using System.IO;
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

            // Parse ini file
            CASCFile itemSkins = (CASCFile)units.Entries["itemskin.txt"];
            var file = Casc.GetCasc().OpenFile(itemSkins.FullName);
            var reader = new StreamReader(file);
            var text = reader.ReadToEnd();

            var iniFile = IniFileConverter.Convert(text);
            IniDataParser parser = new IniDataParser();
            parser.Configuration.AllowDuplicateSections = true;
            parser.Configuration.AllowDuplicateKeys = true;
            IniData data = parser.Parse(iniFile);


            var sections = data.Sections.GetEnumerator();
            while (sections.MoveNext())
            {
                var id = sections.Current.SectionName;
                var keys = sections.Current.Keys;
                var name = id;
                var model = keys["file"];

                items.Add(new ItemType()
                {
                    ItemCode = id,
                    DisplayName = name,
                    Model = model,
                });
            }
        }
    }
}
