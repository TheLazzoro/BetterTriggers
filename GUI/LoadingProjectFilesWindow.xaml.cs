using BetterTriggers.Controllers;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.WorldEdit;
using System.ComponentModel;
using System.Windows;

namespace GUI
{
    public partial class LoadingProjectFilesWindow : Window
    {
        public War3Project project;
        private string projectPath;
        private BackgroundWorker worker;

        public LoadingProjectFilesWindow(string projectPath)
        {
            InitializeComponent();
            this.projectPath = projectPath;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            worker = new BackgroundWorker();
            worker.DoWork += WorkerVerify_DoWork;
            worker.WorkerReportsProgress = true;
            worker.ProgressChanged += WorkerVerify_ProgressChanged;
            worker.RunWorkerAsync();
        }

        private void WorkerVerify_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            lblInfo.Content = $"Loading Project Files {filesLoaded}/{totalFiles}";
            if (e.ProgressPercentage == 100 && project != null)
                this.Close();
        }

        int filesLoaded = 0;
        int totalFiles = 0;
        private void WorkerVerify_DoWork(object sender, DoWorkEventArgs e)
        {
            ControllerProject controllerProject = new ControllerProject();
            controllerProject.FileLoadEvent += ControllerProject_FileLoadEvent;
            project = controllerProject.LoadProject(projectPath);
            worker.ReportProgress(100);
        }

        private void ControllerProject_FileLoadEvent(int arg1, int arg2)
        {
            filesLoaded = arg1;
            totalFiles = arg2;
            float percent = (float)filesLoaded / (float)totalFiles * 100f;
            worker.ReportProgress((int)percent);
        }
    }
}
