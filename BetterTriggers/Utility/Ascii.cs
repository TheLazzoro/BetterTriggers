using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BetterTriggers.Utility
{
    public static class Ascii
    {        
        /// <summary>
        /// Takes an already generated varname and replaces invalid ASCII chars.
        /// </summary>
        /// <param name="text">Input.</param>
        /// <param name="isScriptGenerate">The the final script does not accept underscores '_' at the end of a variable or function name.
        /// We therefore append a char at the end.</param>
        /// <returns>Output.</returns>
        public static string ReplaceNonASCII(string text, bool isScriptGenerate = false)
        {
            string output = Regex.Replace(text, @"[^\u0000-\u007F]+", "__");
            for (int i = 0; i < output.Length; i++)
            {
                char c = output[i];
                if(!char.IsLetterOrDigit(c)) {
                    output = output.Remove(i, 1);
                    output = output.Insert(i, "_");
                }

            }
            if (isScriptGenerate && output != text)
                output = output + "u"; // vanilla WE also does this... so yeah
            while(isScriptGenerate && output.EndsWith("_"))
            {
                output = output.Substring(0, output.Length-1);
            }

            return output;
        }
    }
}
