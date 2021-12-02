using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriggerParser.Types;

namespace TriggerParser.TriggerElements
{
    public static class TriggerElementParser
    {
        public static void ParseTriggerElements(string file, int containerId)
        {
            IEnumerable<string> lines = File.ReadLines(file);

            foreach (string line in lines)
            {
                if (line.Length > 2 && line.Substring(0, 2) != "//")
                {
                    if (line.Length > 1 && line.Substring(0, 1) != "_")
                    {
                        var triggerElement = ParseBasicChunk(line);

                        // hack
                        if(containerId == 0)
                            EventContainer.container.Add(triggerElement);
                        if (containerId == 1)
                            ConditionContainer.container.Add(triggerElement);
                        if (containerId == 2)
                            ActionContainer.container.Add(triggerElement);
                    }
                    else
                        ParseExtendedChunks(line);
                }
            }
        }

        /*
         * A typical basic chunk looks like this:
            TriggerRegisterGameStateEventTimeOfDay=0,limitop,real
         *
         * Returns trigger element
         */
        private static TriggerElement ParseBasicChunk(string line)
        {
            string key = string.Empty;
            string version = string.Empty;
            string displayName = string.Empty;
            string category = string.Empty;
            List<TriggerType> arguments = new List<TriggerType>();
            //List<TriggerType> defaultParams = new List<TriggerType>();

            // read line
            int i = 0;

            // key / identifier
            bool foundKey = false;
            while(!foundKey)
            {
                if (line[i] != '=')
                    key += line[i];
                else
                    foundKey = true;

                i++;
            }

            // version
            bool foundVersion = false;
            while(!foundVersion && i < line.Length)
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
            while(i < line.Length)
            {
                if(line[i] == ',')
                {
                    arguments.Add(new TriggerType());
                } else
                {
                    int argIndex = arguments.Count - 1;
                    arguments[argIndex].key += line[i];
                }

                i++;
            }

            // check valid arguments
            foreach (var item in arguments)
            {
                bool found = false;
                for (int it = 0; it < TriggerTypeContainer.variableTypes.Count; it++)
                {
                    var type = TriggerTypeContainer.variableTypes[it];
                    if (type.key == item.key)
                    {
                        found = true;
                        break;
                    }
                }

                // if not found check other container
                if (item == null)
                {
                    for (int it = 0; it < TriggerTypeContainer.otherTypes.Count; it++)
                    {
                        var type = TriggerTypeContainer.otherTypes[it];
                        if (type.key == item.key)
                        {
                            found = true;
                            break;
                        }
                    }
                }

                if (!found)
                    Console.WriteLine($"WARNING: could not find type '{item.key}' on key '{key}'");
            }

                        
            int intVersion = 0;
            if (version != "")
                intVersion = int.Parse(version);

            var triggerElement = new TriggerElement()
            {
                key = key,
                version = intVersion,
                displayName = displayName,
                arguments = arguments,
                category = category
            };

            TriggerElementContainer.container.Add(triggerElement);

            return triggerElement;
        }


        /*
         * Typical extended chunks look like this:
         * // Game events
            _TriggerRegisterGameStateEventTimeOfDay_DisplayName="Time Of Day"
            _TriggerRegisterGameStateEventTimeOfDay_Parameters="The in-game time of day becomes ",~Operation," ",~Time
            _TriggerRegisterGameStateEventTimeOfDay_Defaults=LimitOpEqual,12
            _TriggerRegisterGameStateEventTimeOfDay_Limits=_,_,0,24
            _TriggerRegisterGameStateEventTimeOfDay_Category=TC_GAME
         * 
         */
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
                    TriggerElement triggerEvent = TriggerElementContainer.FindByKey(key);

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
                        triggerEvent.displayName = displayName;
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

                            i++;
                        }
                        triggerEvent.paramText = paramText;
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
                        triggerEvent.defaultParams = defaultParams;
                    }

                    if (member == "Limits")
                    {
                        i++;
                        while (i < line.Length)
                        {
                            i++;
                        }
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
                        triggerEvent.category = category;
                    }
                }
            }
        }
    }
}
