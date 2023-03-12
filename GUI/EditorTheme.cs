using BetterTriggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace GUI
{
    public enum EditorThemeUnion
    {
        Default,
        Light,
    }

    public static class EditorTheme
    {
        public static void Change(EditorThemeUnion theme)
        {
            Settings settings = Settings.Load();
            settings.editorApperance = (int)theme;
            switch (theme)
            {
                case EditorThemeUnion.Default:
                    DefaultTheme();
                    break;
                case EditorThemeUnion.Light:
                    LightTheme();
                    break;
                default:
                    DefaultTheme();
                    break;
            }
        }

        public static string TreeItemTextColor()
        {
            return "TextBrush";
        }

        public static string HyperlinkColor()
        {
            Settings settings = Settings.Load();
            if(settings.triggerEditorMode == 0)
                return "HyperlinkDefaultBrush";
            else
                return "HyperlinkCliCliBrush";
        }

        public static string HyperlinkHoverColor()
        {
            return "HyperlinkHoverBrush";
        }

        private static void DefaultTheme()
        {
            var app = (App)Application.Current;
            var uri = new Uri("Resources/DarkTheme.xaml", UriKind.Relative);
            app.ChangeTheme(uri);
        }

        private static void LightTheme()
        {
            var app = (App)Application.Current;
            var uri = new Uri("Resources/LightTheme.xaml", UriKind.Relative);
            app.ChangeTheme(uri);
        }
    }
}
