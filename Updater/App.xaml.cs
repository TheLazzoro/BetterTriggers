using System.Configuration;
using System.Data;
using System.Windows;

namespace Updater
{
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message
                + Environment.NewLine
                + Environment.NewLine
                + e.Exception.StackTrace
                , "Install Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

}
