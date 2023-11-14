using BetterTriggers;
using BetterTriggers.Models;
using BetterTriggers.Models.EditorData;
using BetterTriggers.WorldEdit;
using GUI.Components.ChangedTriggers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;

namespace GUI.Components.VerifyTriggers
{
    public partial class VerifyingTriggersWindow : Window
    {
        public event Action OnCloseProject;

        List<IExplorerElement> modifiedElements = new List<IExplorerElement>();
        IdCollisionException collisionError;
        Exception defaultError;

        public VerifyingTriggersWindow()
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
            if (defaultError != null)
            {
                throw defaultError;
            }
            if (collisionError != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append($"{collisionError.Message}{Environment.NewLine}");
                sb.Append($"You need to resolve these manually.{Environment.NewLine}");
                sb.Append($"{Environment.NewLine}");
                if (collisionError.triggers.Count > 0)
                {
                    sb.Append($"Triggers:{Environment.NewLine}");
                    collisionError.triggers.ForEach(t =>
                    {
                        sb.Append($"{t.Item1.GetName()} <-> {t.Item2.GetName()}{Environment.NewLine}");
                    });
                }
                if (collisionError.variables.Count > 0)
                {
                    sb.Append($"{Environment.NewLine}");
                    sb.Append($"Variables:{Environment.NewLine}");
                    collisionError.variables.ForEach(v =>
                    {
                        sb.Append($"{v.Item1.GetName()} <-> {v.Item2.GetName()}{Environment.NewLine}");
                    });
                }
                Dialogs.MessageBox messageBox = new Dialogs.MessageBox("ID Collisions", sb.ToString());
                messageBox.ShowDialog();
                OnCloseProject?.Invoke();
                this.Close();
            }

            if (modifiedElements.Count == 0)
            {
                this.Close();
                return;
            }

            ChangedTriggersWindow changedTriggersWindow = new ChangedTriggersWindow(modifiedElements);
            changedTriggersWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            changedTriggersWindow.Top = this.Top + this.Height / 2 - changedTriggersWindow.Height / 2;
            changedTriggersWindow.Left = this.Left + this.Width / 2 - changedTriggersWindow.Width / 2;
            changedTriggersWindow.Show();

            this.Close();
        }

        private void WorkerVerify_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                modifiedElements = CustomMapData.ReloadMapData();
            }
            catch (IdCollisionException ex)
            {
                this.collisionError = ex;
            }
            catch (Exception ex)
            {
                defaultError = ex;
            }
            (sender as BackgroundWorker).ReportProgress(100);
        }
    }
}
