using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;

namespace GUI.Utility
{
    public static class TextFormatter
    {
        static string pattern = @"(^[0-9A-F]{8}$)";
        static string colorCode;

        /// <summary>
        /// WC3 text formatting with colors and new lines.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static List<Inline> Format(string text)
        {
            List<Inline> inlines = new List<Inline>();
            colorCode = "FFFFFFFF";
            text = text.Replace("|n", System.Environment.NewLine);

            string[] split = Regex.Split(text, @"(\|c)|(\|r)", RegexOptions.IgnoreCase);
            for (int i = 0; i < split.Length; i++)
            {
                if (split[i] == "|r" || split[i] == "|R")
                {
                    colorCode = "FFFFFFFF";
                    continue;
                }

                if (split[i].ToLower() == "|c" && i < split.Length && split[i+1].Length >= 8)
                {
                    var match = Regex.Match(split[i+1].Substring(0, 8), pattern, RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        colorCode = match.Value;
                        split[i+1] = split[i+1].Substring(8, split[i+1].Length - 8);
                        continue;
                    }
                }

                Run r = new Run(split[i]);
                r.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#" + colorCode.Substring(2, colorCode.Length-2));
                inlines.Add(r);
            }

            return inlines;
        }
    }
}
