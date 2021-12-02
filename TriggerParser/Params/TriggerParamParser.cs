using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriggerParser.Params
{
    public static class TriggerParamParser
    {
        public static void ParseParams(string file)
        {
            IEnumerable<string> lines = File.ReadLines(file);

            foreach (string line in lines)
            {
                string key = string.Empty;
                string version = string.Empty;
                string variableType = string.Empty;
                string codeText = string.Empty;
                string displayText = string.Empty;

                int memberIndex = 0;

                // read line
                if (line.Length > 0 && line.Substring(0, 2) != "//")
                {

                    for (int i = 0; i < line.Length; i++)
                    {
                        char c = line[i];
                        bool isSeperator = false;

                        if (c == '=' || c == ',')
                            isSeperator = true;

                        if (!isSeperator && c != '"')
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
                                    variableType += c;
                                    break;
                                case 3:
                                    codeText += c;
                                    break;
                                case 4:
                                    displayText += c;
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
                    if (version != "")
                        intVersion = int.Parse(version);

                    var category = new TriggerParam()
                    {
                        key = key,
                        version = intVersion,
                        variableType = variableType,
                        codeText = codeText,
                        displayText = displayText,
                    };

                    TriggerParamContainer.container.Add(category);

                }
            }
        }
    }
}
