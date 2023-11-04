using BetterTriggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace GUI.Components.Settings
{
    public enum EditorThemeUnion
    {
        Default,
        Light,
        Night,
    }

    public static class EditorTheme
    {
        public static void Change(EditorThemeUnion theme)
        {
            EditorSettings settings = EditorSettings.Load();
            settings.editorAppearance = (int)theme;
            switch (theme)
            {
                case EditorThemeUnion.Default:
                    DefaultTheme();
                    break;
                case EditorThemeUnion.Light:
                    LightTheme();
                    break;
                case EditorThemeUnion.Night:
                    NightTheme();
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
            EditorSettings settings = EditorSettings.Load();
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
            var uri = new Uri("Resources/Themes/Dark.xaml", UriKind.Relative);
            app.ChangeTheme(uri);
        }

        private static void LightTheme()
        {
            var app = (App)Application.Current;
            var uri = new Uri("Resources/Themes/Light.xaml", UriKind.Relative);
            app.ChangeTheme(uri);
        }

        private static void NightTheme()
        {
            var app = (App)Application.Current;
            var uri = new Uri("Resources/Themes/Night.xaml", UriKind.Relative);
            app.ChangeTheme(uri);
        }
    }
}
