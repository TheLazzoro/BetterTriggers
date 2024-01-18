using BetterTriggers;
using GUI.Components.Settings;
using ICSharpCode.Decompiler.Metadata;
using NuGet.Versioning;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup.Localizer;

namespace GUI.Components.VersionCheck
{
    public partial class NewVersionWindow : Window
    {
        public NewVersionWindow()
        {
            this.Owner = MainWindow.GetMainWindow();
            InitializeComponent();

            Task.Run(GetVersion);

        }

        private async Task GetVersion()
        {
            var versionCheck = new VersionCheck(true);
            var version = await versionCheck.GetNewestVersionAsync();

            Application.Current.Dispatcher.Invoke(delegate
            {
                switch (version.VersionCheckEnum)
                {
                    case VersionCheckEnum.IsNewest:
                        progressCircle.Visibility = Visibility.Hidden;
                        imgUpToDate.Visibility = Visibility.Visible;
                        textLoading.Text = "Up to date.";
                        break;
                    case VersionCheckEnum.NewerExists:
                        progressCircle.Visibility = Visibility.Hidden;
                        textLoading.Visibility = Visibility.Hidden;
                        var control = new NewVersionControl(version);
                        grid.Children.Add(control);
                        Grid.SetRow(control, 0);
                        Grid.SetColumn(control, 0);
                        Grid.SetColumnSpan(control, 2);
                        break;
                    case VersionCheckEnum.CouldNotConnect:
                        progressCircle.Visibility = Visibility.Hidden;
                        imgError.Visibility = Visibility.Visible;

                        Hyperlink hyperlink = new Hyperlink(new Run(VersionCheck.url));
                        hyperlink.SetResourceReference(Hyperlink.ForegroundProperty, EditorTheme.HyperlinkColor());
                        hyperlink.Click += Hyperlink_LatestVersionAPI_Click;
                        textLoading.Inlines.Clear();
                        textLoading.Inlines.Add($"Could not get version from: {Environment.NewLine}{Environment.NewLine}");
                        textLoading.Inlines.Add(hyperlink);
                        break;
                }
            });
        }

        private void Hyperlink_LatestVersionAPI_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("explorer", VersionCheck.url);
                e.Handled = true;
            }
            catch (Exception)
            {

            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
