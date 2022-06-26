using BetterTriggers.Models.War3Data;
using BetterTriggers.Utility;
using CASCLib;
using IniParser.Model;
using IniParser.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using War3Net.Build.Extensions;
using War3Net.Common.Extensions;
using War3Net.IO.Slk;

namespace BetterTriggers.WorldEdit
{
    public class UnitTypes
    {
        private static Dictionary<string, UnitType> unitTypes;
        private static Dictionary<string, UnitType> unitTypesBase;
        private static Dictionary<string, UnitType> unitTypesCustom;

        internal static List<UnitType> GetAll()
        {
            return unitTypes.Select(kvp => kvp.Value).ToList();
        }

        internal static List<UnitType> GetBase()
        {
            return unitTypesBase.Select(kvp => kvp.Value).ToList();
        }

        internal static List<UnitType> GetUnitTypesCustom()
        {
            return unitTypesCustom.Select(kvp => kvp.Value).ToList();
        }

        public static UnitType GetUnitType(string unitcode)
        {
            UnitType unitType;
            unitTypes.TryGetValue(unitcode, out unitType);
            return unitType;
        }

        public static string GetName(string unitcode)
        {
            return GetName(GetUnitType(unitcode));
        }

        public static string GetName(UnitType unitType)
        {
            string name;
            if (unitType == null)
                return null;

            UnitName unitName = Locale.GetUnitName(unitType.Id);
            if (unitName.Propernames != null)
                name = unitType.isCampaign ? unitName.Propernames.Split(',')[0] : unitName.Name;
            else
                name = unitName.Name;

            return name;
        }

        internal static void Load()
        {
            unitTypes = new Dictionary<string, UnitType>();
            unitTypesBase = new Dictionary<string, UnitType>();
            unitTypesCustom = new Dictionary<string, UnitType>();

            var units = (CASCFolder)Casc.GetWar3ModFolder().Entries["units"];

            // Extract base data
            CASCFile unitData = (CASCFile)units.Entries["unitdata.slk"];
            var file = Casc.GetCasc().OpenFile(unitData.FullName);
            SylkParser sylkParser = new SylkParser();
            SylkTable table = sylkParser.Parse(file);
            for (int i = 1; i < table.Count(); i++)
            {
                var row = table.ElementAt(i);
                UnitType unitType = new UnitType()
                {
                    Id = (string)row.GetValue(0),
                    Sort = (string)row.GetValue(1),
                    Race = (string)row.GetValue(3),
                };

                unitTypes.TryAdd(unitType.Id, unitType);
                unitTypesBase.TryAdd(unitType.Id, unitType);
            }

            // Parse ini file
            CASCFile unitSkins = (CASCFile)units.Entries["unitskin.txt"];
            file = Casc.GetCasc().OpenFile(unitSkins.FullName);
            var reader = new StreamReader(file);
            var text = reader.ReadToEnd();

            var iniFile = IniFileConverter.Convert(text);
            IniDataParser parser = new IniDataParser();
            parser.Configuration.AllowDuplicateSections = true;
            parser.Configuration.AllowDuplicateKeys = true;
            IniData data = parser.Parse(iniFile);

            var unitTypesList = GetBase();
            for (int i = 0; i < unitTypesList.Count; i++)
            {
                var unitType = unitTypesList[i];
                var section = data[unitType.Id];

                var icon = section["Art"];
                var sort = section["sortUI"];
                var isSpecial = section["special"];
                var isCampaign = section["campaign"];
                var model = section["file"];

                unitType.Icon = icon;
                unitType.Sort = sort;
                unitType.isSpecial = isSpecial == "1";
                unitType.isCampaign = isCampaign == "1";
                unitType.Model = model;
                unitType.Image = Casc.GetCasc().OpenFile("War3.w3mod/" + Path.ChangeExtension(icon, ".dds"));

                unitType.Name = GetName(unitType); // Spaghetti
            }

            string filePath = "war3map.w3u";
            if (!File.Exists(Path.Combine(CustomMapData.mapPath, filePath)))
                return;

            while (CustomMapData.IsMapSaving())
            {
                Thread.Sleep(1000);
            }

            // Custom units
            using (Stream s = new FileStream(Path.Combine(CustomMapData.mapPath, filePath), FileMode.Open, FileAccess.Read))
            {
                BinaryReader bReader = new BinaryReader(s);
                var customUnits = War3Net.Build.Extensions.BinaryReaderExtensions.ReadUnitObjectData(bReader, true);
                for (int i = 0; i < customUnits.NewUnits.Count; i++)
                {
                    var customUnit = customUnits.NewUnits[i];

                    UnitType baseUnit = GetUnitType(Int32Extensions.ToRawcode(customUnit.OldId));
                    string name = baseUnit.Name;
                    string sort = baseUnit.Sort;
                    foreach (var modified in customUnit.Modifications)
                    {
                        if (Int32Extensions.ToRawcode(modified.Id) == "unam")
                            name = MapStrings.GetString(modified.ValueAsString);
                    }

                    var unitType = new UnitType()
                    {
                        Id = customUnit.ToString().Substring(0, 4),
                        Name = name,
                        Sort = sort,
                        Race = baseUnit.Race, // TODO
                        Image = baseUnit.Image, // TODO
                    };
                    unitTypes.TryAdd(unitType.Id, unitType);
                    unitTypesCustom.TryAdd(unitType.Id, unitType);

                    Locale.AddUnitName(unitType.Id, new UnitName() { Name = name });
                }
            }
        }
    }
}
