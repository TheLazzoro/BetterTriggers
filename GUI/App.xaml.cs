using BetterTriggers;
using GUI.Components.Dialogs;
using GUI.Components.Settings;
using GUI.Utility;
using System;
using System.Windows;

namespace GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            ErrorLog.Write(e.Exception, "DispatcherUnhandledException");

            CriticalErrorDialog dialog = new CriticalErrorDialog(e.Exception);
            dialog.ShowDialog();

            e.Handled = true;
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += (s, args) =>
            {
                if (args.ExceptionObject is Exception ex)
                    ErrorLog.Write(ex, "AppDomain.UnhandledException");
            };

            EditorSettings settings = EditorSettings.Load();
            EditorTheme.Change(settings.editorAppearance);
        }

        public void ChangeTheme(Uri uri)
        {
            Resources.MergedDictionaries.Clear();
            var dataTemplate = new ResourceDictionary() { Source = new Uri("/Resources/DataTemplate.xaml", UriKind.Relative) };
            var theme = new ResourceDictionary() { Source = uri };
            Resources.MergedDictionaries.Add(dataTemplate);
            Resources.MergedDictionaries.Add(theme);
        }

    }
}
