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
                if (line.Length > 2 && line.Substring(0, 2) != "//")
                {
                    var constant = ParseBasicChunk(line);
                    TriggerParamContainer.container.Add(constant);
                }
            }
        }


        private static TriggerParam ParseBasicChunk(string line)
        {
            string key = string.Empty;
            string version = string.Empty;
            string variableType = string.Empty;
            string codeText = string.Empty;
            string displayName = string.Empty;

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

            // Variable type
            bool foundVariableType = false;
            while (!foundVariableType && i < line.Length)
            {
                if (line[i] != ',')
                    variableType += line[i];
                else
                    foundVariableType = true;

                i++;
            }

            // code text (used by the script)
            bool foundCodeText = false;
            while (!foundCodeText && i < line.Length)
            {
                if (line[i] != ',')
                    codeText += line[i];
                else
                    foundCodeText = true;

                i++;
            }

            // display name (used by the dropdown and hyperlink)
            bool foundDisplayName = false;
            while (!foundDisplayName && i < line.Length)
            {
                if (line[i] != ',')
                {
                    displayName += line[i];
                    i++;
                }
                else
                    foundDisplayName = true;

            }


            int intVersion = 0;
            if (version != "")
                intVersion = int.Parse(version);

            var constantElement = new TriggerParam()
            {
                key = key,
                version = intVersion,
                displayText = displayName,
                variableType = variableType,
                codeText = codeText
            };

            TriggerParamContainer.container.Add(constantElement);

            return constantElement;
        }
    }
}
