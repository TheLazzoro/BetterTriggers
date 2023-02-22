using BetterTriggers.WorldEdit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;
using War3Net.Build.Info;

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
        public static List<Inline> War3ColoredText(string text)
        {
            List<Inline> inlines = new List<Inline>();
            if (text == null)
                return inlines;

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

        public static List<Inline> CodeColor(string text, ScriptLanguage language)
        {
            List<Inline> inlines = new List<Inline>();
            if (text == null)
                return inlines;

            string[] split = text.Split(" ");
            for (int i = 0; i < split.Length; i++)
            {
                var current = split[i];
                Run r = new Run(current);
                if (ScriptData.TypewordsJass.ContainsKey(current))
                    r.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#4EC9B0");
                else if (ScriptData.KeywordsJass.ContainsKey(current) || ScriptData.KeywordsLua.ContainsKey(current))
                {
                    if(language == ScriptLanguage.Jass)
                        r.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#569CD6");
                    else
                        r.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#C586C0");
                }
                else if (ScriptData.Natives.ContainsKey(current))
                    r.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#DCDCAA");
                else
                    r.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#9CDCFE");

                inlines.Add(r);
                inlines.Add(new Run(" "));
            }

            return inlines;
        }
    }
}
