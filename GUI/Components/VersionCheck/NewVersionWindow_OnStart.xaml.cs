using BetterTriggers;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace GUI.Components.VersionCheck
{
    public partial class NewVersionWindow_OnStart : Window
    {
        public NewVersionWindow_OnStart(VersionCheckCollection versionCheck, Window owner)
        {
            this.Owner = owner;

            InitializeComponent();

            var control = new NewVersionControl(versionCheck);
            grid.Children.Add(control);
            Grid.SetRow(control, 0);
            Grid.SetColumn(control, 0);
        }

        private void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            ProgramSettings programSettings = ProgramSettings.Load();
            programSettings.IgnoreNewVersion = (bool)checkboxIgnore.IsChecked;
            ProgramSettings.Save(programSettings);

            this.Close();
            VersionCheck.DownloadUpdate();
        }
    }
}
