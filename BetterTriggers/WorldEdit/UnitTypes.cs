﻿using BetterTriggers.Containers;
using BetterTriggers.Models.War3Data;
using BetterTriggers.Utility;
using CASCLib;
using IniParser.Model;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using War3Net.Build.Object;
using War3Net.Common.Extensions;
using War3Net.IO.Slk;

namespace BetterTriggers.WorldEdit
{
    public class UnitTypes
    {
        private static Dictionary<string, UnitType> unitTypes;
        private static Dictionary<string, UnitType> unitTypesBaseEdited;
        private static Dictionary<string, UnitType> unitTypesCustom;
        private static bool IsTest = false;

        public static List<UnitType> GetAll()
        {
            List<UnitType> list = new List<UnitType>();
            var enumerator = unitTypes.GetEnumerator();
            while (enumerator.MoveNext())
            {
                UnitType unitType;
                var key = enumerator.Current.Key;
                if (unitTypesBaseEdited.ContainsKey(key))
                {
                    unitTypesBaseEdited.TryGetValue(key, out unitType);
                    list.Add(unitType);
                }
                else
                {
                    unitTypes.TryGetValue(key, out unitType);
                    list.Add(unitType);
                }
            }

            list.AddRange(unitTypesCustom.Select(kvp => kvp.Value).ToList());

            return list;
        }

        internal static List<UnitType> GetBase()
        {
            return unitTypes.Select(kvp => kvp.Value).ToList();
        }

        public static UnitType GetUnitType(string unitcode)
        {
            UnitType unitType;
            unitTypes.TryGetValue(unitcode, out unitType);

            if (unitType == null)
                unitTypesBaseEdited.TryGetValue(unitcode, out unitType);

            if (unitType == null)
                unitTypesCustom.TryGetValue(unitcode, out unitType);

            if (unitType == null)
                unitType = new UnitType()
                {
                    Id = unitcode,
                    Name = new UnitName()
                    {
                        Name = "<Unknown Unit>"
                    }
                };

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

            UnitName unitName = unitType.Name;
            if (unitName.Propernames != string.Empty)
                name = unitType.isCampaign ? unitName.Propernames.Split(',')[0] : unitName.Name;
            else
                name = unitName.Name;

            if (!string.IsNullOrEmpty(unitName.EditorSuffix))
                name = name + " " + unitName.EditorSuffix;

            return name;
        }

        internal static void LoadFromCASC(bool isTest)
        {
            unitTypes = new Dictionary<string, UnitType>();

            SylkParser sylkParser = new SylkParser();
            StreamReader reader;
            SylkTable table;
            string text;
            IniData campaignFunc;

            IsTest = isTest;

            if (isTest)
            {
                using (Stream unitDataSlk = new FileStream(Path.Combine(Directory.GetCurrentDirectory(), "TestResources/unitdata.slk"), FileMode.Open))
                using (Stream unitSkin = new FileStream(Path.Combine(Directory.GetCurrentDirectory(), "TestResources/unitskin.txt"), FileMode.Open))
                using (Stream campaignFuncs = new FileStream(Path.Combine(Directory.GetCurrentDirectory(), "TestResources/campaignunitfunc.txt"), FileMode.Open))
                {
                    table = sylkParser.Parse(unitDataSlk);
                    reader = new StreamReader(unitSkin);
                    text = reader.ReadToEnd();

                    StreamReader sr = new StreamReader(campaignFuncs);
                    campaignFunc = IniFileConverter.GetIniData(sr.ReadToEnd());
                }
            }
            else
            {
                var units = (CASCFolder)Casc.GetWar3ModFolder().Entries["units"];
                CASCFile cascFile = (CASCFile)units.Entries["unitdata.slk"];
                CASCFile unitSkins = (CASCFile)units.Entries["unitskin.txt"];
                CASCFile campaignFuncFile = (CASCFile)units.Entries["campaignunitfunc.txt"];
                using (Stream unitDataSlk = Casc.GetCasc().OpenFile(cascFile.FullName))
                using (Stream unitSkin = Casc.GetCasc().OpenFile(unitSkins.FullName))
                using (Stream campaignFuncStream = Casc.GetCasc().OpenFile(campaignFuncFile.FullName))
                {
                    reader = new StreamReader(unitSkin);
                    text = reader.ReadToEnd();
                    table = sylkParser.Parse(unitDataSlk);

                    StreamReader sr = new StreamReader(campaignFuncStream);
                    campaignFunc = IniFileConverter.GetIniData(sr.ReadToEnd());
                }
            }



            int count = table.Count();
            for (int i = 1; i < count; i++)
            {
                var row = table.ElementAt(i);
                UnitType unitType = new UnitType()
                {
                    Id = (string)row.GetValue(0),
                    Sort = (string)row.GetValue(1),
                    Race = (string)row.GetValue(3),
                };

                unitTypes.TryAdd(unitType.Id, unitType);
            }


            var data = IniFileConverter.GetIniData(text);
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
                unitType.Name = Locale.GetUnitName(unitType.Id); // Spaghetti

                new Icon(icon, UnitTypes.GetName(unitType.Id), "Unit");

                if (!isTest)
                    unitType.Image = Images.ReadImage(Casc.GetCasc().OpenFile("War3.w3mod/" + Path.ChangeExtension(icon, ".dds")));
            }

