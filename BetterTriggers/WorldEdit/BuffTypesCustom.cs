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
    internal class BuffTypesCustom
    {
        private static List<BuffType> buffTypes;
        
        internal static List<BuffType> Load( )
        {
            buffTypes = new List<BuffType>();
            Stream s = new FileStream(Path.Combine(CustomMapData.mapPath, "war3map.w3h"), FileMode.Open);
            BinaryReader reader = new BinaryReader(s);
            var customBuffs = BinaryReaderExtensions.ReadBuffObjectData(reader, true);

            for(int i = 0; i < customBuffs.NewBuffs.Count; i++)
            {
                var customBuff = customBuffs.NewBuffs[i];
                buffTypes.Add(new BuffType()
                {
                    BuffCode = customBuff.ToString(),
                });
            }

            return buffTypes;
        }
    }
}
