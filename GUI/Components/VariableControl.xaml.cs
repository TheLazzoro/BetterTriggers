using Facades.Controllers;
using GUI.Components;
using GUI.Components.TriggerExplorer;
using GUI.Components.VariableEditor;
using GUI.Controllers;
using Model.Data;
using Model.EditorData;
using Model.SaveableData;
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

namespace GUI.Components
{
    /// <summary>
    /// Interaction logic for VariableControl.xaml
    /// </summary>
    public partial class VariableControl : UserControl, IEditor
    {
        private Variable variable;
        private ComboBoxItemType previousSelected;
        private string confirmationText = "This variable is in use. Changing it will reset all references to it. Are you sure?";


        public VariableControl(Variable variable, string varName)
        {
            InitializeComponent();

            this.variable = variable;

            ControllerTriggerData controller = new ControllerTriggerData();
            List<VariableType> types = controller.LoadVariableTypes();

            for (int i = 0; i < types.Count; i++)
            {
                ComboBoxItemType item = new ComboBoxItemType();
                item.Content = types[i].displayname;
                item.Type = types[i].key;

                comboBoxVariableType.Items.Add(item);

                if (variable.Type == item.Type)
                    comboBoxVariableType.SelectedItem = item;
            }

            Rename(varName);
            checkBoxIsArray.IsChecked = variable.IsArray;
            checkBoxIsArray.IsEnabled = variable.IsArray;
            textBoxArraySize0.Text = variable.ArraySize[0].ToString();
            textBoxArraySize1.Text = variable.ArraySize[1].ToString();
            if (!variable.IsTwoDimensions)
                comboBoxArrayDimensions.SelectedIndex = 0;
            else 
                comboBoxArrayDimensions.SelectedIndex = 1;

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

        public void Show()
        {
            this.Visibility = Visibility.Visible;
        }

        public void Rename(string name)
        {
            var newIdentifier = "udg_" + name;
            this.textBlockVariableNameUDG.Text = newIdentifier;
        }

        public string GetSaveString()
        {
            var selectedComboBoxItem = (ComboBoxItemType) comboBoxVariableType.SelectedItem;
            var isTwoDimensions = comboBoxArrayDimensions.SelectedIndex == 1;
            int[] arraySize = new int[] { int.Parse(textBoxArraySize0.Text), int.Parse(textBoxArraySize1.Text) };

            variable.Type = selectedComboBoxItem.Type;
            variable.IsArray = (bool)checkBoxIsArray.IsChecked;
            variable.IsTwoDimensions = isTwoDimensions;
            variable.ArraySize = arraySize;
            variable.InitialValue = "???????????";

            return JsonConvert.SerializeObject(variable);
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
            if(variable.FilesUsing.Count > 0)
            {
                DialogBox dialog = new DialogBox("Confirmation", confirmationText);
                dialog.ShowDialog();
                ok = dialog.OK;
            }

            if (ok)
            {
                previousSelected = (ComboBoxItemType)comboBoxVariableType.SelectedItem;
                variable.FilesUsing.Clear();
            }
            else
            {
                comboBoxVariableType.SelectedItem = previousSelected;
                e.Handled = false;
            }
        }

        public void OnRemoteChange()
        {
            throw new NotImplementedException();
        }

        private void comboBoxArrayDimensions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool ok = true;
            if (variable.FilesUsing.Count > 0)
            {
                DialogBox dialog = new DialogBox("Confirmation", confirmationText);
                dialog.ShowDialog();
                ok = dialog.OK;
            }

            if (ok)
            {
                int array0 = int.Parse(textBoxArraySize0.Text);
                int array1 = int.Parse(textBoxArraySize1.Text);

                if (comboBoxArrayDimensions.SelectedIndex == 0)
                {
                    variable.IsTwoDimensions = false;
                    textBoxArraySize1.IsEnabled = false;
                }
                else
                {
                    variable.IsTwoDimensions = true;
                    textBoxArraySize1.IsEnabled = true;
                }
            }
        }

    }
}
