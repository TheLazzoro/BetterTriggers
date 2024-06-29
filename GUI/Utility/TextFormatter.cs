using BetterTriggers.WorldEdit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
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
        public static List<Inline> War3ColoredText(string text, string defaultColor = "FFFFFFFF")
        {
            List<Inline> inlines = new List<Inline>();
            if (text == null)
                return inlines;

            colorCode = defaultColor;
            text = text.Replace("|n", System.Environment.NewLine);

            string[] split = Regex.Split(text, @"(\|c)|(\|r)", RegexOptions.IgnoreCase);
            for (int i = 0; i < split.Length; i++)
            {
                if (split[i] == "|r" || split[i] == "|R")
                {
                    colorCode = defaultColor;
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


        internal static string JassTypewordBrush;
        internal static string JassKeywordBrush;
        internal static string LuaKeywordBrush;
        internal static string JassFunctionBrush;
        internal static string JassVariableBrush;
        public static List<Inline> CodeColor(string text, ScriptLanguage language)
        {
            if (JassTypewordBrush == null)
            {
                Color c = ((SolidColorBrush)Application.Current.Resources["JassTypewordBrush"]).Color;
                JassTypewordBrush = c.ToString();
            }
            if (JassKeywordBrush == null)
            {
                Color c = ((SolidColorBrush)Application.Current.Resources["JassKeywordBrush"]).Color;
                JassKeywordBrush = c.ToString();
            }
            if (LuaKeywordBrush == null)
            {
                Color c = ((SolidColorBrush)Application.Current.Resources["LuaKeywordBrush"]).Color;
                LuaKeywordBrush = c.ToString();
            }
            if (JassFunctionBrush == null)
            {
                Color c = ((SolidColorBrush)Application.Current.Resources["JassFunctionBrush"]).Color;
                JassFunctionBrush = c.ToString();
            }
            if (JassVariableBrush == null)
            {
                Color c = ((SolidColorBrush)Application.Current.Resources["JassVariableBrush"]).Color;
                JassVariableBrush = c.ToString();
            }

            List<Inline> inlines = new List<Inline>();
            if (text == null)
                return inlines;

            string[] split = text.Split(" ");
            for (int i = 0; i < split.Length; i++)
            {
                var current = split[i];
                Run r = new Run(current);
                if (ScriptData.TypewordsJass.ContainsKey(current))
                    r.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom(JassTypewordBrush);
                else if (ScriptData.KeywordsJass.ContainsKey(current) || ScriptData.KeywordsLua.ContainsKey(current))
                {
                    if(language == ScriptLanguage.Jass)
                        r.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom(JassKeywordBrush);
                    else
                        r.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom(LuaKeywordBrush);
                }
                else if (ScriptData.Natives.ContainsKey(current))
                    r.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom(JassFunctionBrush);
                else
                    r.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom(JassVariableBrush);

                inlines.Add(r);
                inlines.Add(new Run(" "));
            }

            return inlines;
        }
    }
}
