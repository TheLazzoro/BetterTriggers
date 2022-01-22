using GUI.Components.TriggerExplorer;
using GUI.Components.VariableEditor;
using GUI.Controllers;
using Model.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GUI
{
    /// <summary>
    /// Interaction logic for VariableControl.xaml
    /// </summary>
    public partial class VariableControl : UserControl, IExplorerElement
    {
        public List<string> FilesUsing = new List<string>();
        private ComboBoxItemType previousSelected;
        
        public VariableControl()
        {
            InitializeComponent();

            // Events in the variableControl
            this.OnRename += delegate
            {
                //OnElementRename(textBoxVariableName.Text);
            };
        }

        public void Hide()
        {
            this.Visibility = Visibility.Hidden;
        }

        public void OnElementClick()
        {
            ExplorerElement.currentExplorerElement = this;
        }

        public void Show()
        {
            this.Visibility = Visibility.Visible;
        }

        public void OnElementRename(string name)
        {
            var newIdentifier = "udg_" + name;
            this.textBlockVariableNameUDG.Text = newIdentifier;
        }

        public string GetSaveString()
        {
            var selectedComboBoxItem = (ComboBoxItemType) comboBoxVariableType.SelectedItem;
            int[] arraySize = new int[] { int.Parse(textBoxArraySize0.Text), int.Parse(textBoxArraySize1.Text) };

            Variable variable = new Variable()
            {
                Type = selectedComboBoxItem.Type,
                ArraySize = arraySize,
                IsTwoDimensions = comboBoxArrayDimensions.SelectedIndex == 1,
                IsArray = (bool)checkBoxIsArray.IsChecked,
                FilesUsing = FilesUsing,
            };

            string json = JsonConvert.SerializeObject(variable);

            return json;
        }

        public UserControl GetControl()
        {
            return this;
        }

        [Browsable(true)]
        [Category("Action")]
        public event EventHandler OnRename;

        private void textBoxVariableName_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                //bubble the event up to the parent
                if (this.OnRename != null)
                    this.OnRename(this, e);
            }
        }

        private void checkBoxIsArray_Click(object sender, RoutedEventArgs e)
        {
            textBoxArraySize0.IsEnabled = (bool)checkBoxIsArray.IsChecked;
            lblSize0.IsEnabled = (bool)checkBoxIsArray.IsChecked;
        }

        private void comboBoxVariableType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool ok = true;
            if(FilesUsing.Count > 0)
            {
                DialogBox dialog = new DialogBox("Confirmation", "This variable is in use. Changing it will reset all references to it. Are you sure?");
                dialog.ShowDialog();
                ok = dialog.OK;
            }

            if (ok)
            {
                previousSelected = (ComboBoxItemType)comboBoxVariableType.SelectedItem;
                FilesUsing.Clear();
            }
            else
            {
                comboBoxVariableType.SelectedItem = previousSelected;
                e.Handled = false;
            }
        }
    }
}
