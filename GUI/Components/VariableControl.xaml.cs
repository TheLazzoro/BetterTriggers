using BetterTriggers.Controllers;
using GUI.Commands;
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
        private ExplorerElementVariable explorerElementVariable;
        private ComboBoxItemType previousSelected;
        private string confirmationText = "This variable is still in use. Changing it will reset all references to it and cannot be undone. Continue with change?";
        private bool isLoading = true;
        private int defaultSelected = 0;
        private List<TreeItemExplorerElement> observers = new List<TreeItemExplorerElement>();



        public VariableControl(ExplorerElementVariable explorerElementVariable, string varName)
        {
            InitializeComponent();

            this.explorerElementVariable = explorerElementVariable;
            Variable variable = explorerElementVariable.variable;

            ControllerTriggerData controller = new ControllerTriggerData();
            List<VariableType> types = controller.LoadVariableTypes();

            for (int i = 0; i < types.Count; i++)
            {
                ComboBoxItemType item = new ComboBoxItemType();
                item.Content = types[i].displayname;
                item.Type = types[i].key;

                comboBoxVariableType.Items.Add(item);

                if (variable.Type == item.Type)
                {
                    defaultSelected = i;
                    previousSelected = item;
                }
            }


            Rename(varName);
            checkBoxIsArray.IsChecked = variable.IsArray;
            textBoxArraySize0.IsEnabled = variable.IsArray;
            comboBoxArrayDimensions.IsEnabled = variable.IsArray;
            textBoxArraySize0.Text = variable.ArraySize[0].ToString();
            textBoxArraySize1.Text = variable.ArraySize[1].ToString();
            if (!variable.IsTwoDimensions)
                comboBoxArrayDimensions.SelectedIndex = 0;
            else
            {
                comboBoxArrayDimensions.SelectedIndex = 1;
                textBoxArraySize1.IsEnabled = variable.IsArray;
            }


            // Events in the variableControl
            this.OnRename += delegate
            {
                //OnElementRename(textBoxVariableName.Text);
            };
        }

        private void comboBoxVariableType_Loaded(object sender, RoutedEventArgs e)
        {
            comboBoxVariableType.SelectedIndex = defaultSelected;
            isLoading = false;
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
            var selectedComboBoxItem = (ComboBoxItemType)comboBoxVariableType.SelectedItem;
            var isTwoDimensions = comboBoxArrayDimensions.SelectedIndex == 1;
            int[] arraySize = new int[] { int.Parse(textBoxArraySize0.Text), int.Parse(textBoxArraySize1.Text) };

            explorerElementVariable.variable.Type = selectedComboBoxItem.Type;
            explorerElementVariable.variable.IsArray = (bool)checkBoxIsArray.IsChecked;
            explorerElementVariable.variable.IsTwoDimensions = isTwoDimensions;
            explorerElementVariable.variable.ArraySize = arraySize;
            explorerElementVariable.variable.InitialValue = "???????????";

            return JsonConvert.SerializeObject(explorerElementVariable.variable);
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
            if (e.Key == Key.Enter)
            {
                //bubble the event up to the parent
                if (this.OnRename != null)
                    this.OnRename(this, e);
            }
        }

        private void comboBoxVariableType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isLoading)
                return;

            if (ResetVarRefs())
            {
                var selected = (ComboBoxItemType)comboBoxVariableType.SelectedItem;

                CommandVariableModifyType command = new CommandVariableModifyType(explorerElementVariable, selected.Type, previousSelected.Type);
                command.Execute();
                OnStateChange();

                previousSelected = (ComboBoxItemType)comboBoxVariableType.SelectedItem;
                defaultSelected = comboBoxVariableType.SelectedIndex;
            }
            else
            {
                comboBoxVariableType.SelectedItem = previousSelected;
                e.Handled = false;
            }
        }

        private void checkBoxIsArray_Click(object sender, RoutedEventArgs e)
        {
            if (isLoading)
                return;

            if (ResetVarRefs())
            {
                CommandVariableModifyArray command = new CommandVariableModifyArray(explorerElementVariable, (bool)checkBoxIsArray.IsChecked);
                command.Execute();
                OnStateChange();

                textBoxArraySize0.IsEnabled = (bool)checkBoxIsArray.IsChecked;
                comboBoxArrayDimensions.IsEnabled = (bool)checkBoxIsArray.IsChecked;
                if (comboBoxArrayDimensions.SelectedIndex == 1)
                    textBoxArraySize1.IsEnabled = (bool)checkBoxIsArray.IsChecked;
            }
            else
            {
                checkBoxIsArray.IsChecked = !checkBoxIsArray.IsChecked;
                e.Handled = false;
            }
        }


        private void comboBoxArrayDimensions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isLoading)
                return;

            bool isTwoDimensions = comboBoxArrayDimensions.SelectedIndex == 1;
            if (ResetVarRefs())
            {
                CommandVariableModifyDimension command = new CommandVariableModifyDimension(explorerElementVariable, isTwoDimensions);
                command.Execute();
                OnStateChange();
            }
            else
            {
                if (!explorerElementVariable.variable.IsTwoDimensions)
                    comboBoxArrayDimensions.SelectedIndex = 0;
                else
                    comboBoxArrayDimensions.SelectedIndex = 1;

                e.Handled = false;
            }

            if (!isTwoDimensions)
                textBoxArraySize1.IsEnabled = false;
            else
                textBoxArraySize1.IsEnabled = true;
        }

        public void OnRemoteChange()
        {
            throw new NotImplementedException();
        }

        private bool ResetVarRefs()
        {
            bool ok = true;
            if (explorerElementVariable.variable.TriggersUsing.Count > 0)
            {
                DialogBox dialog = new DialogBox("Confirmation", confirmationText);
                dialog.ShowDialog();
                ok = dialog.OK;

                if (ok)
                {
                    ControllerVariable controller = new ControllerVariable();
                    controller.RemoveVariableRefFromTriggers(this.explorerElementVariable);
                }
            }

            return ok;
        }

        public void Refresh()
        {
            isLoading = true;
            Variable variable = explorerElementVariable.variable;

            checkBoxIsArray.IsChecked = variable.IsArray;
            textBoxArraySize0.IsEnabled = variable.IsArray;
            comboBoxArrayDimensions.IsEnabled = variable.IsArray;
            textBoxArraySize0.Text = variable.ArraySize[0].ToString();
            textBoxArraySize1.Text = variable.ArraySize[1].ToString();
            if (!variable.IsTwoDimensions)
                comboBoxArrayDimensions.SelectedIndex = 0;
            else
            {
                comboBoxArrayDimensions.SelectedIndex = 1;
                textBoxArraySize1.IsEnabled = variable.IsArray;
            }

            for (int i = 0; i < comboBoxVariableType.Items.Count; i++)
            {
                var item = (ComboBoxItemType)comboBoxVariableType.Items[i];

                if (variable.Type == item.Type)
                    comboBoxVariableType.SelectedIndex = i;
            }

            isLoading = false;
        }

        public void SetElementEnabled(bool isEnabled)
        {
            throw new NotImplementedException();
        }

        public void SetElementInitiallyOn(bool isInitiallyOn)
        {
            throw new NotImplementedException();
        }

        public void Attach(TreeItemExplorerElement explorerElement)
        {
            this.observers.Add(explorerElement);
        }

        public void Detach(TreeItemExplorerElement explorerElement)
        {
            this.observers.Add(explorerElement);
        }

        public void OnStateChange()
        {
            foreach (var observer in observers)
            {
                observer.OnStateChange();
            }
        }
    }
}
