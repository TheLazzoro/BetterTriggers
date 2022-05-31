using BetterTriggers.Utility;
using CASCLib;
using IniParser.Model;
using IniParser.Parser;
using Model.War3Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using War3Net.Build.Extensions;
using War3Net.IO.Slk;

namespace BetterTriggers.WorldEdit
{
    public class UnitTypes
    {
        private static List<UnitType> unitTypes { get; set; }
        private static List<UnitType> unitTypesBase { get; set; }
        private static List<UnitType> unitTypesCustom { get; set; }

        internal static List<UnitType> GetAll()
        {
            return unitTypes;
        }

        internal static List<UnitType> GetBase()
        {
            return unitTypesBase;
        }

        internal static List<UnitType> GetUnitTypesCustom()
        {
            return unitTypesCustom;
        }

        internal static string GetName(string unitcode)
        {
            throw new NotImplementedException();
        }

        internal static void Load()
        {
            unitTypes = new List<UnitType>();
            unitTypesBase = new List<UnitType>();
            unitTypesCustom = new List<UnitType>();

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

                unitTypes.Add(unitType);
                unitTypesBase.Add(unitType);
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

            for (int i = 0; i < unitTypes.Count; i++)
            {
                var section = data[unitTypes[i].Id];

                var icon = section["Art"];
                var sort = section["sortUI"];
                var isSpecial = section["special"];
                var model = section["file"];

                unitTypes[i].Icon = icon;
                unitTypes[i].Sort = sort;
                unitTypes[i].isSpecial = isSpecial == "1";
                unitTypes[i].Model = model;
                unitTypes[i].Image = Casc.GetCasc().OpenFile("War3.w3mod/" + Path.ChangeExtension(icon, ".dds"));
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
                var customUnits = BinaryReaderExtensions.ReadUnitObjectData(bReader, true);

                for (int i = 0; i < customUnits.NewUnits.Count; i++)
                {
                    var customUnit = customUnits.NewUnits[i];
                    var unitType = new UnitType()
                    {
                        Id = customUnit.ToString(),
                    };
                    unitTypes.Add(unitType);
                    unitTypesCustom.Add(unitType);
                }
            }
        }
    }
}
