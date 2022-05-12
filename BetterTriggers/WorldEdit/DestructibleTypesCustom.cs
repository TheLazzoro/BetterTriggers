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
    internal class DestructibleTypesCustom
    {
        private static List<DestructibleType> destructiblesTypes;
        
        internal static List<DestructibleType> Load( )
        {
            destructiblesTypes = new List<DestructibleType>();
            Stream s = new FileStream(Path.Combine(CustomMapData.mapPath, "war3map.w3b"), FileMode.Open);
            BinaryReader reader = new BinaryReader(s);
            var customDests = BinaryReaderExtensions.ReadDestructableObjectData(reader, true);

            for(int i = 0; i < customDests.NewDestructables.Count; i++)
            {
                var customDest = customDests.NewDestructables[i];
                destructiblesTypes.Add(new DestructibleType()
                {
                    DestCode = customDest.ToString(),
                });
            }

            return destructiblesTypes;
        }
    }
}
