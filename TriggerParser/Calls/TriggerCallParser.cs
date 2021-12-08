using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriggerParser.Types;

namespace TriggerParser.Calls
{
    public static class TriggerCallParser
    {
        public static void ParseCalls(string file)
        {
            IEnumerable<string> lines = File.ReadLines(file);

            foreach (string line in lines)
            {
                if (line.Length > 2 && line.Substring(0, 2) != "//")
                {
                    if (line.Length > 1 && line.Substring(0, 1) != "_")
                    {
                        var call = ParseBasicChunk(line);
                        TriggerCallContainer.container.Add(call);
                        Console.WriteLine("Pared: " + call.key);
                    }
                    else
                        ParseExtendedChunks(line);

                }
            }
        }


        private static TriggerCall ParseBasicChunk(string line)
        {
            string key = string.Empty;
            string version = string.Empty;
            string returnType = string.Empty;
            string canBeUsedInEvent = string.Empty;
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
                    version += line[i];
                else
                    foundVersion = true;

                i++;
            }

            // indicating if the call can be used in events
            bool foundEventValidity = false;
            while (!foundEventValidity && i < line.Length)
            {
                if (line[i] != ',')
                    canBeUsedInEvent += line[i];
                else
                    foundEventValidity = true;

                i++;

            }

            // return type
            bool foundReturnType = false;
            while (!foundReturnType && i < line.Length)
            {
                if (line[i] != ',')
                {
                    returnType += line[i];
                    i++;
                }
                else
                    foundReturnType = true;

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
            int intCanBeUsedInEvent = 0;
            if (version != "")
                intVersion = int.Parse(version);
            if (canBeUsedInEvent != "")
                intCanBeUsedInEvent = int.Parse(canBeUsedInEvent);


            var condition = new TriggerCall()
            {
                key = key,
                version = intVersion,
                canBeUsedInEvent = intCanBeUsedInEvent,
                arguments = arguments,
                returnType = returnType
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
                    TriggerCall triggerCall = TriggerCallContainer.FindByKey(key);

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
                        triggerCall.displayName = displayName;
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

                            if (c == ',')
                            {
                            }

                            i++;
                        }
                        triggerCall.paramText = paramText;
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
                        triggerCall.defaultParams = defaultParams;
                    }

                    if (member.ToLower() == "category")
                    {
                        i++;
                        string category = string.Empty;
                        while (i < line.Length)
                        {
                            c = line[i];
                            category += c;

                            i++;
                        }
                        triggerCall.category = category;
                    }

                    if (member == "Limits") // NOT IMPLEMENTED
                        break;
                }
            }
        }
    }
}
