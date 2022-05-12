using BetterTriggers.WorldEdit;
using Model.War3Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using War3Net.Build.Environment;
using War3Net.Build.Extensions;
using War3Net.Build.Object;
using War3Net.Build.Widget;

namespace BetterTriggers
{
    public static class CustomMapData
    {
        public static string mapPath = @"D:\Test\TestMap.w3x";

        public static List<UnitType> customUnits = new List<UnitType>();
        public static List<ItemType> customItems = new List<ItemType>();
        public static List<DestructibleType> customDest = new List<DestructibleType>();
        public static List<AbilityType> customAbilities = new List<AbilityType>();
        public static List<BuffType> customBuffs = new List<BuffType>();
        public static List<UpgradeType> customUpgrades = new List<UpgradeType>();

        public static List<Camera> cameras = new List<Camera>();
        public static List<DoodadData> destructibles = new List<DoodadData>();
        public static List<Region> regions = new List<Region>();
        public static List<UnitData> units = new List<UnitData>();

        public static void Load()
        {
            cameras = Cameras.Load();
            destructibles = Destructibles.Load();
            regions = Regions.Load();
            units = Units.Load();

            customUnits = UnitTypesCustom.Load();
            customItems = ItemTypesCustom.Load();
            customDest = DestructibleTypesCustom.Load();
            customAbilities = AbilityTypesCustom.Load();
            customBuffs = BuffTypesCustom.Load();
            customUpgrades = UpgradeTypesCustom.Load();
        }
    }
}
