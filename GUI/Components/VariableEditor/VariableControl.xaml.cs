using BetterTriggers;
using BetterTriggers.Commands;
using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.WorldEdit;
using GUI.Components;
using GUI.Components.Dialogs;
using GUI.Components.Shared;
using GUI.Components.VariableEditor;
using GUI.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class VariableControl : UserControl
    {
        public event Action OnChange;

        private Variable variable;

        private string previousText0 = "1";
        private string previousText1 = "1";

        private bool preventStateChange = true;
        private bool suppressUIEvents = false;

        private VariableControlViewModel _viewModel;

        public VariableControl(Variable variable)
        {
            this.variable = variable;
            previousText0 = variable.ArraySize[0].ToString();
            previousText1 = variable.ArraySize[1].ToString();
            InitializeComponent();

            _viewModel = new VariableControlViewModel(variable);
            DataContext = _viewModel;

            var usedByList = Project.CurrentProject.References.GetReferrers(variable);
            usedByList.ForEach(r => _viewModel.ReferenceTriggers.Add(r));
            if (usedByList.Count == 0)
            {
                listViewUsedBy.Visibility = Visibility.Hidden;
                lblUsedBy.Visibility = Visibility.Hidden;
            }

            preventStateChange = false;
            variable.PropertyChanged += Variable_ValuesChanged;
            
            textblockInitialValue.Inlines.Clear();
            ParamTextBuilder paramTextBuilder = new ParamTextBuilder();
            var inlines = paramTextBuilder.GenerateParamText(variable);
            textblockInitialValue.Inlines.AddRange(inlines);

            this.Loaded += VariableControl_Loaded;
        }

        /// <summary>
        /// Prevents 'Changed' events from firing when opening the control,
        /// since they only hook after the values have been set.
        /// </summary>
        private void VariableControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.comboBoxVariableType.SelectionChanged += comboBoxVariableType_SelectionChanged;
            this.comboBoxArrayDimensions.SelectionChanged += comboBoxArrayDimensions_SelectionChanged;
            this.textBoxArraySize0.TextChanged += textBoxArraySize0_TextChanged;
            this.textBoxArraySize1.TextChanged += textBoxArraySize1_TextChanged;
        }

        private void Variable_ValuesChanged()
        {
            textblockInitialValue.Inlines.Clear();
            ParamTextBuilder paramTextBuilder = new ParamTextBuilder();
            var inlines = paramTextBuilder.GenerateParamText(variable);
            textblockInitialValue.Inlines.AddRange(inlines);

            OnChange?.Invoke();
        }

        public void UpdateIdentifierText()
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                this.textBlockVariableNameUDG.Text = variable.GetIdentifierName();
            });
        }

        private void comboBoxVariableType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (suppressUIEvents)
                return;

            if (ResetVarRefs())
            {
                var selected = (War3Type)comboBoxVariableType.SelectedItem;

                CommandVariableModifyType command = new CommandVariableModifyType(variable, selected.Type);
                command.Execute();
                OnStateChange();

                _viewModel.SelectedItemPrevious = (War3Type)comboBoxVariableType.SelectedItem;

                ParamTextBuilder controllerParamText = new ParamTextBuilder();
                this.textblockInitialValue.Inlines.Clear();
                var inlines = controllerParamText.GenerateParamText(variable);
                this.textblockInitialValue.Inlines.AddRange(inlines);
            }
            else
            {
                comboBoxVariableType.SelectedItem = _viewModel.SelectedItemPrevious;
                e.Handled = false;
            }
        }

        private void checkBoxIsArray_Click(object sender, RoutedEventArgs e)
        {
            if (ResetVarRefs())
            {
                CommandVariableModifyArray command = new CommandVariableModifyArray(variable, (bool)checkBoxIsArray.IsChecked);
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
            bool isTwoDimensions = comboBoxArrayDimensions.SelectedIndex == 1;
            if (ResetVarRefs())
            {
                CommandVariableModifyDimension command = new CommandVariableModifyDimension(variable, isTwoDimensions);
                command.Execute();
                OnStateChange();
            }
            else
            {
                if (!variable.IsTwoDimensions)
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
            List<ExplorerElement> refs = Project.CurrentProject.References.GetReferrers(this.variable);
            if (refs.Count > 0)
            {
                DialogBoxReferences dialog = new DialogBoxReferences(refs, ExplorerAction.Reset);
                dialog.ShowDialog();
                ok = dialog.OK;
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

        public void OnStateChange()
        {
            OnChange?.Invoke();
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

                variable.ArraySize[0] = size0;
                variable.ArraySize[1] = size1;

                OnStateChange();
            }
            catch (Exception ex)
            {
                textBoxArraySize0.Text = previousText0;
                textBoxArraySize1.Text = previousText1;
            }

            //e.Handled = true;
        }

        internal void Dispose()
        {
            variable.PropertyChanged -= Variable_ValuesChanged;
        }

        private void listViewUsedBy_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = listViewUsedBy.SelectedItem as ListViewItem;
            if (item == null)
                return;

            var element = (ExplorerElement) listViewUsedBy.ItemContainerGenerator.ItemFromContainer(item);
            var triggerExplorer = TriggerExplorer.Current;
            triggerExplorer.NavigateToExplorerElement(element);
        }
    }
}
