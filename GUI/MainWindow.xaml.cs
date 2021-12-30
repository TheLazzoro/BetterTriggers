using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using BetterTriggers;
using GUI.Components.TextEditor;
using GUI.Components.TriggerExplorer;
using GUI.Containers;
using GUI.Controllers;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TriggerExplorer currentTriggerExplorer;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void TriggerExplorer_ItemSelectionChanged(object sender, EventArgs e)
        {
            var explorerItem = currentTriggerExplorer.treeViewTriggerExplorer.SelectedItem as ExplorerElement;

            ControllerProject controller = new ControllerProject();
            controller.OnClick_ExplorerElement(explorerItem, tabControl);

            var triggerExplorerElement = ControllerProject.currentTriggerExplorerElement;

            if (triggerExplorerElement != null)
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
        }

        private void btnCreateFolder_Click(object sender, RoutedEventArgs e)
        {
            var controller = new ControllerFolder();
            controller.CreateFolder(currentTriggerExplorer);
        }

        private void btnCreateTrigger_Click(object sender, RoutedEventArgs e)
        {
            var controller = new ControllerTrigger();
            controller.CreateTrigger(currentTriggerExplorer, tabControl);
        }

        private void btnCreateScript_Click(object sender, RoutedEventArgs e)
        {
            var controller = new ControllerScript();
            controller.CreateScript(mainGrid, currentTriggerExplorer);
        }

        private void btnCreateVariable_Click(object sender, RoutedEventArgs e)
        {
            var controller = new ControllerVariable();
            controller.CreateVariable(mainGrid, currentTriggerExplorer);
        }

        private void btnCreateEvent_Click(object sender, RoutedEventArgs e)
        {
            var triggerControl = ControllerProject.currentTriggerExplorerElement as TriggerControl;
            triggerControl.CreateEvent();
        }

        private void btnCreateCondition_Click(object sender, RoutedEventArgs e)
        {
            var triggerControl = ControllerProject.currentTriggerExplorerElement as TriggerControl;
            triggerControl.CreateCondition();
        }

        private void btnCreateAction_Click(object sender, RoutedEventArgs e)
        {
            var triggerControl = ControllerProject.currentTriggerExplorerElement as TriggerControl;
            triggerControl.CreateAction();
        }

        private void btnSaveScript_Click(object sender, RoutedEventArgs e)
        {
            string fileJassHelper = "C:/Users/Lasse Dam/Desktop/JassHelper Experiement/jasshelper.exe";
            string fileCommonJ = "\"C:/Users/Lasse Dam/Desktop/JassHelper Experiement/common.j\"";
            string fileBlizzardJ = "\"C:/Users/Lasse Dam/Desktop/JassHelper Experiement/Blizzard.j\"";
            string fileInput = "C:/Users/Lasse Dam/Desktop/JassHelper Experiement/vJass.j";
            string fileOutput = "\"C:/Users/Lasse Dam/Desktop/JassHelper Experiement/output.j\"";

            string script = ContainerITriggerElements.GenerateScript();

            JassHelper.SaveVJassScript(fileInput, script);
            JassHelper.RunJassHelper(fileJassHelper, fileCommonJ, fileBlizzardJ, "\"" + fileInput + "\"", fileOutput);
        }

        private void MenuItem_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            bool canUndo = Commands.CommandManager.CanUndo();
            bool canRedo = Commands.CommandManager.CanRedo();
            string nameCommandToUndo = Commands.CommandManager.GetNameCommandToUndo();
            string nameCommandToRedo = Commands.CommandManager.GetNameCommandToRedo();

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
            Commands.CommandManager.Undo();
        }

        private void menuItemRedo_Click(object sender, RoutedEventArgs e)
        {
            Commands.CommandManager.Redo();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Z && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                Commands.CommandManager.Undo();
            }
            else if (e.Key == Key.Y && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                Commands.CommandManager.Redo();
            }
        }

        private void CreateNewTriggerExplorer()
        {
            if (currentTriggerExplorer != null)
            {
                var parent = (Grid)currentTriggerExplorer.Parent;
                parent.Children.Remove(currentTriggerExplorer);
            }
            currentTriggerExplorer = new TriggerExplorer();
            currentTriggerExplorer.Margin = new Thickness(0, 0, 4, 0);
            currentTriggerExplorer.HorizontalAlignment = HorizontalAlignment.Stretch;
            currentTriggerExplorer.Width = Double.NaN;
            //currentTriggerExplorer.BorderThickness = new Thickness(0, 0, 0, 0);
            currentTriggerExplorer.BorderBrush = new SolidColorBrush(Color.FromArgb(0, 32, 32, 32));
            mainGrid.Children.Add(currentTriggerExplorer);
            Grid.SetRow(currentTriggerExplorer, 3);
            Grid.SetRowSpan(currentTriggerExplorer, 4);
            Grid.SetColumn(currentTriggerExplorer, 0);
            currentTriggerExplorer.treeViewTriggerExplorer.SelectedItemChanged += TriggerExplorer_ItemSelectionChanged;
        }

        private void menuNewProject_Click(object sender, RoutedEventArgs e)
        {
            NewProjectWindow window = new NewProjectWindow();
            window.WindowStartupLocation = WindowStartupLocation.Manual;
            window.Top = this.Top + this.Height / 2 - window.Height / 2;
            window.Left = this.Left + this.Width / 2 - window.Width / 2;
            window.ShowDialog();
            var createdProject = window.createdProject;

            if (createdProject != null)
            {
                CreateNewTriggerExplorer();

                ControllerProject controller = new ControllerProject();
                controller.LoadProject(currentTriggerExplorer, mainGrid, createdProject);
            }
        }

        private void menuSave_Click(object sender, RoutedEventArgs e)
        {
            var triggerControl = ControllerProject.currentTriggerExplorerElement as TriggerControl;
            if (triggerControl == null)
                return;

            SaveLoad.SaveLoad.SaveStringAs(triggerControl.GetSaveString());
        }

        private void menuOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.DefaultExt = ".json";
            dialog.Filter = "JSON Files (*.json)|*.json";
            if (dialog.ShowDialog() == true)
            {
                var file = dialog.FileName;

                CreateNewTriggerExplorer();

                ControllerProject controller = new ControllerProject();
                controller.LoadProject(currentTriggerExplorer, mainGrid, file);
            }
        }
    }
}
