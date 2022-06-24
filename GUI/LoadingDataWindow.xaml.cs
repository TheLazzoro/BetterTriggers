using BetterTriggers.WorldEdit;
using System.ComponentModel;
using System.Windows;

namespace GUI
{
    public partial class LoadingDataWindow : Window
    {
        string mapPath;

        public LoadingDataWindow(string mapPath)
        {
            InitializeComponent();
            this.mapPath = mapPath;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BackgroundWorker workerVerify = new BackgroundWorker();
            workerVerify.DoWork += WorkerVerify_DoWork;
            workerVerify.WorkerReportsProgress = true;
            workerVerify.ProgressChanged += WorkerVerify_ProgressChanged;
            workerVerify.RunWorkerAsync();
        }

        private void WorkerVerify_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.Close();
        }

        private void WorkerVerify_DoWork(object sender, DoWorkEventArgs e)
        {
            BetterTriggers.CustomMapData.Init(mapPath);
            (sender as BackgroundWorker).ReportProgress(100);
        }
    }
}
