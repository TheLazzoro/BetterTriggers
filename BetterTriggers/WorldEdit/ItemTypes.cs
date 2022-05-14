using BetterTriggers.Utility;
using CASCLib;
using IniParser.Model;
using IniParser.Parser;
using Model.War3Data;
using System.Collections.Generic;
using System.IO;
using War3Net.Build.Extensions;

namespace BetterTriggers.WorldEdit
{
    public class ItemTypes
    {
        private static List<ItemType> items;
        private static List<ItemType> itemsBase;
        private static List<ItemType> itemsCustom;

        internal static List<ItemType> GetAll()
        {
            return items;

        }
        internal static List<ItemType> GetBase()
        {
            return itemsBase;
        }
        internal static List<ItemType> GetCustom()
        {
            return itemsCustom;
        }

        internal static void Load()
        {
            items = new List<ItemType>();
            itemsBase = new List<ItemType>();
            itemsCustom = new List<ItemType>();

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

                var item = new ItemType()
                {
                    ItemCode = id,
                    DisplayName = name,
                    Model = model,
                };
                items.Add(item);
                itemsBase.Add(item);
            }

            //Custom items
            Stream s = new FileStream(Path.Combine(CustomMapData.mapPath, "war3map.w3t"), FileMode.Open);
            BinaryReader bReader = new BinaryReader(s);
            var customItems = BinaryReaderExtensions.ReadItemObjectData(bReader, true);

            for (int i = 0; i < customItems.NewItems.Count; i++)
            {
                var customItem = customItems.NewItems[i];
                var item = new ItemType()
                {
                    ItemCode = customItem.ToString(),
                };
                items.Add(item);
                itemsCustom.Add(item);
            }

        }
    }
}
