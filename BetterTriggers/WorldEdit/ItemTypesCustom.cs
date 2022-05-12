using Model.War3Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using War3Net.Build.Extensions;

namespace BetterTriggers.WorldEdit
{
    internal class ItemTypesCustom
    {
        private static List<ItemType> itemTypes;
        
        internal static List<ItemType> Load( )
        {
            itemTypes = new List<ItemType>();
            Stream s = new FileStream(Path.Combine(CustomMapData.mapPath, "war3map.w3t"), FileMode.Open);
            BinaryReader reader = new BinaryReader(s);
            var customItems = BinaryReaderExtensions.ReadItemObjectData(reader, true);

            for(int i = 0; i < customItems.NewItems.Count; i++)
            {
                var customItem = customItems.NewItems[i];
                itemTypes.Add(new ItemType()
                {
                    ItemCode = customItem.ToString(),
                });
            }

            return itemTypes;
        }
    }
}
