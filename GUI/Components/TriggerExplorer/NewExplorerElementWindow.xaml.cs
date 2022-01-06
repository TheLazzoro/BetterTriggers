using GUI.Components.TriggerExplorer;
using GUI.Containers;
using GUI.Utility;
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
        EnumExplorerElement enumExplorerElement;
        public string ElementName;
        
        public NewExplorerElementWindow(EnumExplorerElement enumExplorerElement)
        {
            InitializeComponent();
            
            this.enumExplorerElement = enumExplorerElement;

            switch (enumExplorerElement)
            {
                case EnumExplorerElement.Directory:
                    textBoxName.Text = NameGenerator.GenerateCategoryName();
                    break;
                case EnumExplorerElement.Trigger:
                    textBoxName.Text = NameGenerator.GenerateTriggerName();
                    break;
                case EnumExplorerElement.Script:
                    textBoxName.Text = NameGenerator.GenerateScriptName();
                    break;
                case EnumExplorerElement.Variable:
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
                case EnumExplorerElement.Directory:
                    isValid = !ContainerFolders.Contains(textBoxName.Text) && textBoxName.Text != "";
                    break;
                case EnumExplorerElement.Trigger:
                    isValid = !ContainerTriggers.Contains(textBoxName.Text) && textBoxName.Text != "";
                    break;
                case EnumExplorerElement.Script:
                    isValid = !ContainerScripts.Contains(textBoxName.Text) && textBoxName.Text != "";
                    break;
                case EnumExplorerElement.Variable:
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
