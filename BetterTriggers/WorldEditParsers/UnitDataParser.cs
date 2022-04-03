using CASCLib;
using Model.War3Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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
            files = CASCFolder.GetFiles(units.Entries.Select(kv => kv.Value), null, false).ToList();

            int iterator = 0;
            while (iterator < files.Count)
            {
                if (files[iterator].Name.ToLower() == "unitdata.slk")
                {
                    CASCFile unitData = files[iterator];
                    var file = Casc.GetCasc().OpenFile(unitData.FullName);
                    StreamReader reader = new StreamReader(file);
                    string data = reader.ReadToEnd();
                    string[] slkUnitData = data.Split("\r\n");



                    Regex regexUnitType = new Regex(@";X1;"); // 'X1' is unit Id
                    Regex regexSort = new Regex(@";X2;");     // 'X2' is sort
                    Regex regexRace = new Regex(@";X4;");     // 'X4' is race

                    for (int i = 0; i < slkUnitData.Length; i++)
                    {
                        MatchCollection matches = regexUnitType.Matches(slkUnitData[i]);
                        if (matches.Count > 0)
                        {
                            UnitType unitType = new UnitType();
                            unitType.Id = ParseCell(slkUnitData[i]);
                            unitTypes.Add(unitType);
                        }

                        matches = regexSort.Matches(slkUnitData[i]);
                        if (matches.Count > 0)
                        {
                            // Was created in first match
                            unitTypes[unitTypes.Count - 1].Sort = ParseCell(slkUnitData[i]);
                        }

                        matches = regexRace.Matches(slkUnitData[i]);
                        if (matches.Count > 0)
                        {
                            // Was created in first match
                            unitTypes[unitTypes.Count - 1].Race = ParseCell(slkUnitData[i]);
                        }
                    }

                    unitTypes.RemoveAt(0);
                }

                // Set icons on all unit type entries
                if (files[iterator].Name.ToLower() == "unitskin.txt")
                {
                    CASCFile unitSkins = files[iterator];
                    var file = Casc.GetCasc().OpenFile(unitSkins.FullName);
                    StreamReader reader = new StreamReader(file);
                    string data = reader.ReadToEnd();
                    string[] unitSkinsData = data.Split("\r\n");

                    // Loop through all unit types and regex their Id to find the art
                    for (int i = 0; i < unitTypes.Count; i++)
                    {
                        var unitType = unitTypes[i];

                        Regex regexUnitType = new Regex("\\[" + unitType.Id + "\\]");

                        int lineNumber = 0;
                        bool matchesUnitType = false;
                        while(!matchesUnitType && lineNumber < unitSkinsData.Length)
                        {
                            MatchCollection matches = regexUnitType.Matches(unitSkinsData[lineNumber]);
                            if(matches.Count > 0)
                            {
                                matchesUnitType = true;

                                // find icon
                                string key = unitSkinsData[lineNumber].Substring(0, 4);
                                while(key != "Art=")
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
                }

                iterator++;
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
