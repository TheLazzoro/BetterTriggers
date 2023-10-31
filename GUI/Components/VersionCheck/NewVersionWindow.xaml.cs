using System;
using System.Diagnostics;
using System.Windows;

namespace GUI.Components.VersionCheck
{
    public partial class NewVersionWindow : Window
    {
        public NewVersionWindow(VersionDTO versionDTO, string currentVersion)
        {
            this.Owner = MainWindow.GetMainWindow();

            InitializeComponent();

            textGithub.NavigateUri = new Uri(versionDTO.html_url);
            textGithub.ToolTip = versionDTO.html_url;

            txtNewVersion.Text = $"New version: {versionDTO.name.Replace("Version ", "")}";
            txtOldVersion.Text = $"Current version: {currentVersion}";
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start("explorer", e.Uri.ToString());
                e.Handled = true;
            }
            catch (Exception)
            {

            }
        }
    }
}
