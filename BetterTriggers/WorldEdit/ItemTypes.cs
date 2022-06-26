using BetterTriggers.Models.War3Data;
using BetterTriggers.Utility;
using CASCLib;
using IniParser.Model;
using IniParser.Parser;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using War3Net.Build.Extensions;
using War3Net.Common.Extensions;

namespace BetterTriggers.WorldEdit
{
    public class ItemTypes
    {
        private static Dictionary<string, ItemType> items;
        private static Dictionary<string, ItemType> itemsBase;
        private static Dictionary<string, ItemType> itemsCustom;

        internal static List<ItemType> GetAll()
        {
            return items.Select(kvp => kvp.Value).ToList();

        }
        internal static List<ItemType> GetBase()
        {
            return itemsBase.Select(kvp => kvp.Value).ToList();
        }
        internal static List<ItemType> GetCustom()
        {
            return itemsCustom.Select(kvp => kvp.Value).ToList();
        }

        public static ItemType GetItemType(string itemcode)
        {
            ItemType destType;
            items.TryGetValue(itemcode, out destType);
            return destType;
        }

        internal static string GetName(string itemcode)
        {
            return GetItemType(itemcode).DisplayName;
        }

        internal static void Load()
        {
            items = new Dictionary<string, ItemType>();
            itemsBase = new Dictionary<string, ItemType>();
            itemsCustom = new Dictionary<string, ItemType>();

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
                var model = keys["file"];

                var item = new ItemType()
                {
                    ItemCode = id,
                    DisplayName = Locale.Translate(id),
                    Model = model,
                };
                items.TryAdd(item.ItemCode, item);
                itemsBase.TryAdd(item.ItemCode, item);
            }

            string filePath = "war3map.w3t";
            if (!File.Exists(Path.Combine(CustomMapData.mapPath, filePath)))
                return;

            while (CustomMapData.IsMapSaving())
            {
                Thread.Sleep(1000);
            }

            //Custom items
            using (Stream s = new FileStream(Path.Combine(CustomMapData.mapPath, filePath), FileMode.Open, FileAccess.Read))
            {
                BinaryReader bReader = new BinaryReader(s);
                var customItems = War3Net.Build.Extensions.BinaryReaderExtensions.ReadItemObjectData(bReader, true);

                for (int i = 0; i < customItems.NewItems.Count; i++)
                {
                    var customItem = customItems.NewItems[i];

                    ItemType baseAbil = GetItemType(Int32Extensions.ToRawcode(customItem.OldId));
                    string name = baseAbil.DisplayName;
                    foreach (var modified in customItem.Modifications)
                    {
                        if (Int32Extensions.ToRawcode(modified.Id) == "unam")
                            name = MapStrings.GetString(modified.ValueAsString);
                    }

                    var item = new ItemType()
                    {
                        ItemCode = customItem.ToString().Substring(0, 4),
                        DisplayName = name,
                    };
                    items.TryAdd(item.ItemCode, item);
                    itemsCustom.TryAdd(item.ItemCode, item);
                }
            }
        }

    }
}
