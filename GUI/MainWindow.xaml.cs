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
        private ExplorerTrigger currentTriggerExplorerElement;
        
        public MainWindow()
        {
            InitializeComponent();


            /*
            // TEST LOAD
            string[] files = Directory.GetFiles(@"C:\Users\Lasse Dam\Desktop\BetterTriggersTestMap");
            for (int i = 0; i < files.Length; i++)
            {
                var file = File.ReadAllText(files[i]);
                Model.Trigger trigger = JsonConvert.DeserializeObject<Model.Trigger>(file);

                var controller = new ControllerTrigger();
                controller.CreateTriggerWithElements(triggerExplorer, mainGrid, "test", trigger);
            }
            */
        }

        private void TriggerExplorer_ItemSelectionChanged(object sender, EventArgs e)
        {
            var item = (TreeViewItem)currentTriggerExplorer.treeViewTriggerExplorer.SelectedItem;
            var triggerExplorerElement = item.Tag as GUI.Components.TriggerExplorer.ExplorerTrigger;

            if(triggerExplorerElement != null)
            {
                currentTriggerExplorerElement = triggerExplorerElement;
                btnCreateEvent.IsEnabled = true;
                btnCreateCondition.IsEnabled = true;
                btnCreateAction.IsEnabled = true;
            } else
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
            controller.CreateTrigger(currentTriggerExplorer, mainGrid);
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
            currentTriggerExplorerElement.triggerControl.CreateEvent();
        }

        private void btnCreateCondition_Click(object sender, RoutedEventArgs e)
        {
            currentTriggerExplorerElement.triggerControl.CreateCondition();
        }

        private void btnCreateAction_Click(object sender, RoutedEventArgs e)
        {
            currentTriggerExplorerElement.triggerControl.CreateAction();
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

        private void menuNewProject_Click(object sender, RoutedEventArgs e)
        {
            NewProjectWindow window = new NewProjectWindow();
            window.ShowDialog();
        }

        private void menuSave_Click(object sender, RoutedEventArgs e)
        {
            if (currentTriggerExplorerElement == null)
                return;

            SaveLoad.SaveLoad.SaveStringAs(currentTriggerExplorerElement.GetSaveString());
        }

        private void menuOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.DefaultExt = ".json";
            dialog.Filter = "JSON Files (*.json)|*.json";
            if (dialog.ShowDialog() == true)
            {
                if(currentTriggerExplorer != null)
                {
                    var parent = (Grid) currentTriggerExplorer.Parent;
                    parent.Children.Remove(currentTriggerExplorer);
                }
                currentTriggerExplorer = new TriggerExplorer();
                currentTriggerExplorer.Margin = new Thickness(0, 0, 4, 0);
                currentTriggerExplorer.HorizontalAlignment = HorizontalAlignment.Stretch;
                currentTriggerExplorer.Width = Double.NaN;
                currentTriggerExplorer.BorderThickness = new Thickness(0, 0, 0, 0);
                mainGrid.Children.Add(currentTriggerExplorer);
                Grid.SetRow(currentTriggerExplorer, 3);
                Grid.SetRowSpan(currentTriggerExplorer, 4);
                Grid.SetColumn(currentTriggerExplorer, 0);
                currentTriggerExplorer.treeViewTriggerExplorer.SelectedItemChanged += TriggerExplorer_ItemSelectionChanged;

                var file = dialog.FileName;
                ControllerProject controller = new ControllerProject();
                controller.LoadProject(currentTriggerExplorer, mainGrid, file);
            }
        }
    }
}
