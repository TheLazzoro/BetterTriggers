using BetterTriggers;
using BetterTriggers.Containers;
using BetterTriggers.Controllers;
using BetterTriggers.Models.SaveableData;
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
        static Window instance;
        TriggerExplorer triggerExplorer;
        TreeItemExplorerElement selectedExplorerItem;
        TabViewModel vmd;

        public MainWindow()
        {
            InitializeComponent();
            instance = this;

            Settings settings = Settings.Load();
            this.Width = settings.windowWidth;
            this.Height = settings.windowHeight;
            this.Left = settings.windowX;
            this.Top = settings.windowY;
            this.WindowState = settings.windowFullscreen ? WindowState.Maximized : WindowState.Normal;
            rowTriggerExplorer.Width = new GridLength(settings.triggerExplorerWidth);

            vmd = new TabViewModel();
            tabControl.ItemsSource = vmd.Tabs;

            BetterTriggers.Init.Initialize();
            CustomMapData.OnSaving += CustomMapData_OnSaving;

            btnNewMap.ToolTip = Locale.Translate(btnNewMap.ToolTip as string);
            //btnOpenMap.ToolTip          = Locale.Translate(btnOpenMap.ToolTip as string);

            btnUndo.ToolTip = Locale.Translate(btnUndo.ToolTip as string);
            btnRedo.ToolTip = Locale.Translate(btnRedo.ToolTip as string);
            // TODO:
            //btnCut.ToolTip              = Locale.Translate(btnCut.ToolTip as string);
            //btnCopy.ToolTip             = Locale.Translate(btnCopy.ToolTip as string);
            //btnPaste.ToolTip            = Locale.Translate(btnPaste.ToolTip as string);

            btnCreateFolder.ToolTip = Locale.Translate(btnCreateFolder.ToolTip as string);
            btnCreateTrigger.ToolTip = Locale.Translate(btnCreateTrigger.ToolTip as string);
            btnCreateScript.ToolTip = Locale.Translate(btnCreateScript.ToolTip as string);
            btnCreateVariable.ToolTip = Locale.Translate(btnCreateVariable.ToolTip as string);

            btnCreateEvent.ToolTip = Locale.Translate(btnCreateEvent.ToolTip as string);
            btnCreateCondition.ToolTip = Locale.Translate(btnCreateCondition.ToolTip as string);
            btnCreateAction.ToolTip = Locale.Translate(btnCreateAction.ToolTip as string);
            btnSaveScript.ToolTip = Locale.Translate(btnSaveScript.ToolTip as string);

            btnTestMap.ToolTip = Locale.Translate(btnTestMap.ToolTip as string);
            btnBuildMap.ToolTip = Locale.Translate(btnBuildMap.ToolTip as string);


            menuFile.Header = Locale.Translate(menuFile.Header as string);
            //menuOpen.Header             = Locale.Translate(menuOpen.Header as string);


            menuEdit.Header = Locale.Translate(menuEdit.Header as string);


            menuNew.Header = Locale.Translate(menuNew.Header as string);
            menuNewCategory.Header = Locale.Translate(menuNewCategory.Header as string);
            menuNewTrigger.Header = Locale.Translate(menuNewTrigger.Header as string);
            menuNewScript.Header = Locale.Translate(menuNewScript.Header as string);
            menuNewVariable.Header = Locale.Translate(menuNewVariable.Header as string);

            menuNewEvent.Header = Locale.Translate(menuNewEvent.Header as string);
            menuNewCondition.Header = Locale.Translate(menuNewCondition.Header as string);
            menuNewAction.Header = Locale.Translate(menuNewAction.Header as string);

            // TODO:
            //menuJassHelper.Header       = Locale.Translate(menuJassHelper.Header as string);
            //menuEnableJassHelper.Header = Locale.Translate(menuEnableJassHelper.Header as string);
            //menuEnableVJass.Header      = Locale.Translate(menuEnableVJass.Header as string);
            //menuEnableDebugMode.Header  = Locale.Translate(menuEnableDebugMode.Header as string);
            //menuEnableOptimizer.Header  = Locale.Translate(menuEnableOptimizer.Header as string);


            menuTools.Header = Locale.Translate(menuTools.Header as string);
            menuItemOptions.Header = Locale.Translate(menuItemOptions.Header as string);
        }

        /// <summary>
        /// This function only exists because of WPF wizardry.
        /// With a debug build 'Application.Current.MainWindow' works, but not in release.
        /// </summary>
        public static Window GetMainWindow()
        {
            return instance;
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

        private void TreeViewTriggerExplorer_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeItemExplorerElement selected = e.NewValue as TreeItemExplorerElement;
            if (selected == null)
                return;

            ContainerProject.currentSelectedElement = selected.Ielement.GetPath();
        }

        private void TreeViewTriggerExplorer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenExplorerElement();
            e.Handled = true; // prevents event from firing up the parent items
        }

        private void TreeViewTriggerExplorer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                OpenExplorerElement();
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabControl.Items.Count == 0)
                return;

            TabItemBT tabItem = tabControl.SelectedItem as TabItemBT;
            if (tabItem == null) // it crashes when we don't do this?
                return;

            ControllerTriggerExplorer controller = new ControllerTriggerExplorer();
            controller.OnSelectTab(tabItem.explorerElement, vmd, tabControl);
            selectedExplorerItem = tabItem.explorerElement; // TODO: lazy
            EnableTriggerElementButtons();
        }

        private void OpenExplorerElement()
        {
            selectedExplorerItem = triggerExplorer.treeViewTriggerExplorer.SelectedItem as TreeItemExplorerElement;
            if (selectedExplorerItem == null)
                return;

            triggerExplorer.currentElement = selectedExplorerItem;

            ControllerTriggerExplorer controller = new ControllerTriggerExplorer();
            controller.OnSelectTab(selectedExplorerItem, vmd, tabControl);
            EnableTriggerElementButtons();
        }


        private void EnableTriggerElementButtons()
        {
            if (selectedExplorerItem.editor as TriggerControl != null)
                EnableECAButtons(true);
            else
                EnableECAButtons(false);
        }

        private void ButtonTabItem_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)e.Source;
            TabItemBT tabItem = (TabItemBT)btn.DataContext;
            tabItem.Close();

            if (tabControl.Items.Count == 0)
                EnableECAButtons(false);
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
            ControllerProject controller = new ControllerProject();
            controller.GenerateScript();
        }

        private void btnTestMap_Click(object sender, RoutedEventArgs e)
        {
            ControllerProject controller = new ControllerProject();
            if (!controller.War3MapDirExists())
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
            if (!controller.War3MapDirExists())
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
            }
            catch (Exception ex)
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

        private bool IsProjectActive()
        {
            return triggerExplorer != null;
        }

        private void OpenProject(string file)
        {
            ControllerProject controllerProject = new ControllerProject();
            War3Project project = null;
            try
            {
                project = controllerProject.LoadProject(file);
            }
            catch (Exception ex)
            {
                MessageBox dialog = new MessageBox("Error", ex.Message);
                dialog.ShowDialog();
                return;
            }

            if (!controllerProject.War3MapDirExists())
            {
                SelectWar3MapWindow window = new SelectWar3MapWindow();
                window.ShowDialog();
                if (!window.OK)
                {
                    return;
                }
            }

            vmd.Tabs.Clear();
            LoadingDataWindow loadingDataWindow = new LoadingDataWindow(project.War3MapDirectory);
            loadingDataWindow.ShowDialog();

            if (triggerExplorer != null)
            {
                var parent = (Grid)triggerExplorer.Parent;
                parent.Children.Remove(triggerExplorer);
                triggerExplorer.Dispose();
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

            triggerExplorer.treeViewTriggerExplorer.MouseDoubleClick += TreeViewTriggerExplorer_MouseDoubleClick; // subscribe to item selection changed event
            triggerExplorer.treeViewTriggerExplorer.SelectedItemChanged += TreeViewTriggerExplorer_SelectedItemChanged;
            triggerExplorer.treeViewTriggerExplorer.KeyDown += TreeViewTriggerExplorer_KeyDown;
            triggerExplorer.CreateRootItem();

            EnableToolbar(true);

            ControllerTriggerExplorer controllerTriggerExplorer = new ControllerTriggerExplorer();
            controllerTriggerExplorer.Populate(triggerExplorer);
        }

        private void EnableToolbar(bool enable)
        {
            btnCreateFolder.IsEnabled = enable;
            btnCreateTrigger.IsEnabled = enable;
            btnCreateScript.IsEnabled = enable;
            btnCreateVariable.IsEnabled = enable;
            btnSaveScript.IsEnabled = enable;
            btnTestMap.IsEnabled = enable;
            btnBuildMap.IsEnabled = enable;
        }

        private void EnableECAButtons(bool enable)
        {
            btnCreateEvent.IsEnabled = enable;
            btnCreateCondition.IsEnabled = enable;
            btnCreateAction.IsEnabled = enable;
        }


        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Menu_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void menuItemTriggerConverter_Click(object sender, RoutedEventArgs e)
        {
            ConvertTriggersWindow window = new ConvertTriggersWindow();
            window.ShowDialog();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            bool doClose = DoCloseProject();
            Settings.Save(Settings.Load());
            e.Cancel = !doClose;
        }

        private bool DoCloseProject()
        {
            ControllerProject controller = new ControllerProject();
            if (controller.GetUnsavedFileCount() == 0)
                return true;

            OnCloseWindow onCloseWindow = new OnCloseWindow();
            onCloseWindow.ShowDialog();
            if (onCloseWindow.Yes)
            {
                controller.SetEnableFileEvents(false);
                controller.SaveProject();
                return true;
            }
            else if (!onCloseWindow.Yes && !onCloseWindow.No)
                return false;

            return true;
        }


        private void CommandBinding_CanExecute_NewProject(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBinding_Executed_NewProject(object sender, ExecutedRoutedEventArgs e)
        {
            NewProject();
        }

        private void CommandBinding_CanExecute_OpenProject(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBinding_Executed_OpenProject(object sender, ExecutedRoutedEventArgs e)
        {
            OpenMap();
        }

        private void CommandBinding_CanExecute_Save(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsProjectActive();
        }

        private void CommandBinding_Executed_Save(object sender, ExecutedRoutedEventArgs e)
        {
            ControllerProject controller = new ControllerProject();
            controller.SaveProject();
        }


        private void CommandBinding_CanExecute_CloseProject(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsProjectActive();
        }

        private void CommandBinding_Executed_CloseProject(object sender, ExecutedRoutedEventArgs e)
        {
            if (!DoCloseProject())
                return;

            vmd.Tabs.Clear();
            mainGrid.Children.Remove(triggerExplorer);
            triggerExplorer.Dispose();
            triggerExplorer = null;
            EnableToolbar(false);
            EnableECAButtons(false);

            ControllerProject controller = new ControllerProject();
            controller.CloseProject();
        }

        private void CommandBinding_CanExecute_Undo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = BetterTriggers.Commands.CommandManager.CanUndo();

        }

        private void CommandBinding_Executed_Undo(object sender, ExecutedRoutedEventArgs e)
        {
            BetterTriggers.Commands.CommandManager.Undo();
        }

        private void CommandBinding_CanExecute_Redo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = BetterTriggers.Commands.CommandManager.CanRedo();

        }

        private void CommandBinding_Executed_Redo(object sender, ExecutedRoutedEventArgs e)
        {
            BetterTriggers.Commands.CommandManager.Redo();
        }

        private void CommandBinding_CanExecute_NewCategory(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsProjectActive();
        }

        private void CommandBinding_Executed_NewCategory(object sender, ExecutedRoutedEventArgs e)
        {
            var controller = new ControllerFolder();
            controller.CreateFolder();
        }

        private void CommandBinding_CanExecute_NewTrigger(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsProjectActive();

        }

        private void CommandBinding_Executed_NewTrigger(object sender, ExecutedRoutedEventArgs e)
        {
            var controller = new ControllerTrigger();
            controller.CreateTrigger();
        }

        private void CommandBinding_CanExecute_NewScript(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsProjectActive();
        }

        private void CommandBinding_Executed_NewScript(object sender, ExecutedRoutedEventArgs e)
        {
            var controller = new ControllerScript();
            controller.CreateScript();
        }

        private void CommandBinding_CanExecute_NewGlobalVariable(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsProjectActive();

        }

        private void CommandBinding_Executed_NewGlobalVariable(object sender, ExecutedRoutedEventArgs e)
        {
            var controller = new ControllerVariable();
            controller.CreateVariable();
        }

        private void CommandBinding_CanExecute_NewEvent(object sender, CanExecuteRoutedEventArgs e)
        {
            if (selectedExplorerItem != null)
            {
                var triggerControl = selectedExplorerItem.editor as TriggerControl;
                e.CanExecute = triggerControl != null;
            }
        }

        private void CommandBinding_Executed_NewEvent(object sender, ExecutedRoutedEventArgs e)
        {
            var triggerControl = selectedExplorerItem.editor as TriggerControl;
            triggerControl.CreateEvent();
        }

        private void CommandBinding_CanExecute_NewCondition(object sender, CanExecuteRoutedEventArgs e)
        {
            if (selectedExplorerItem != null)
            {
                var triggerControl = selectedExplorerItem.editor as TriggerControl;
                e.CanExecute = triggerControl != null;
            }
        }

        private void CommandBinding_Executed_NewCondition(object sender, ExecutedRoutedEventArgs e)
        {
            var triggerControl = selectedExplorerItem.editor as TriggerControl;
            triggerControl.CreateCondition();
        }

        private void CommandBinding_CanExecute_NewAction(object sender, CanExecuteRoutedEventArgs e)
        {
            if (selectedExplorerItem != null)
            {
                var triggerControl = selectedExplorerItem.editor as TriggerControl;
                e.CanExecute = triggerControl != null;
            }
        }

        private void CommandBinding_Executed_NewAction(object sender, ExecutedRoutedEventArgs e)
        {
            var triggerControl = selectedExplorerItem.editor as TriggerControl;
            triggerControl.CreateAction();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Settings settings = Settings.Load();
            settings.windowWidth = (int)this.Width;
            settings.windowHeight = (int)this.Height;
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            Settings settings = Settings.Load();
            settings.windowX = (int)this.Left;
            settings.windowY = (int)this.Top;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            Settings settings = Settings.Load();
            settings.windowFullscreen = this.WindowState.HasFlag(WindowState.Maximized);
        }

        private void GridSplitter_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            Settings settings = Settings.Load();
            settings.triggerExplorerWidth = (int)rowTriggerExplorer.Width.Value;
        }
    }
}
