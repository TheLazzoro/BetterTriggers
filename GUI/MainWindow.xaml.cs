using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GUI.Components.TriggerExplorer;
using Microsoft.Win32;
using BetterTriggers.Controllers;
using BetterTriggers.Containers;
using GUI.Controllers;
using GUI.Components;
using System.Windows.Input;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TriggerExplorer triggerExplorer;
        TreeItemExplorerElement selectedExplorerItem;


        public MainWindow()
        {
            InitializeComponent();
            
            BetterTriggers.Init.Initialize();
        }

        private void menuNewProject_Click(object sender, RoutedEventArgs e)
        {
            NewProjectWindow window = new NewProjectWindow();
            window.WindowStartupLocation = WindowStartupLocation.Manual;
            window.Top = this.Top + this.Height / 2 - window.Height / 2;
            window.Left = this.Left + this.Width / 2 - window.Width / 2;
            window.ShowDialog();

            if (window.Ok)
            {
                var root = ContainerProject.project.Root;
                TriggerExplorer te = new TriggerExplorer();
                mainGrid.Children.Add(te);
                Grid.SetColumn(te, 0);
                Grid.SetRow(te, 2);

                this.triggerExplorer = te;
            }
        }

        private void TriggerExplorer_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            selectedExplorerItem = triggerExplorer.treeViewTriggerExplorer.SelectedItem as TreeItemExplorerElement;
            triggerExplorer.currentElement = selectedExplorerItem;
            ContainerProject.currentSelectedElement = selectedExplorerItem.Ielement.GetPath();

            ControllerTriggerExplorer controller = new ControllerTriggerExplorer();
            controller.OnSelectItem(selectedExplorerItem, dragableTabControl);

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
            string script = scriptGenerator.GenerateScript("C:/Users/Lasse Dam/Desktop/jass.j");
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

            triggerExplorer.CreateRootItem();

            //triggerExplorer.treeViewTriggerExplorer.SelectedItemChanged += TriggerExplorer_ItemSelectionChanged; 
            triggerExplorer.treeViewTriggerExplorer.SelectedItemChanged += TriggerExplorer_SelectedItemChanged; // subscribe to item selection changed event

            ControllerTriggerExplorer controllerTriggerExplorer = new ControllerTriggerExplorer();
            controllerTriggerExplorer.Populate(triggerExplorer);
        }
    }
}
