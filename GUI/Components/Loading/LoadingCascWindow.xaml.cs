using BetterTriggers.WorldEdit.GameDataReader;
using GUI.Components.Setup;
using System;
using System.ComponentModel;
using System.Windows;

namespace GUI.Components.Loading
{
    public partial class LoadingCascWindow : Window
    {
        bool isCascValid = false;
        BackgroundWorker workerVerify;

        public LoadingCascWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            workerVerify = new BackgroundWorker();
            workerVerify.DoWork += WorkerVerify_DoWork;
            workerVerify.WorkerReportsProgress = true;
            workerVerify.ProgressChanged += WorkerVerify_ProgressChanged;
            workerVerify.RunWorkerAsync();
        }

        private void WorkerVerify_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 0)
            {
                SetupWindow window = new SetupWindow();
                window.Show();
                this.Close();
            }
            else if (e.ProgressPercentage == 50)
            {
                lblInfo.Content = "Loading Warcraft III data...";
            }
            else if (e.ProgressPercentage == 75)
            {
                lblInfo2.Content = BetterTriggers.Init.NextData + "...";
            }
            else if (e.ProgressPercentage == 100)
            {
                MainWindow window = new MainWindow();
                window.Show();
                this.Close();
            }
        }

        private void WorkerVerify_DoWork(object sender, DoWorkEventArgs e)
        {
            BetterTriggers.Init.OnNextData += Init_NextData;

            string error;
            (isCascValid, error) = WarcraftStorageReader.Load();
            if (isCascValid)
            {
                try
                {
                    (sender as BackgroundWorker).ReportProgress(50);
                    BetterTriggers.Init.Initialize(false);
                    (sender as BackgroundWorker).ReportProgress(100);
                }
                catch (Exception)
                {
                    (sender as BackgroundWorker).ReportProgress(0);
                }
            }
            else
                (sender as BackgroundWorker).ReportProgress(0);

            BetterTriggers.Init.OnNextData -= Init_NextData;
        }

        private void Init_NextData()
        {
            workerVerify.ReportProgress(75);
        }
    }
}
