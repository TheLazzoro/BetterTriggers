using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriggerParser.Types;

namespace TriggerParser.Conditions
{
    public static class TriggerConditionParser
    {
        public static void ParseConditions(string file)
        {
            IEnumerable<string> lines = File.ReadLines(file);

            foreach (string line in lines)
            {
                if (line.Length > 2 && line.Substring(0, 2) != "//")
                {
                    if (line.Length > 1 && line.Substring(0, 1) != "_")
                    {
                        var condition = ParseBasicChunk(line);
                        TriggerConditionContainer.container.Add(condition);
                    }
                    else
                        ParseExtendedChunks(line);
                }
            }
        }


        private static TriggerCondition ParseBasicChunk(string line)
        {
            string key = string.Empty;
            string version = string.Empty;
            List<TriggerType> arguments = new List<TriggerType>();

            // read line
            int i = 0;

            // key / identifier
            bool foundKey = false;
            while (!foundKey)
            {
                if (line[i] != '=')
                    key += line[i];
                else
                    foundKey = true;

                i++;
            }

            // version
            bool foundVersion = false;
            while (!foundVersion && i < line.Length)
            {
                if (line[i] != ',')
                {
                    version += line[i];
                    i++;
                }
                else
                    foundVersion = true;

            }

            // arguments
            while (i < line.Length)
            {
                if (line[i] == ',')
                {
                    arguments.Add(new TriggerType());
                }
                else
                {
                    int argIndex = arguments.Count - 1;
                    arguments[argIndex].key += line[i];
                }

                i++;
            }

            int intVersion = 0;
            if (version != "")
                intVersion = int.Parse(version);

            var condition = new TriggerCondition()
            {
                key = key,
                version = intVersion,
                arguments = arguments
            };

            return condition;
        }

        private static void ParseExtendedChunks(string line)
        {
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '_')
                {
                    i++;
                    string key = string.Empty;
                    bool hasParsedName = false;
                    while (!hasParsedName)
                    {
                        c = line[i];
                        if (c != '_')
                        {
                            i++;
                            key += c;
                        }
                        else
                            hasParsedName = true;

                    }

                    // The element has already been parsed and created so we need to find it in the container
                    TriggerCondition triggerCondition = TriggerConditionContainer.FindByKey(key);

                    i++;
                    string member = string.Empty;
                    bool found = false;
                    while (!found)
                    {
                        c = line[i];
                        if (c != '=')
                        {
                            member += c;
                            i++;
                        }
                        else
                            found = true;
                    }

                    if (member == "DisplayName") // name when selecting in the combobox
                    {
                        i++;
                        string displayName = string.Empty;
                        while (i < line.Length)
                        {
                            c = line[i];
                            if (c != '"')
                                displayName += c;

                            i++;
                        }
                        triggerCondition.displayName = displayName;
                    }

                    if (member == "Parameters") // text with parameter clickables
                    {
                        i++;
                        string paramText = string.Empty;
                        while (i < line.Length)
                        {
                            c = line[i];
                            if (c != '"')
                                paramText += c;

                            if(c == ',') { 
                            }

                            i++;
                        }
                        triggerCondition.paramText = paramText;
                    }

                    if (member == "Defaults") // default parameters when selecting a new function from the list
                    {
                        i++;
                        string defaultParams = string.Empty;
                        while (i < line.Length)
                        {
                            c = line[i];
                            if (c != '"')
                                defaultParams += c;

                            i++;
                        }
                        triggerCondition.defaultParams = defaultParams;
                    }

                    if (member == "Category")
                    {
                        i++;
                        string category = string.Empty;
                        while (i < line.Length)
                        {
                            c = line[i];
                            category += c;

                            i++;
                        }
                        triggerCondition.category = category;
                    }
                }
            }
        }
    }
}
