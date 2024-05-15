using GUI.Components.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace GUI.Components.BuildMap
{
    public partial class BuildMapWindow : Window
    {
        private BackgroundWorker _worker;

        public BuildMapWindow()
        {
            InitializeComponent();

            _worker = new BackgroundWorker();
            _worker.WorkerReportsProgress = true;
            _worker.DoWork += worker_DoWork;
            _worker.ProgressChanged += worker_ProgressChanged;
            _worker.RunWorkerCompleted += worker_RunWorkerCompleted;
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            gifAcolyte.Visibility = Visibility.Visible;
            progressBar.IsIndeterminate = true;
            btnExport.IsEnabled = false;
            btnCancel.IsEnabled = false;
            checkBoxRemoveListfile.IsEnabled = false;
            checkBoxTriggerData.IsEnabled = false;
            _worker.RunWorkerAsync();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (_worker.IsBusy)
            {
                var dialog = new DialogBox("Confirmation", $"Exporting is still in progress.{Environment.NewLine}{Environment.NewLine}Are you sure you want to close this window?");
                dialog.ShowDialog();
                e.Cancel = !dialog.OK;
            }
        }

        private void worker_DoWork(object? sender, DoWorkEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void worker_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void worker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            throw new NotImplementedException();
        }

    }
}
