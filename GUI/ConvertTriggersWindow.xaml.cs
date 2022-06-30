using BetterTriggers.WorldEdit;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace GUI
{
    /// <summary>
    /// Interaction logic for ConvertTriggersWindow.xaml
    /// </summary>
    public partial class ConvertTriggersWindow : Window
    {
        bool MapExists;
        bool DestinationDirectoryExists;
        bool FinalPathAlreadyExists;
        string FinalPath = string.Empty;

        public ConvertTriggersWindow()
        {
            InitializeComponent();
            Owner = MainWindow.GetMainWindow();
        }

        private void btnSelectMap_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (dialog.SelectedPath != "")
                    lblMap.Text = dialog.SelectedPath;

                VerifyPaths();
            }
        }

        private void btnDestination_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (dialog.SelectedPath != "")
                    lblDestination.Text = dialog.SelectedPath;

                VerifyPaths();
            }
        }

        private bool VerifyPaths()
        {
            bool ok;
            FinalPath = System.IO.Path.Combine(lblDestination.Text, System.IO.Path.GetFileNameWithoutExtension(lblMap.Text));
            MapExists = File.Exists(System.IO.Path.Combine(lblMap.Text, "war3map.w3i"));
            DestinationDirectoryExists = Directory.Exists(lblDestination.Text);
            FinalPathAlreadyExists = Directory.Exists(FinalPath);

            ok = MapExists && DestinationDirectoryExists && !FinalPathAlreadyExists;
            btnConvert.IsEnabled = ok;

            return ok;
        }

        private void btnConvert_Click(object sender, RoutedEventArgs e)
        {
            if (!VerifyPaths())
                return;

            TriggerConverter converter = new TriggerConverter();
            converter.Convert(lblMap.Text, FinalPath);
        }
    }
}
