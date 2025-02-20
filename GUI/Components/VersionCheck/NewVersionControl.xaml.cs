using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GUI.Components.VersionCheck
{
    /// <summary>
    /// Interaction logic for NewVersionControl.xaml
    /// </summary>
    public partial class NewVersionControl : UserControl
    {
        public NewVersionControl(VersionCheckCollection versionCheck)
        {
            InitializeComponent();

            textGithub.NavigateUri = new Uri(versionCheck.VersionDTO.html_url);
            textGithub.ToolTip = versionCheck.VersionDTO.html_url;

            txtNewVersion.Text = $"{versionCheck.VersionDTO.name.Replace("Version ", "")}";
            txtOldVersion.Text = $"{versionCheck.CurrentVersion}";
            txtVersionNotes.Text = versionCheck.VersionDTO.body;
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
