using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
    }

}
