using BetterTriggers.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GUI.Components.UserReports
{
    public partial class ReportIssueWindow : Window
    {
        const int MAX_CHARS = 2000;

        public ReportIssueWindow()
        {
            Owner = MainWindow.GetMainWindow();
            InitializeComponent();

            UpdateMaxCharsText();
        }

        private void textBoxReport_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBoxWatermark.Visibility = textBoxReport.Text.Length > 0 ? Visibility.Collapsed : Visibility.Visible;
            UpdateMaxCharsText();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            var service = new LoggingService();
            service.SubmitReport(textBoxReport.Text);

            var dialog = new Dialogs.MessageBox("Issue Submitted", "Your issue has been submitted!", this);
            dialog.ShowDialog();

            this.Close();
        }

        private void UpdateMaxCharsText()
        {
            textBlockMaxChars.Text = $"{textBoxReport.Text.Length}/{MAX_CHARS}";
        }

        private void textBoxReport_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length + textBoxReport.Text.Length > MAX_CHARS)
            {
                e.Handled = true; // cancel
            }
        }
    }
}
