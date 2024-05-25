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
    public static class EditorTheme
    {
        public static void Change(EditorAppearance theme)
        {
            EditorSettings settings = EditorSettings.Load();
            settings.editorAppearance = theme;
            switch (theme)
            {
                case EditorAppearance.Dark:
                    DefaultTheme();
                    break;
                case EditorAppearance.Light:
                    LightTheme();
                    break;
                case EditorAppearance.Night:
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
            if(settings.triggerEditorMode == TriggerEditorMode.Default)
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
