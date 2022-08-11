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
            if (isScriptGenerate && output != text)
                output = output + "u";
            while(isScriptGenerate && output.EndsWith("_"))
            {
                output = output.Substring(0, output.Length-1);
            }

            return output;
        }
    }
}
