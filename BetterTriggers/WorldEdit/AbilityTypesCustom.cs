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
    internal class AbilityTypesCustom
    {
        private static List<AbilityType> abilityTypes;
        
        internal static List<AbilityType> Load( )
        {
            abilityTypes = new List<AbilityType>();
            Stream s = new FileStream(Path.Combine(CustomMapData.mapPath, "war3map.w3a"), FileMode.Open);
            BinaryReader reader = new BinaryReader(s);
            var customAbilities = BinaryReaderExtensions.ReadAbilityObjectData(reader, true);

            for(int i = 0; i < customAbilities.NewAbilities.Count; i++)
            {
                var customAbility = customAbilities.NewAbilities[i];
                abilityTypes.Add(new AbilityType()
                {
                    AbilCode = customAbility.ToString(),
                });
            }

            return abilityTypes;
        }
    }
}
