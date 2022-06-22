using BetterTriggers;
using BetterTriggers.Containers;
using BetterTriggers.Controllers;
using GUI.Components;
using GUI.Components.TriggerExplorer;
using GUI.Controllers;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TriggerExplorer triggerExplorer;
        TreeItemExplorerElement selectedExplorerItem;
        static Window instance;


        public MainWindow()
        {
            InitializeComponent();
            instance = this;

            BetterTriggers.Init.Initialize();
            CustomMapData.OnSaving += CustomMapData_OnSaving;

            btnNewMap.ToolTip           = Locale.Translate(btnNewMap.ToolTip as string);
            btnOpenMap.ToolTip          = Locale.Translate(btnOpenMap.ToolTip as string);
            
            btnUndo.ToolTip             = Locale.Translate(btnUndo.ToolTip as string);
            btnRedo.ToolTip             = Locale.Translate(btnRedo.ToolTip as string);
            btnCut.ToolTip              = Locale.Translate(btnCut.ToolTip as string);
            btnCopy.ToolTip             = Locale.Translate(btnCopy.ToolTip as string);
            btnPaste.ToolTip            = Locale.Translate(btnPaste.ToolTip as string);

            btnCreateFolder.ToolTip     = Locale.Translate(btnCreateFolder.ToolTip as string);
            btnCreateTrigger.ToolTip    = Locale.Translate(btnCreateTrigger.ToolTip as string);
            btnCreateScript.ToolTip     = Locale.Translate(btnCreateScript.ToolTip as string);
            btnCreateVariable.ToolTip   = Locale.Translate(btnCreateVariable.ToolTip as string);

            btnCreateEvent.ToolTip      = Locale.Translate(btnCreateEvent.ToolTip as string);
            btnCreateCondition.ToolTip  = Locale.Translate(btnCreateCondition.ToolTip as string);
            btnCreateAction.ToolTip     = Locale.Translate(btnCreateAction.ToolTip as string);
            btnSaveScript.ToolTip       = Locale.Translate(btnSaveScript.ToolTip as string);

            btnTestMap.ToolTip          = Locale.Translate(btnTestMap.ToolTip as string);
            btnBuildMap.ToolTip         = Locale.Translate(btnBuildMap.ToolTip as string);


            menuFile.Header             = Locale.Translate(menuFile.Header as string);
            menuOpen.Header             = Locale.Translate(menuOpen.Header as string);


            menuEdit.Header             = Locale.Translate(menuEdit.Header as string);


            menuNew.Header              = Locale.Translate(menuNew.Header as string);
            menuNewCategory.Header      = Locale.Translate(menuNewCategory.Header as string);
            menuNewTrigger.Header       = Locale.Translate(menuNewTrigger.Header as string);
            menuNewScript.Header        = Locale.Translate(menuNewScript.Header as string);
            menuNewVariable.Header      = Locale.Translate(menuNewVariable.Header as string);

            menuNewEvent.Header         = Locale.Translate(menuNewEvent.Header as string);
            menuNewCondition.Header     = Locale.Translate(menuNewCondition.Header as string);
            menuNewAction.Header        = Locale.Translate(menuNewAction.Header as string);


            menuJassHelper.Header       = Locale.Translate(menuJassHelper.Header as string);
            menuEnableJassHelper.Header = Locale.Translate(menuEnableJassHelper.Header as string);
            menuEnableVJass.Header      = Locale.Translate(menuEnableVJass.Header as string);
            menuEnableDebugMode.Header  = Locale.Translate(menuEnableDebugMode.Header as string);
            menuEnableOptimizer.Header  = Locale.Translate(menuEnableOptimizer.Header as string);


            menuTools.Header            = Locale.Translate(menuTools.Header as string);
            menuItemOptions.Header      = Locale.Translate(menuItemOptions.Header as string);
        }

        /// <summary>
        /// This function only exists because of WPF wizardry.
        /// With a debug build 'Application.Current.MainWindow' works, but not in release.
        /// </summary>
        public static Window GetMainWindow()
        {
            return instance;
        }


        private void menuNewProject_Click(object sender, RoutedEventArgs e)
        {
            NewProject();
        }

        private void CustomMapData_OnSaving(object sender, System.IO.FileSystemEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                var window = new SavingMapWindow();
                window.WindowStartupLocation = WindowStartupLocation.Manual;
                window.Top = this.Top + this.Height / 2 - window.Height / 2;
                window.Left = this.Left + this.Width / 2 - window.Width / 2;
                window.ShowDialog();
                ControllerMapData controllerMapData = new ControllerMapData();
                var modifiedTriggers = controllerMapData.ReloadMapData();
                if (modifiedTriggers.Count == 0)
                    return;

                ChangedTriggersWindow changedTriggersWindow = new ChangedTriggersWindow(modifiedTriggers);
                changedTriggersWindow.WindowStartupLocation = WindowStartupLocation.Manual;
                changedTriggersWindow.Top = this.Top + this.Height / 2 - changedTriggersWindow.Height / 2;
                changedTriggersWindow.Left = this.Left + this.Width / 2 - changedTriggersWindow.Width / 2;
                changedTriggersWindow.Show();
            });
        }

        private void TriggerExplorer_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            selectedExplorerItem = triggerExplorer.treeViewTriggerExplorer.SelectedItem as TreeItemExplorerElement;
            triggerExplorer.currentElement = selectedExplorerItem;
            ContainerProject.currentSelectedElement = selectedExplorerItem.Ielement.GetPath();

            ControllerTriggerExplorer controller = new ControllerTriggerExplorer();
            controller.OnSelectItem(selectedExplorerItem, tabControl);

            if (selectedExplorerItem.editor as TriggerControl != null)
            {
                btnCreateEvent.IsEnabled = true;
                btnCreateCondition.IsEnabled = true;
                btnCreateAction.IsEnabled = true;
            }
            else
            {
                btnCreateEvent.IsEnabled = false;
                btnCreateCondition.IsEnabled = false;
                btnCreateAction.IsEnabled = false;
            }

            e.Handled = true; // prevents event from firing up the parent items
        }

        private void btnNewMap_Click(object sender, RoutedEventArgs e)
        {
            NewProject();
        }

        private void btnOpenMap_Click(object sender, RoutedEventArgs e)
        {
            OpenMap();
        }

        private void btnSaveAll_Click(object sender, RoutedEventArgs e)
        {
            ControllerTriggerExplorer controller = new ControllerTriggerExplorer();
            controller.SaveAll();
        }

        private void btnUndo_Click(object sender, RoutedEventArgs e)
        {
            BetterTriggers.Commands.CommandManager.Undo();
        }

        private void btnRedo_Click(object sender, RoutedEventArgs e)
        {
            BetterTriggers.Commands.CommandManager.Redo();
        }

        private void btnCut_Click(object sender, RoutedEventArgs e)
        {
            MessageBox messageBox = new MessageBox("Error", "Not yet implemented");
            messageBox.ShowDialog();
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            MessageBox messageBox = new MessageBox("Error", "Not yet implemented");
            messageBox.ShowDialog();
        }

        private void btnPaste_Click(object sender, RoutedEventArgs e)
        {
            MessageBox messageBox = new MessageBox("Error", "Not yet implemented");
            messageBox.ShowDialog();
        }

        private void btnCreateFolder_Click(object sender, RoutedEventArgs e)
        {
            var controller = new ControllerFolder();
            controller.CreateFolder();
        }

        private void btnCreateTrigger_Click(object sender, RoutedEventArgs e)
        {
            var controller = new ControllerTrigger();
            controller.CreateTrigger();
        }

        private void btnCreateScript_Click(object sender, RoutedEventArgs e)
        {
            var controller = new ControllerScript();
            controller.CreateScript();
        }

        private void btnCreateVariable_Click(object sender, RoutedEventArgs e)
        {
            var controller = new ControllerVariable();
            controller.CreateVariable();
        }

        private void btnCreateEvent_Click(object sender, RoutedEventArgs e)
        {
            var triggerControl = selectedExplorerItem.editor as TriggerControl;
            triggerControl.CreateEvent();
        }

        private void btnCreateCondition_Click(object sender, RoutedEventArgs e)
        {
            var triggerControl = selectedExplorerItem.editor as TriggerControl;
            triggerControl.CreateCondition();
        }

        private void btnCreateAction_Click(object sender, RoutedEventArgs e)
        {
            var triggerControl = selectedExplorerItem.editor as TriggerControl;
            triggerControl.CreateAction();
        }

        private void btnSaveScript_Click(object sender, RoutedEventArgs e)
        {
            ControllerScriptGenerator scriptGenerator = new ControllerScriptGenerator();
            scriptGenerator.GenerateScript();
        }

        private void btnTestMap_Click(object sender, RoutedEventArgs e)
        {
            ControllerProject controller = new ControllerProject();
            try
            {
                controller.TestMap();
            }
            catch (Exception ex)
            {
                MessageBox dialogBox = new MessageBox("Error", ex.Message);
                dialogBox.ShowDialog();
            }
        }

        private void btnBuildMap_Click(object sender, RoutedEventArgs e)
        {
            ControllerProject controller = new ControllerProject();
            if(!controller.War3MapDirExists())
            {
                SelectWar3MapWindow window = new SelectWar3MapWindow();
                window.ShowDialog();
                if (!window.OK)
                {
                    return;
                }
            }
            try
            {
                controller.BuildMap();
            } catch (Exception ex)
            {
                MessageBox dialogBox = new MessageBox("Error", ex.Message);
                dialogBox.ShowDialog();
            }
        }


        private void MenuItem_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            bool canUndo = BetterTriggers.Commands.CommandManager.CanUndo();
            bool canRedo = BetterTriggers.Commands.CommandManager.CanRedo();
            string nameCommandToUndo = BetterTriggers.Commands.CommandManager.GetNameCommandToUndo();
            string nameCommandToRedo = BetterTriggers.Commands.CommandManager.GetNameCommandToRedo();

            menuItemUndo.IsEnabled = canUndo;
            menuItemRedo.IsEnabled = canRedo;
            if (canUndo)
                menuItemUndo.Header = $"Undo '{nameCommandToUndo}'";
            else
                menuItemUndo.Header = "Undo";

            if (canRedo)
                menuItemRedo.Header = $"Redo '{nameCommandToRedo}'";
            else
                menuItemRedo.Header = "Redo";
        }

        private void menuItemUndo_Click(object sender, RoutedEventArgs e)
        {
            BetterTriggers.Commands.CommandManager.Undo();
        }

        private void menuItemRedo_Click(object sender, RoutedEventArgs e)
        {
            BetterTriggers.Commands.CommandManager.Redo();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Z && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                BetterTriggers.Commands.CommandManager.Undo();
            }
            else if (e.Key == Key.Y && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                BetterTriggers.Commands.CommandManager.Redo();
            }
            else if (e.Key == Key.S && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                ControllerTriggerExplorer controller = new ControllerTriggerExplorer();
                controller.SaveAll();
            }
        }

        private void menuSave_Click(object sender, RoutedEventArgs e)
        {
            ControllerTriggerExplorer controller = new ControllerTriggerExplorer();
            controller.SaveAll();
        }

        private void menuOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenMap();
        }

        private void menuItemOptions_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settings = new SettingsWindow();
            settings.WindowStartupLocation = WindowStartupLocation.Manual;
            settings.Top = this.Top + this.Height / 2 - settings.Height / 2;
            settings.Left = this.Left + this.Width / 2 - settings.Width / 2;
            settings.ShowDialog();
        }

        private void NewProject()
        {
            NewProjectWindow window = new NewProjectWindow();
            window.WindowStartupLocation = WindowStartupLocation.Manual;
            window.Top = this.Top + this.Height / 2 - window.Height / 2;
            window.Left = this.Left + this.Width / 2 - window.Width / 2;
            window.ShowDialog();

            if (window.Ok)
            {
                string projectPath = window.projectFilePath;
                OpenProject(projectPath);
            }
        }

        private void OpenMap()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.DefaultExt = ".json";
            dialog.Filter = "JSON Files (*.json)|*.json";
            if (dialog.ShowDialog() == true)
            {
                OpenProject(dialog.FileName);
            }
        }

        private void menuRecentFiles_MouseEnter(object sender, MouseEventArgs e)
        {
            menuRecentFiles.Items.Clear();
            ControllerRecentFiles controllerRecentFiles = new ControllerRecentFiles();
            List<string> recentFiles = controllerRecentFiles.GetRecentFiles();

            for (int i = 0; i < recentFiles.Count; i++)
            {
                int index = i;
                MenuItem item = new MenuItem();
                item.Header = recentFiles[i];
                item.Click += delegate
                {
                    OpenProject(recentFiles[index]);
                };
                menuRecentFiles.Items.Add(item);
            }
        }

        private void menuNew_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            bool isProjectActive = triggerExplorer != null;
            menuNewCategory.IsEnabled = isProjectActive;
            menuNewTrigger.IsEnabled = isProjectActive;
            menuNewScript.IsEnabled = isProjectActive;
            menuNewVariable.IsEnabled = isProjectActive;

            bool selectedIsTrigger = false;
            if (isProjectActive && selectedExplorerItem != null)
                selectedIsTrigger = selectedExplorerItem.editor as TriggerControl != null;

            menuNewEvent.IsEnabled = selectedIsTrigger;
            menuNewCondition.IsEnabled = selectedIsTrigger;
            menuNewAction.IsEnabled = selectedIsTrigger;
        }

        private void OpenProject(string file)
        {
            ControllerProject controllerProject = new ControllerProject();
            var project = controllerProject.LoadProject(file);

            if (project == null)
            {
                DialogBox dialog = new DialogBox("Error", $"File '{file}' does not exist.");
                dialog.ShowDialog();
                return;
            }

            if (triggerExplorer != null)
            {
                var parent = (Grid)triggerExplorer.Parent;
                parent.Children.Remove(triggerExplorer);
            }
            triggerExplorer = new TriggerExplorer();
            TriggerExplorer.Current = triggerExplorer;
            triggerExplorer.Margin = new Thickness(-1, 1, 4, -1);
            triggerExplorer.HorizontalAlignment = HorizontalAlignment.Stretch;
            triggerExplorer.Width = Double.NaN;
            //currentTriggerExplorer.BorderThickness = new Thickness(0, 0, 0, 0);
            triggerExplorer.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 32, 32, 32));
            mainGrid.Children.Add(triggerExplorer);
            Grid.SetRow(triggerExplorer, 3);
            Grid.SetRowSpan(triggerExplorer, 4);
            Grid.SetColumn(triggerExplorer, 0);

            triggerExplorer.treeViewTriggerExplorer.SelectedItemChanged += TriggerExplorer_SelectedItemChanged; // subscribe to item selection changed event
            triggerExplorer.CreateRootItem();

            // temporary thing
            btnCreateFolder.IsEnabled = true;
            btnCreateTrigger.IsEnabled = true;
            btnCreateScript.IsEnabled = true;
            btnCreateVariable.IsEnabled = true;
            btnSaveScript.IsEnabled = true;
            btnTestMap.IsEnabled = true;
            btnBuildMap.IsEnabled = true;

            ControllerTriggerExplorer controllerTriggerExplorer = new ControllerTriggerExplorer();
            controllerTriggerExplorer.Populate(triggerExplorer);
        }

        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Menu_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ControllerProject controller = new ControllerProject();
            if (controller.GetUnsavedFileCount() == 0)
                return;

            OnCloseWindow onCloseWindow = new OnCloseWindow();
            onCloseWindow.ShowDialog();
            if (onCloseWindow.Yes)
                controller.SaveProject();
            else if (!onCloseWindow.Yes && !onCloseWindow.No)
                e.Cancel = true;

        }

    }
}
