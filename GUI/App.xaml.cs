using BetterTriggers;
using GUI.Components.Dialogs;
using GUI.Components.Settings;
using System;
using System.IO;
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
            if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "errors")))
                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "errors"));

            File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "errors/Error_" + DateTime.Now.ToString("yyyy/MM/dd HH.mm.ss") + ".txt"), e.Exception.StackTrace);

            CriticalErrorDialog dialog = new CriticalErrorDialog(e.Exception);
            dialog.ShowDialog();

            e.Handled = true;
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
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
