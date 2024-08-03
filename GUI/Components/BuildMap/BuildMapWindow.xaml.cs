using BetterTriggers;
using BetterTriggers.Containers;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.TestMap;
using BetterTriggers.Utility;
using GUI.Components.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using War3Net.Build.Info;

namespace GUI.Components.BuildMap
{
    public partial class BuildMapWindow : Window
    {
        private Thread _thread;
        private Exception _error;
        private event Action _finished;

        private BuildMapViewModel _viewModel;

        public BuildMapWindow()
        {
            Owner = MainWindow.GetMainWindow();
            InitializeComponent();

            _viewModel = new BuildMapViewModel();
            DataContext = _viewModel;

            var settings = EditorSettings.Load();
            War3Project project = Project.CurrentProject.war3project;
            var language = project.Language == "lua" ? ScriptLanguage.Lua : ScriptLanguage.Jass;
            checkBoxRemoveListfile.IsChecked = settings.Export_RemoveListfile;
            checkBoxTriggerData.IsChecked = settings.Export_RemoveTriggerData;
            checkBoxIncludeTriggerData.IsChecked = settings.Export_IncludeTriggerData;
            checkBoxObfuscate.IsChecked = settings.Export_Obfuscate;
            checkBoxCompress.IsChecked = settings.Export_Compress;
            checkBoxAdvanced.IsChecked = settings.Export_Compress_Advanced;
            textboxBlockSize.Value = settings.Export_Compress_BlockSize;
            HandleRemoveTriggerData();
            if (language != ScriptLanguage.Jass)
            {
                checkBoxObfuscate.IsEnabled = false;
                checkBoxObfuscate.Content += " (Only available in Jass mode)";
            }

            _thread = new Thread(ExportMapAsync);
            _finished += Export_Finished;
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            var settings = EditorSettings.Load();
            settings.Export_IncludeTriggerData = (bool)checkBoxIncludeTriggerData.IsChecked;
            settings.Export_Compress = (bool)checkBoxCompress.IsChecked;
            settings.Export_Compress_Advanced = (bool)checkBoxAdvanced.IsChecked;
            settings.Export_Compress_BlockSize = (ushort)textboxBlockSize.Value;
            settings.Export_RemoveListfile = (bool)checkBoxRemoveListfile.IsChecked;
            settings.Export_RemoveTriggerData = (bool)checkBoxTriggerData.IsChecked;
            settings.Export_Obfuscate = (bool)checkBoxObfuscate.IsChecked;

            imgMap.Visibility = Visibility.Hidden;
            gifAcolyte.Visibility = Visibility.Visible;
            progressBar.IsIndeterminate = true;
            btnExport.IsEnabled = false;
            btnCancel.IsEnabled = false;
            checkBoxIncludeTriggerData.IsEnabled = false;
            checkBoxCompress.IsEnabled = false;
            checkBoxAdvanced.IsEnabled = false;
            textboxBlockSize.IsEnabled = false;
            checkBoxRemoveListfile.IsEnabled = false;
            checkBoxTriggerData.IsEnabled = false;
            checkBoxObfuscate.IsEnabled = false;
            _thread.Start();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (_thread.ThreadState == ThreadState.Running)
            {
                var dialog = new DialogBox("Confirmation", $"Exporting is still in progress.{Environment.NewLine}{Environment.NewLine}Are you sure you want to close this window?");
                dialog.ShowDialog();
                e.Cancel = !dialog.OK;
            }
        }

        private void ExportMapAsync()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _error = null;
            });

            try
            {
                Builder builder = new Builder();
                builder.BuildMap(includeMPQSettings: true);
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _error = ex;
                });
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                _finished?.Invoke();
            });
        }

        private void Export_Finished()
        {
            progressBar.IsIndeterminate = false;
            if (_error == null)
            {
                Title = "Export Complete!";
                progressBar.Value = 100;
                gifAcolyte.Visibility = Visibility.Hidden;
                imgMap.Visibility = Visibility.Visible;
                btnShowFolder.IsEnabled = true;
            }
            else
            {
                Title = "Export Failed";
                gifAcolyte.Visibility = Visibility.Hidden;
                imgMap.Visibility = Visibility.Visible;
                progressBar.Foreground = (Brush)FindResource("TextErrorBrush");
                progressBar.Value = 100;
                Dialogs.MessageBox dialog = new Dialogs.MessageBox("Error", _error.Message, this);
                dialog.ShowDialog();
            }
        }

        private void btnShowFolder_Click(object sender, RoutedEventArgs e)
        {
            FileSystemUtil.OpenInExplorer(Project.CurrentProject.dist, false);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void checkBoxTriggerData_Click(object sender, RoutedEventArgs e)
        {
            HandleRemoveTriggerData();
        }

        private void HandleRemoveTriggerData()
        {
            bool removeTriggerData = checkBoxTriggerData.IsChecked == true;
            checkBoxIncludeTriggerData.IsEnabled = !removeTriggerData;
            if (removeTriggerData)
            {
                checkBoxIncludeTriggerData.IsChecked = false;
            }
        }
    }
}