            var campaignSections = campaignFunc.Sections;
            var enumerator = campaignSections.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var section = enumerator.Current;
                string sectionName = section.SectionName;
                var keys = section.Keys.GetEnumerator();
                while (keys.MoveNext())
                {
                    var key = keys.Current;
                    if (key.KeyName == "ScoreScreenIcon")
                        new Icon(key.Value, UnitTypes.GetName(sectionName), "Unit - Special");
                }
            }
        }

        internal static void Load(string fullMapPath)
        {
            unitTypesBaseEdited = new Dictionary<string, UnitType>();
            unitTypesCustom = new Dictionary<string, UnitType>();

            UnitObjectData customUnits = CustomMapData.MPQMap.UnitObjectData;
            if (customUnits == null)
                return;

            // Base units
            foreach (var baseUnit in customUnits.BaseUnits)
            {
                UnitType unit = GetUnitType(Int32Extensions.ToRawcode(baseUnit.OldId));
                UnitName name = unit.Name.Clone();
                string sort = unit.Sort;
                string race = unit.Race;
                string icon = unit.Icon;
                Bitmap image = unit.Image;
                var unitType = new UnitType()
                {
                    Id = baseUnit.ToString().Substring(0, 4),
                    Name = name,
                    Sort = sort,
                    Race = race,
                    Icon = icon,
                    Image = image
                };
                unitTypesBaseEdited.Add(unitType.Id, unitType);
                SetCustomFields(baseUnit, Int32Extensions.ToRawcode(baseUnit.OldId), fullMapPath);
            }

            // custom units
            for (int i = 0; i < customUnits.NewUnits.Count; i++)
            {
                var customUnit = customUnits.NewUnits[i];

                UnitType baseUnit = GetUnitType(Int32Extensions.ToRawcode(customUnit.OldId));
                UnitName name = baseUnit.Name.Clone();
                string sort = baseUnit.Sort;
                string race = baseUnit.Race;
                string icon = baseUnit.Icon;
                Bitmap image = baseUnit.Image;

                var unitType = new UnitType()
                {
                    Id = customUnit.ToString().Substring(0, 4),
                    Name = name,
                    Sort = sort,
                    Race = race,
                    Icon = icon,
                    Image = image
                };

                unitTypesCustom.TryAdd(unitType.Id, unitType);
                SetCustomFields(customUnit, unitType.Id, fullMapPath);
            }
        }

        private static void SetCustomFields(SimpleObjectModification modified, string unitId, string fullMapPath)
        {
            UnitType unitType = GetUnitType(unitId);
            UnitName unitName = unitType.Name;
            string race = unitType.Race;
            string icon = unitType.Icon;
            string sort = unitType.Sort;
            bool isSpecial = unitType.isSpecial;
            Bitmap image = unitType.Image;

            foreach (var modification in modified.Modifications)
            {
                if (Int32Extensions.ToRawcode(modification.Id) == "unam")
                    unitName.Name = MapStrings.GetString(modification.ValueAsString);
                else if (Int32Extensions.ToRawcode(modification.Id) == "unsf")
                    unitName.EditorSuffix = MapStrings.GetString(modification.ValueAsString);
                else if (Int32Extensions.ToRawcode(modification.Id) == "urac")
                    race = modification.Value as string;
                else if (Int32Extensions.ToRawcode(modification.Id) == "uspe")
                    isSpecial = (modification.Value as int?) == 1;
                else if (Int32Extensions.ToRawcode(modification.Id) == "ubdg") // is building
                    sort = (modification.Value as int?) == 1 ? "u3" : sort;
                else if (Int32Extensions.ToRawcode(modification.Id) == "uico")
                {
                    string iconPath = modification.Value as string;
                    Stream stream = null;
                    if (!IsTest)
                    {
                        if (Casc.GetCasc().FileExists("War3.w3mod/" + Path.ChangeExtension(iconPath, ".dds")))
                            stream = Casc.GetCasc().OpenFile("War3.w3mod/" + Path.ChangeExtension(iconPath, ".dds"));
                    }

                    if (stream == null)
                    {
                        // WE accepts paths with no extensions, so we need to work around that.
                        iconPath = Path.Combine(fullMapPath, Path.Combine(Path.GetDirectoryName(iconPath), Path.GetFileNameWithoutExtension(iconPath)));
                        string finalIconPath = string.Empty;
                        string[] extensions = { ".blp", ".tga", ".dds" };
                        int i = 0;
                        bool exists = false;
                        while (i < extensions.Length)
                        {
                            finalIconPath = iconPath + extensions[i];
                            if (File.Exists(finalIconPath))
                            {
                                exists = true;
                                break;
                            }

                            i++;
                        }
                        if (exists)
                            stream = File.OpenRead(finalIconPath);
                        else
                            stream = File.OpenRead(Path.Combine(Directory.GetCurrentDirectory(), "Resources/Icons/War3Green.png"));
                    }

                    icon = iconPath;
                    image = Images.ReadImage(stream);
                }
            }

            unitType.Race = race;
            unitType.Icon = icon;
            unitType.isSpecial = isSpecial;
            unitType.Sort = sort;
            unitType.Image = image;
        }
    }
}
