using BetterTriggers.Logging;
using System;
using System.Media;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;


namespace GUI.Components.Dialogs
{
    public partial class CriticalErrorDialog : Window
    {
        private Exception _exception;

        public CriticalErrorDialog(Exception ex)
        {
            InitializeComponent();
            Owner = MainWindow.GetMainWindow();
            _exception = ex;

            textBoxException.Text = ex.Message;
            textBoxStacktrace.Text = ex.StackTrace;
            SystemSounds.Exclamation.Play();

            Task.Factory.StartNew(SubmitError);
        }

        private void SubmitError()
        {
            var service = new LoggingService();
            service.SubmitError(_exception);
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
