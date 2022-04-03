using CASCLib;
using Model.War3Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using War3Net.IO.Slk;

namespace BetterTriggers.WorldEditParsers
{
    public class UnitDataParser
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns>A list of all base unit types.</returns>
        public List<UnitType> ParseUnitData()
        {
            List<UnitType> unitTypes = new List<UnitType>();

            List<CASCFile> files = null;
            var units = (CASCFolder)Casc.GetWar3ModFolder().Entries["units"];
            //files = CASCFolder.GetFiles(units.Entries.Select(kv => kv.Value), null, false).ToList();

            // Extract base data
            CASCFile unitData = (CASCFile)units.Entries["unitdata.slk"];
            var file = Casc.GetCasc().OpenFile(unitData.FullName);
            SylkParser sylkParser = new SylkParser();
            SylkTable table = sylkParser.Parse(file);
            for(int i = 1; i < table.Count(); i++)
            {
                var row = table.ElementAt(i);
                UnitType unitType = new UnitType()
                {
                    Id = (string)row.GetValue(0),
                    Sort = (string)row.GetValue(1),
                    Race = (string)row.GetValue(3),
                };

                unitTypes.Add(unitType);
            }

            // Set 'special' units
            CASCFile unitUI = (CASCFile)units.Entries["unitui.slk"];
            file = Casc.GetCasc().OpenFile(unitUI.FullName);
            sylkParser = new SylkParser();
            table = sylkParser.Parse(file);
            for (int i = 0; i < unitTypes.Count(); i++)
            {
                var unitType = unitTypes[i];
                var row = table.ElementAt(i+1);
                unitType.isSpecial = (int)row.GetValue(6) == 1;
            }

            // Set icons on all unit type entries
            CASCFile unitSkins = (CASCFile)units.Entries["unitskin.txt"];
            file = Casc.GetCasc().OpenFile(unitSkins.FullName);
            var reader = new StreamReader(file);
            var data = reader.ReadToEnd();
            string[] unitSkinsData = data.Split("\r\n");

            // Loop through all unit types and regex their Id to find the art
            for (int i = 0; i < unitTypes.Count; i++)
            {
                var unitType = unitTypes[i];

                var regexUnitType = new Regex("\\[" + unitType.Id + "\\]");

                int lineNumber = 0;
                bool matchesUnitType = false;
                while (!matchesUnitType && lineNumber < unitSkinsData.Length)
                {
                    MatchCollection matches = regexUnitType.Matches(unitSkinsData[lineNumber]);
                    if (matches.Count > 0)
                    {
                        matchesUnitType = true;

                        // find icon
                        string key = unitSkinsData[lineNumber].Substring(0, 4);
                        while (key != "Art=")
                        {
                            lineNumber++;

                            if (unitSkinsData[lineNumber].Length > 4)
                                key = unitSkinsData[lineNumber].Substring(0, 4);
                        }

                        // Found icon
                        string icon = unitSkinsData[lineNumber].Substring(4, unitSkinsData[lineNumber].Length - 4);
                        unitType.Icon = Path.ChangeExtension(icon, ".dds");
                        unitType.Image = Casc.GetCasc().OpenFile("War3.w3mod/" + unitType.Icon);
                    }

                    lineNumber++;
                }
            }

            return unitTypes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line">A line in the .slk file.</param>
        /// <returns>Value in the given cell</returns>
        private string ParseCell(string line)
        {
            // 'K' is present before each value in the table
            string magicChar = "K";
            int i = 0;
            bool gotValue = false;
            bool startValueParsing = false;
            string value = string.Empty;
            while (!gotValue && i < line.Length)
            {
                string s = line.Substring(i, 1);


                if (startValueParsing && s != "\"") // removes " from the final value
                    value += s;

                if (s == magicChar)
                    startValueParsing = true;

                i++;
            }

            return value;
        }
    }
}
