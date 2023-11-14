using BetterTriggers;
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
            System.Windows.MessageBox.Show(e.Exception.Message + "\n\nError log saved.", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            Application.Current.Shutdown();
            e.Handled = true;
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            EditorSettings settings = EditorSettings.Load();
            EditorTheme.Change((EditorThemeUnion) settings.editorAppearance);
        }

        public void ChangeTheme(Uri uri)
        {
            Resources.MergedDictionaries.Clear();
            var dict = new ResourceDictionary() { Source = uri };
            Resources.MergedDictionaries.Add(dict);
        }

    }
}
