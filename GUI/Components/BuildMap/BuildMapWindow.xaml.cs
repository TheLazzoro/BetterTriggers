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

        public BuildMapWindow()
        {
            Owner = MainWindow.GetMainWindow();
            InitializeComponent();

            var settings = EditorSettings.Load();
            War3Project project = Project.CurrentProject.war3project;
            var language = project.Language == "lua" ? ScriptLanguage.Lua : ScriptLanguage.Jass;
            checkBoxRemoveListfile.IsChecked = settings.Export_RemoveListfile;
            checkBoxTriggerData.IsChecked = settings.Export_RemoveTriggerData;
            checkBoxObfuscate.IsChecked = settings.Export_Obfuscate;
            if(language != ScriptLanguage.Jass)
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
            settings.Export_RemoveListfile = (bool)checkBoxRemoveListfile.IsChecked;
            settings.Export_RemoveTriggerData = (bool)checkBoxTriggerData.IsChecked;
            settings.Export_Obfuscate = (bool)checkBoxObfuscate.IsChecked;

            imgMap.Visibility = Visibility.Hidden;
            gifAcolyte.Visibility = Visibility.Visible;
            progressBar.IsIndeterminate = true;
            btnExport.IsEnabled = false;
            btnCancel.IsEnabled = false;
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
                Dialogs.MessageBox dialog = new Dialogs.MessageBox("Error", _error.Message, this);
                dialog.ShowDialog();
            }
        }

        private void btnShowFolder_Click(object sender, RoutedEventArgs e)
        {
            FileSystemUtil.OpenInExplorer(Project.CurrentProject.dist);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
