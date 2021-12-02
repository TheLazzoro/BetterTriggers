using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriggerParser.Types
{
    public static class TriggerTypeParser
    {
        public static void ParseVariableTypes(string file)
        {
            ParseTypes(file, TriggerTypeContainer.variableTypes);
        }

        public static void ParseOtherTypes(string file)
        {
            ParseTypes(file, TriggerTypeContainer.otherTypes);
        }

        private static void ParseTypes(string file, List<TriggerType> list)
        {
            IEnumerable<string> lines = File.ReadLines(file);

            foreach (string line in lines)
            {
                string key = string.Empty;
                string version = string.Empty;
                string globalVariable = string.Empty;
                string canCompare = string.Empty;
                string displayName = string.Empty;
                string baseType = string.Empty;
                string importType = string.Empty;
                string flag = string.Empty;

                int memberIndex = 0;

                // read line
                for (int i = 0; i < line.Length; i++)
                {
                    char c = line[i];
                    bool isSeperator = false;

                    if (c == '=' || c == ',')
                        isSeperator = true;

                    if (!isSeperator)
                    {
                        switch (memberIndex)
                        {
                            case 0:
                                key += c;
                                break;
                            case 1:
                                version += c;
                                break;
                            case 2:
                                globalVariable += c;
                                break;
                            case 3:
                                canCompare += c;
                                break;
                            case 4:
                                displayName += c;
                                break;
                            case 5:
                                baseType += c;
                                break;
                            case 6:
                                importType += c;
                                break;
                            case 7:
                                flag += c;
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {

                        memberIndex++;
                    }
                }

                int intVersion = 0;
                bool boolGlobalVariable = false;
                bool boolCanCompare = false;
                bool boolFlag = false;

                if (version != "")
                    intVersion = int.Parse(version);
                if (globalVariable != "")
                    if (int.Parse(globalVariable) != 0) { boolGlobalVariable = true; };
                if (canCompare != "")
                    if (int.Parse(canCompare) != 0) { boolCanCompare = true; };
                if (flag != "")
                    if (int.Parse(flag) != 0) { boolFlag = true; };

                var type = new TriggerType()
                {
                    key = key,
                    version = intVersion,
                    globalVariable = boolGlobalVariable,
                    canCompare = boolCanCompare,
                    displayName = displayName,
                    baseType = baseType,
                    importType = importType,
                    flag = boolFlag,
                };

                list.Add(type);
            }
        }
    }
}
