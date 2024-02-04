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

        List<ExplorerElement> modifiedElements = new List<ExplorerElement>();
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
                int index = 0;
                string[] items = new string[collisionError.triggers.Count + collisionError.variables.Count];
                if (collisionError.triggers.Count > 0)
                {
                    collisionError.triggers.ForEach(t =>
                    {
                        items[index] = $"{t.Item1.GetName()} <-> {t.Item2.GetName()}{Environment.NewLine}";
                        index++;
                    });
                }
                if (collisionError.variables.Count > 0)
                {
                    collisionError.variables.ForEach(v =>
                    {
                        items[index] = $"{v.Item1.GetName()} <-> {v.Item2.GetName()}{Environment.NewLine}";
                        index++;
                    });
                }
                string message = $"{collisionError.Message}{Environment.NewLine}{Environment.NewLine}You need to resolve these manually.{Environment.NewLine}";
                Dialogs.MessageBoxWithList messageBox = new Dialogs.MessageBoxWithList("ID Collisions", message, items);
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
