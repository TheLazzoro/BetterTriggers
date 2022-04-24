using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.Utility
{
    public static class IniFileConverter
    {
        public static string Convert(string data)
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
