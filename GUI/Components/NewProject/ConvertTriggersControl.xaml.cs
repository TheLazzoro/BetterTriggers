using BetterTriggers.Containers;
using BetterTriggers.WorldEdit;
using GUI.Components.OpenMap;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace GUI.Components.NewProject
{
    public partial class ConvertTriggersControl : UserControl, INewProjectControl
    {
        bool MapExists;
        bool DestinationDirectoryExists;
        bool FinalPathAlreadyExists;
        string FinalPath = string.Empty;
        string mapPath;
        public string ProjectLocation { get; set; }

        BackgroundWorker worker;
        public event Action OnOpenProject;


        public ConvertTriggersControl()
        {
            InitializeComponent();

            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += WorkerVerify_DoWork;
            worker.ProgressChanged += WorkerVerify_ProgressChanged;
        }


        private void btnSelectMap_Click(object sender, RoutedEventArgs e)
        {
            OpenWar3MapWindow window = new OpenWar3MapWindow();
            window.ShowDialog();
            if (!window.OK)
                return;

            lblMap.Text = window.SelectedPath;
            VerifyPaths();
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
            MapExists = Project.CurrentProject.VerifyMapPath(lblMap.Text);
            DestinationDirectoryExists = Directory.Exists(lblDestination.Text);
            FinalPathAlreadyExists = Directory.Exists(FinalPath);

            if (!MapExists)
            {
                lblError.Visibility = Visibility.Visible;
                lblError.Content = "Invalid map directory";
            }
            else if (FinalPathAlreadyExists)
            {
                lblError.Visibility = Visibility.Visible;
                lblError.Content = "Project directory already exists";
            }
            else
                lblError.Visibility = Visibility.Hidden;


            ok = MapExists && DestinationDirectoryExists && !FinalPathAlreadyExists;
            btnConvert.IsEnabled = ok;

            return ok;
        }

        private void btnConvert_Click(object sender, RoutedEventArgs e)
        {
            if (!VerifyPaths())
                return;

            mapPath = lblMap.Text;
            btnCancel.IsEnabled = false;
            btnConvert.Visibility = Visibility.Hidden;
            progressBar.Visibility = Visibility.Visible;
            lblConverting.Visibility = Visibility.Visible;
            worker.RunWorkerAsync();
        }

        private void WorkerVerify_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            btnCancel.IsEnabled = true;
            btnCancel.Content = "Close";
            lblConverting.Content = "Conversion Complete!";
            progressBar.IsIndeterminate = false;
            progressBar.Value = 100;
            btnOpen.Visibility = Visibility.Visible;
        }

        private void WorkerVerify_DoWork(object sender, DoWorkEventArgs e)
        {
            TriggerConverter converter = new TriggerConverter();
            ProjectLocation = converter.Convert(mapPath, FinalPath);
            (sender as BackgroundWorker).ReportProgress(100);
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            OnOpenProject?.Invoke();
        }
    }
}
