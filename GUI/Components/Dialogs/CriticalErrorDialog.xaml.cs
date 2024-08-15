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

            SubmitError();
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

        private void textBoxReport_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            textBoxWatermark.Visibility = textBoxReport.Text.Length > 0 ? Visibility.Collapsed : Visibility.Visible;
            btnSubmitReport.IsEnabled = textBoxReport.Text.Length > 0;
        }

        private void btnSubmitReport_Click(object sender, RoutedEventArgs e)
        {
            btnSubmitReport.IsEnabled = false;
            textBoxReport.IsEnabled = false;
            var service = new LoggingService();
            service.SubmitError(_exception, textBoxReport.Text);

            var dialog = new MessageBox("Error Submitted", "Your error report was submitted!", this);
            dialog.ShowDialog();
        }
    }
}
