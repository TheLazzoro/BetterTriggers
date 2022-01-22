using GUI.Components.TriggerExplorer;
using GUI.Containers;
using GUI.Utility;
using Model.Data;
using Model.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GUI
{
    /// <summary>
    /// Interaction logic for CreateExplorerElementWindow.xaml
    /// </summary>
    public partial class NewExplorerElementWindow : Window
    {
        ExplorerElementType enumExplorerElement;
        public string ElementName;
        
        public NewExplorerElementWindow(ExplorerElementType enumExplorerElement)
        {
            InitializeComponent();
            
            this.enumExplorerElement = enumExplorerElement;

            switch (enumExplorerElement)
            {
                case ExplorerElementType.Directory:
                    textBoxName.Text = NameGenerator.GenerateCategoryName();
                    break;
                case ExplorerElementType.Trigger:
                    textBoxName.Text = NameGenerator.GenerateTriggerName();
                    break;
                case ExplorerElementType.Script:
                    textBoxName.Text = NameGenerator.GenerateScriptName();
                    break;
                case ExplorerElementType.Variable:
                    textBoxName.Text = NameGenerator.GenerateVariableName();
                    break;
                default:
                    break;
            }

            textBoxName.Focusable = true;
            textBoxName.Focus();
            textBoxName.Select(textBoxName.Text.Length, 0);
        }

        private bool IsNameValid()
        {
            bool isValid = false;
            switch (this.enumExplorerElement)
            {
                case ExplorerElementType.Directory:
                    isValid = !ContainerFolders.Contains(textBoxName.Text) && textBoxName.Text != "";
                    break;
                case ExplorerElementType.Trigger:
                    isValid = !ContainerTriggers.Contains(textBoxName.Text) && textBoxName.Text != "";
                    break;
                case ExplorerElementType.Script:
                    isValid = !ContainerScripts.Contains(textBoxName.Text) && textBoxName.Text != "";
                    break;
                case ExplorerElementType.Variable:
                    isValid = !ContainerVariables.Contains(textBoxName.Text) && textBoxName.Text != "";
                    break;
                default:
                    break;
            }

            return isValid;
        }

        private void OnClickOK()
        {
            if (IsNameValid())
            {
                ElementName = textBoxName.Text;
                this.Close();
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            OnClickOK();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
                OnClickOK();
            if (e.Key == Key.Escape)
                Close();
        }
    }
}
