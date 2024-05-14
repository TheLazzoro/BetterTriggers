using System;
using System.Media;
using System.Windows;
using System.Windows.Input;


namespace GUI.Components.Dialogs
{
    public partial class CriticalErrorDialog : Window
    {
        public CriticalErrorDialog(Exception ex)
        {
            InitializeComponent();
            Owner = MainWindow.GetMainWindow();

            textBoxException.Text = ex.Message;
            textBoxStacktrace.Text = ex.StackTrace;
            SystemSounds.Exclamation.Play();
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
