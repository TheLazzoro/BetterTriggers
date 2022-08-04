using BetterTriggers;
using BetterTriggers.Commands;
using BetterTriggers.Controllers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.WorldEdit;
using GUI.Components;
using GUI.Components.TriggerExplorer;
using GUI.Components.VariableEditor;
using GUI.Controllers;
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
    public partial class VariableControl : UserControl, IEditor
    {
        private ExplorerElementVariable explorerElementVariable;
        private ComboBoxItemType previousSelected;
        private bool isLoading = true;
        private int defaultSelected = 0;
        private List<TreeItemExplorerElement> observers = new List<TreeItemExplorerElement>();

        private string previousText0 = "1";
        private string previousText1 = "1";

        private bool preventStateChange = true;

        public VariableControl(ExplorerElementVariable explorerElementVariable)
        {
            this.explorerElementVariable = explorerElementVariable;
            Variable variable = explorerElementVariable.variable;

            previousText0 = variable.ArraySize[0].ToString();
            previousText1 = variable.ArraySize[1].ToString();

            InitializeComponent();

            ControllerTriggerData controller = new ControllerTriggerData();
            List<Types> types = controller.LoadAllVariableTypes();

            for (int i = 0; i < types.Count; i++)
            {
                ComboBoxItemType item = new ComboBoxItemType();
                item.Content = Locale.Translate(types[i].DisplayName);
                item.Type = types[i].Key;

                comboBoxVariableType.Items.Add(item);

                if (variable.Type == item.Type)
                {
                    defaultSelected = i;
                    previousSelected = item;
                }
            }

            ControllerVariable controllerVariable = new ControllerVariable();
            Rename(controllerVariable.GetVariableNameById(variable.Id));
            OnRemoteChange();
            checkBoxIsArray.IsChecked = variable.IsArray;
            textBoxArraySize0.IsEnabled = variable.IsArray;
            comboBoxArrayDimensions.IsEnabled = variable.IsArray;
            textBoxArraySize0.Text = previousText0;
            textBoxArraySize1.Text = previousText1;
            if (!variable.IsTwoDimensions)
                comboBoxArrayDimensions.SelectedIndex = 0;
            else
            {
                comboBoxArrayDimensions.SelectedIndex = 1;
                textBoxArraySize1.IsEnabled = variable.IsArray;
            }

            preventStateChange = false;
        }

        private void comboBoxVariableType_Loaded(object sender, RoutedEventArgs e)
        {
            comboBoxVariableType.SelectedIndex = defaultSelected;
            isLoading = false;
        }

        public void Rename(string name)
        {
            var newIdentifier = "udg_" + name;
            this.textBlockVariableNameUDG.Text = newIdentifier;
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

                explorerElementVariable.variable.InitialValue = new Parameter(); // Reset initial value
                 
                ControllerParamText controllerParamText = new ControllerParamText();
                this.textblockInitialValue.Inlines.Clear();
                var inlines = controllerParamText.GenerateParamText(explorerElementVariable);
                this.textblockInitialValue.Inlines.AddRange(inlines);
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

        private bool ResetVarRefs()
        {
            bool ok = true;
            ControllerReferences controllerRef = new ControllerReferences();
            List<ExplorerElementTrigger> refs = controllerRef.GetReferrers(this.explorerElementVariable);

            if (refs.Count > 0)
            {
                DialogBoxReferences dialog = new DialogBoxReferences(refs, ExplorerAction.Reset);
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

        public void OnRemoteChange()
        {
            ControllerParamText controllerParamText = new ControllerParamText();
            this.textblockInitialValue.Inlines.Clear();
            var inlines = controllerParamText.GenerateParamText(explorerElementVariable);
            this.textblockInitialValue.Inlines.AddRange(inlines);
            OnStateChange();
        }

        private void textBoxArraySize0_TextChanged(object sender, TextChangedEventArgs e)
        {
            ArraySizeTextChanged(e);
        }

        private void textBoxArraySize1_TextChanged(object sender, TextChangedEventArgs e)
        {
            ArraySizeTextChanged(e);
        }

        private void ArraySizeTextChanged(TextChangedEventArgs e)
        {
            // prevent exception and state change on init
            if (textBoxArraySize0 == null || textBoxArraySize1 == null || preventStateChange)
                return;

            try
            {
                int size0 = int.Parse(textBoxArraySize0.Text);
                int size1 = int.Parse(textBoxArraySize1.Text);
                if (size0 < 1 || size1 < 1)
                    throw new Exception("Array dimension cannot go below 1.");

                textBoxArraySize0.Text = textBoxArraySize0.Text;
                textBoxArraySize1.Text = textBoxArraySize1.Text;
                previousText0 = textBoxArraySize0.Text;
                previousText1 = textBoxArraySize1.Text;

                explorerElementVariable.variable.ArraySize[0] = size0;
                explorerElementVariable.variable.ArraySize[1] = size1;

                OnStateChange();
            }
            catch (Exception ex)
            {
                textBoxArraySize0.Text = previousText0;
                textBoxArraySize1.Text = previousText1;
            }

            //e.Handled = true;
        }

    }
}
