using BetterTriggers.WorldEdit;
using System.ComponentModel;
using System.Windows;

namespace GUI
{
    public partial class LoadingCascWindow : Window
    {
        bool isCascValid = false;


        public LoadingCascWindow()
        {
            InitializeComponent();
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
            /*
            if(e.ProgressPercentage == 0)
                lblInfo.Content = "Loading CASC data...";
            */

            if(e.ProgressPercentage == 0)
            {
                SetupWindow window = new SetupWindow();
                window.Show();
                this.Close();
            }
            else if(e.ProgressPercentage == 100)
            {
                MainWindow window = new MainWindow();
                window.Show();
                this.Close();
            }
        }

        private void WorkerVerify_DoWork(object sender, DoWorkEventArgs e)
        {
            isCascValid = Casc.Load();
            if(isCascValid)
            {
                BetterTriggers.Init.Initialize();
                (sender as BackgroundWorker).ReportProgress(100);
            }
            else
                (sender as BackgroundWorker).ReportProgress(0);
        }
    }
}
