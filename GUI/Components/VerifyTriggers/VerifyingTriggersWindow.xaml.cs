﻿using BetterTriggers;
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
                string[] items = new string[collisionError.IdCollisions.Count + collisionError.IdCollisions.Count];
                if (collisionError.IdCollisions.Count > 0)
                {
                    collisionError.IdCollisions.ForEach(t =>
                    {
                        items[index] = $"{t.Item1.GetName()} <-> {t.Item2.GetName()}";
                        index++;
                    });
                }
                
                string message = $"{collisionError.Message}{Environment.NewLine}{Environment.NewLine}Triggers or variables with the same ID are not allowed.{Environment.NewLine}You need to resolve these manually in a text editor.{Environment.NewLine}";
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
                Application.Current.Dispatcher.Invoke(() =>
                {
                    modifiedElements = CustomMapData.ReloadMapData();
                });
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
