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
using War3Net.Build.Object;
using War3Net.Common.Extensions;

namespace BetterTriggers.WorldEdit
{
    public class ItemTypes
    {
        private static Dictionary<string, ItemType> items;
        private static Dictionary<string, ItemType> itemsBaseEdited = new();
        private static Dictionary<string, ItemType> itemsCustom = new();

        internal static List<ItemType> GetAll()
        {
            List<ItemType> list = new List<ItemType>();
            var enumerator = items.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ItemType itemType;
                var key = enumerator.Current.Key;
                if (itemsBaseEdited.ContainsKey(key))
                {
                    itemsBaseEdited.TryGetValue(key, out itemType);
                    list.Add(itemType);
                }
                else
                {
                    items.TryGetValue(key, out itemType);
                    list.Add(itemType);
                }
            }

            list.AddRange(itemsCustom.Select(kvp => kvp.Value).ToList());

            return list;
        }

        internal static List<ItemType> GetBase()
        {
            return items.Select(kvp => kvp.Value).ToList();
        }

        public static ItemType GetItemType(string itemcode)
        {
            ItemType itemType;
            itemsCustom.TryGetValue(itemcode, out itemType);

            if (itemType == null)
                itemsBaseEdited.TryGetValue(itemcode, out itemType);

            if (itemType == null)
                items.TryGetValue(itemcode, out itemType);

            return itemType;
        }

        internal static string GetName(string itemcode)
        {
            ItemType itemType = GetItemType(itemcode);
            if (itemType == null)
                return "<Empty Name>";

            if (string.IsNullOrEmpty(itemType.DisplayName))
                return "<Empty Name>";

            return itemType.DisplayName;
        }

        internal static void LoadFromCASC(bool isTest)
        {
            items = new Dictionary<string, ItemType>();

            Stream itemskin;
            Stream itemfunc;

            if (isTest)
            {
                itemskin = new FileStream(Path.Combine(Directory.GetCurrentDirectory(), "TestResources/itemskin.txt"), FileMode.Open);
                itemfunc = new FileStream(Path.Combine(Directory.GetCurrentDirectory(), "TestResources/itemfunc.txt"), FileMode.Open);
            }
            else
            {
                var units = (CASCFolder)Casc.GetWar3ModFolder().Entries["units"];
                CASCFile itemSkins = (CASCFile)units.Entries["itemskin.txt"];
                CASCFile itemfuncs = (CASCFile)units.Entries["itemfunc.txt"];
                itemskin = Casc.GetCasc().OpenFile(itemSkins.FullName);
                itemfunc = Casc.GetCasc().OpenFile(itemfuncs.FullName);
            }


            var reader = new StreamReader(itemskin);
            var text = reader.ReadToEnd();
            var data = IniFileConverter.GetIniData(text);

            var sections = data.Sections.GetEnumerator();
            while (sections.MoveNext())
            {
                var id = sections.Current.SectionName;
                var keys = sections.Current.Keys;
                var model = keys["file"];

                var item = new ItemType()
                {
                    ItemCode = id,
                    DisplayName = Locale.GetDisplayName(id),
                    Model = model,
                };
                items.TryAdd(item.ItemCode, item);
            }

            itemskin.Close();

            // --- Itemfunc (art) --- //

            reader = new StreamReader(itemfunc);
            text = reader.ReadToEnd();
            data = IniFileConverter.GetIniData(text);
            sections = data.Sections.GetEnumerator();
            while (sections.MoveNext())
            {
                string sectionName = sections.Current.SectionName;
                var keys = sections.Current.Keys;
                string path = keys["Art"];
                if (path != null)
                    new Icon(path, ItemTypes.GetName(sectionName), "Item");
            }

            itemfunc.Close();

        }

        internal static void Load()
        {
            itemsBaseEdited = new Dictionary<string, ItemType>();
            itemsCustom = new Dictionary<string, ItemType>();

            ItemObjectData customItems;
            customItems = CustomMapData.MPQMap.ItemObjectData;
            if (customItems == null)
                return;

            for (int i = 0; i < customItems.BaseItems.Count; i++)
            {
                var baseItem = customItems.BaseItems[i];
                ItemType baseAbil = GetItemType(Int32Extensions.ToRawcode(baseItem.OldId));
                string name = baseAbil.DisplayName;
                var item = new ItemType()
                {
                    ItemCode = baseItem.ToString().Substring(0, 4),
                    DisplayName = name,
                };
                itemsBaseEdited.TryAdd(item.ItemCode, item);
                SetCustomFields(baseItem, Int32Extensions.ToRawcode(baseItem.OldId));
            }

            for (int i = 0; i < customItems.NewItems.Count; i++)
            {
                var customItem = customItems.NewItems[i];
                ItemType baseAbil = GetItemType(Int32Extensions.ToRawcode(customItem.OldId));
                string name = baseAbil.DisplayName;
                var item = new ItemType()
                {
                    ItemCode = customItem.ToString().Substring(0, 4),
                    DisplayName = name,
                };
                itemsCustom.TryAdd(item.ItemCode, item);
                SetCustomFields(customItem, item.ItemCode);
            }
        }

        private static void SetCustomFields(SimpleObjectModification modified, string itemcode)
        {
            ItemType itemType = GetItemType(itemcode);
            string displayName = itemType.DisplayName;

            foreach (var modification in modified.Modifications)
            {
                if (Int32Extensions.ToRawcode(modification.Id) == "unam")
                    displayName = MapStrings.GetString(modification.ValueAsString);
            }

            itemType.DisplayName = displayName;
        }
    }
}
