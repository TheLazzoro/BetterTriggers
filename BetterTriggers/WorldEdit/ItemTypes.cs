using BetterTriggers.Containers;
using BetterTriggers.Models.War3Data;
using BetterTriggers.Utility;
using BetterTriggers.WorldEdit.GameDataReader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using War3Net.Build.Object;
using War3Net.Common.Extensions;
using War3Net.IO.Slk;

namespace BetterTriggers.WorldEdit
{
    public class ItemTypes
    {
        private static Dictionary<string, ItemType> items;
        private Dictionary<string, ItemType> itemsBaseEdited = new();
        private Dictionary<string, ItemType> itemsCustom = new();

        public List<ItemType> GetAll()
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

        public static ItemType GetItemType(ItemTypes? itemTypes, string itemcode)
        {
            ItemType? itemType = null;

            if (itemTypes != null)
            {
                itemTypes.itemsCustom.TryGetValue(itemcode, out itemType);

                if (itemType == null)
                    itemTypes.itemsBaseEdited.TryGetValue(itemcode, out itemType);

                if (itemType == null)
                    items.TryGetValue(itemcode, out itemType);
            }

            if (itemType == null)
                itemType = new ItemType()
                {
                    ItemCode = itemcode,
                    DisplayName = "<Unknown Doodad>"
                };

            return itemType;
        }

        internal static string GetName(ItemTypes? itemTypes, string itemcode)
        {
            ItemType itemType = GetItemType(itemTypes, itemcode);
            if (itemType == null)
                return "<Empty Name>";

            if (string.IsNullOrEmpty(itemType.DisplayName))
                return "<Empty Name>";

            return itemType.DisplayName;
        }

        internal static void LoadFromGameStorage(bool isTest)
        {
            items = new Dictionary<string, ItemType>();

            Stream itemskin;
            Stream itemfunc;

            if (!isTest && WarcraftStorageReader.GameVersion < new Version(1, 32))
            {
                LoadFromMpq();
                return;
            }

            if (isTest)
            {
                itemskin = new FileStream(Path.Combine(Directory.GetCurrentDirectory(), "TestResources/itemskin.txt"), FileMode.Open);
                itemfunc = new FileStream(Path.Combine(Directory.GetCurrentDirectory(), "TestResources/itemfunc.txt"), FileMode.Open);
            }
            else
            {
                itemskin = WarcraftStorageReader.OpenFile(@"units\itemskin.txt");
                itemfunc = WarcraftStorageReader.OpenFile(@"units\itemfunc.txt");
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
                {
                    new Icon(path, ItemTypes.GetName(null, sectionName), "Item");
                }
            }

            itemfunc.Close();

        }

        private static void LoadFromMpq()
        {
            SylkParser sylkParser = new SylkParser();
            SylkTable table;
            Stream itemfunc;

            itemfunc = WarcraftStorageReader.OpenFile(@"units\itemfunc.txt");
            using (Stream itemData = WarcraftStorageReader.OpenFile(@"units\itemdata.slk"))
            {
                table = sylkParser.Parse(itemData);
            }

            var count = table.Count();
            for (int i = 1; i < count; i++)
            {
                var row = table.ElementAt(i);
                var id = (string)row.GetValue(0);
                var item = new ItemType()
                {
                    ItemCode = id,
                    DisplayName = Locale.GetDisplayName(id),
                    Model = (string)row.GetValue(28),
                };
                items.TryAdd(item.ItemCode, item);
            }

            // --- Itemfunc (art) --- //

            var reader = new StreamReader(itemfunc);
            var text = reader.ReadToEnd();
            var data = IniFileConverter.GetIniData(text);
            var sections = data.Sections.GetEnumerator();
            while (sections.MoveNext())
            {
                string sectionName = sections.Current.SectionName;
                var keys = sections.Current.Keys;
                string path = keys["Art"];
                if (path != null)
                    new Icon(path, ItemTypes.GetName(null, sectionName), "Item");
            }

            itemfunc.Close();
        }

        internal void Load(Project project)
        {
            itemsBaseEdited = new Dictionary<string, ItemType>();
            itemsCustom = new Dictionary<string, ItemType>();

            ItemObjectData customItems;
            customItems = project.MPQMap.ItemObjectData;
            if (customItems == null)
                return;

            for (int i = 0; i < customItems.BaseItems.Count; i++)
            {
                var baseItem = customItems.BaseItems[i];
                ItemType baseAbil = GetItemType(this, Int32Extensions.ToRawcode(baseItem.OldId));
                string name = baseAbil.DisplayName;
                var item = new ItemType()
                {
                    ItemCode = baseItem.ToString().Substring(0, 4),
                    DisplayName = name,
                };
                itemsBaseEdited.TryAdd(item.ItemCode, item);
                SetCustomFields(project, baseItem, Int32Extensions.ToRawcode(baseItem.OldId));
            }

            for (int i = 0; i < customItems.NewItems.Count; i++)
            {
                var customItem = customItems.NewItems[i];
                ItemType baseAbil = GetItemType(this, Int32Extensions.ToRawcode(customItem.OldId));
                string name = baseAbil.DisplayName;
                var item = new ItemType()
                {
                    ItemCode = customItem.ToString().Substring(0, 4),
                    DisplayName = name,
                };
                itemsCustom.TryAdd(item.ItemCode, item);
                SetCustomFields(project, customItem, item.ItemCode);
            }
        }

        private void SetCustomFields(Project project, SimpleObjectModification modified, string itemcode)
        {
            ItemType itemType = GetItemType(this, itemcode);
            string displayName = itemType.DisplayName;

            foreach (var modification in modified.Modifications)
            {
                if (Int32Extensions.ToRawcode(modification.Id) == "unam")
                    displayName = project.MapStrings.GetString(modification.ValueAsString);
            }

            itemType.DisplayName = displayName;
        }
    }
}
