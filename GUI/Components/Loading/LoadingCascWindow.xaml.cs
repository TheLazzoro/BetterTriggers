using BetterTriggers.WorldEdit.GameDataReader;
using GUI.Components.Setup;
using GUI.Utility;
using System;
using System.ComponentModel;
using System.Windows;

namespace GUI.Components.Loading
{
    public partial class LoadingCascWindow : Window
    {
        bool isStorageValid = false;
        BackgroundWorker workerVerify;
        string? loadError;

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
                SetupWindow window = new SetupWindow(loadError);
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
            try
            {
                (isStorageValid, error) = WarcraftStorageReader.Load();
            }
            catch (Exception ex)
            {
                SetLoadError(ex, "Failed to open Warcraft III game data.");
                (sender as BackgroundWorker).ReportProgress(0);
                BetterTriggers.Init.OnNextData -= Init_NextData;
                return;
            }
            if (isStorageValid)
            {
                try
                {
                    (sender as BackgroundWorker).ReportProgress(50);
                    BetterTriggers.Init.Initialize(false);
                    (sender as BackgroundWorker).ReportProgress(100);
                }
                catch (Exception ex)
                {
                    SetLoadError(ex, $"Failed to load Warcraft III data ({BetterTriggers.Init.NextData}).");
                    (sender as BackgroundWorker).ReportProgress(0);
                }
            }
            else
                (sender as BackgroundWorker).ReportProgress(0);

            BetterTriggers.Init.OnNextData -= Init_NextData;
        }

        private void SetLoadError(Exception ex, string context)
        {
            string? logPath = ErrorLog.Write(ex, context);
            loadError = $"{context}{Environment.NewLine}{Environment.NewLine}{ex.GetType().Name}: {ex.Message}";
            if (logPath != null)
                loadError += $"{Environment.NewLine}{Environment.NewLine}Details were written to:{Environment.NewLine}{logPath}";
        }

        private void Init_NextData()
        {
            workerVerify.ReportProgress(75);
        }
    }
}
