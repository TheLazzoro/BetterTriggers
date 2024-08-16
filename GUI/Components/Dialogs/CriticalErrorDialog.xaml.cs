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

        private async void SubmitError()
        {
            var service = new LoggingService();
            await service.SubmitError_Async(_exception);
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

        private async void btnSubmitReport_Click(object sender, RoutedEventArgs e)
        {
            btnSubmitReport.IsEnabled = false;
            textBoxReport.IsEnabled = false;
            progressBar.Visibility = Visibility.Visible;
            var service = new LoggingService();
            await service.SubmitError_Async(_exception, textBoxReport.Text);

            greenCheck.Visibility = Visibility.Visible;
            progressBar.Visibility = Visibility.Collapsed;
            textResponseMsg.Text = "Your error report has been submitted!";
        }
    }
}
