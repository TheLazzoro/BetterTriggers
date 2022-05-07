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
        static string currentColor = string.Empty;
        static string pattern = @"(^[0-9A-F]{6}$)";
        static string colorCode;

        public static List<Inline> Format(string text)
        {
            List<Inline> inlines = new List<Inline>();
            colorCode = "#FFFFFF";
            text = text.Replace("|n", System.Environment.NewLine);

            string[] split = Regex.Split(text, @"(\|cff)|(\|r)", RegexOptions.IgnoreCase);
            for (int i = 0; i < split.Length; i++)
            {
                if (split[i] == "|r" || split[i] == "|R")
                {
                    colorCode = "#FFFFFF";
                    continue;
                }

                if (split[i] == "|cff" && i < split.Length && split[i+1].Length >= 6)
                {
                    var match = Regex.Match(split[i+1].Substring(0, 6), pattern, RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        colorCode = "#" + match.Value;
                        split[i+1] = split[i+1].Substring(6, split[i+1].Length - 6);
                        continue;
                    }
                }

                Run r = new Run(split[i]);
                r.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom(colorCode);
                inlines.Add(r);
            }

            return inlines;
        }
    }
}
