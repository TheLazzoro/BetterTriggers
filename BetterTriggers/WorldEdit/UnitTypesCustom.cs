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
    internal class UnitTypesCustom
    {
        private static List<UnitType> unitTypes;
        
        internal static List<UnitType> Load( )
        {
            unitTypes = new List<UnitType>();
            Stream s = new FileStream(Path.Combine(CustomMapData.mapPath, "war3map.w3u"), FileMode.Open);
            BinaryReader reader = new BinaryReader(s);
            var customUnits = BinaryReaderExtensions.ReadUnitObjectData(reader, true);

            for(int i = 0; i < customUnits.NewUnits.Count; i++)
            {
                var customUnit = customUnits.NewUnits[i];
                unitTypes.Add(new UnitType()
                {
                    Id = customUnit.ToString(),
                });
            }

            return unitTypes;
        }
    }
}
