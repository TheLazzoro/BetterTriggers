using IniParser.Model;
using IniParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.Utility
{
    public static class IniFileConverter
    {
        private static IniDataParser parser = new IniDataParser();

        public static IniData GetIniData(string fileContent, bool allowDuplicateSections = true, bool allowDuplicateKeys = true)
        {
            var iniFile = Convert(fileContent);
            parser.Configuration.AllowDuplicateSections = allowDuplicateSections;
            parser.Configuration.AllowDuplicateKeys = allowDuplicateKeys;
            IniData data = parser.Parse(iniFile);

            return data;
        }

        private static string Convert(string data)
        {
            // fix blizzard ini file
            string[] textArr = data.Split("\r\n");
            string initFile = string.Empty;
            for (int i = 0; i < textArr.Length; i++)
            {
                if (textArr[i].Contains("//"))
                    textArr[i] = textArr[i].Replace("//", ";");
                else if (!textArr[i].Contains("=") && !textArr[i].Contains("["))
                    textArr[i] = "";
            }

            return string.Join("\r\n", textArr);
        }
    }
}
