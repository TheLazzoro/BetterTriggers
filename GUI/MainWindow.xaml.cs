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
using GUI.Containers;
using GUI.Controllers;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TriggerControl currentTriggerWindow;
        
        public MainWindow()
        {
            InitializeComponent();

            treeViewTriggerExplorer.treeViewTriggerExplorer.SelectedItemChanged += TriggerExplorer_ItemSelectionChanged;
        }

        // Bubbled up event
        private void TriggerExplorer_ItemSelectionChanged(object sender, EventArgs e)
        {
            var item = (TreeViewItem) treeViewTriggerExplorer.treeViewTriggerExplorer.SelectedItem;
            var trigger = item.Tag as GUI.Components.TriggerExplorer.Trigger;

            if(trigger != null)
            {
                currentTriggerWindow = trigger.triggerControl;
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
            var controller = new ControllerTriggerExplorer();
            controller.CreateFolder(treeViewTriggerExplorer);
        }

        private void btnCreateTrigger_Click(object sender, RoutedEventArgs e)
        {
            var controller = new ControllerTriggerExplorer();
            controller.CreateTrigger(mainGrid, treeViewTriggerExplorer);
        }

        private void btnCreateScript_Click(object sender, RoutedEventArgs e)
        {
            var controller = new ControllerTriggerExplorer();
            controller.CreateScript(mainGrid, treeViewTriggerExplorer);
        }

        private void btnCreateVariable_Click(object sender, RoutedEventArgs e)
        {
            var controller = new ControllerTriggerExplorer();
            controller.CreateVariable(mainGrid, treeViewTriggerExplorer);
        }

        private void btnCreateEvent_Click(object sender, RoutedEventArgs e)
        {
            currentTriggerWindow.CreateEvent();
        }

        private void btnCreateCondition_Click(object sender, RoutedEventArgs e)
        {
            currentTriggerWindow.CreateCondition();
        }

        private void btnCreateAction_Click(object sender, RoutedEventArgs e)
        {
            currentTriggerWindow.CreateAction();
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
    }
}
