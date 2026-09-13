using BetterTriggers.Utility.IniParser;

namespace BetterTriggers.Utility
{
    public static class IniFileConverter
    {

        internal static IniData GetIniData(string fileContent, bool allowDuplicateSections = true, bool allowDuplicateKeys = true)
        {
            var iniFile = Convert(fileContent);
            //parser.Configuration.AllowDuplicateSections = allowDuplicateSections;
            //parser.Configuration.AllowDuplicateKeys = allowDuplicateKeys;
            var data = new IniData(iniFile);

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
                    textArr[i] = "";
                else if (!textArr[i].Contains("=") && !textArr[i].Contains("["))
                    textArr[i] = "";
            }

            return string.Join("\r\n", textArr);
        }
    }
}
