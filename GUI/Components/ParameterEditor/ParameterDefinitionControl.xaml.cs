using BetterTriggers.Commands;
using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using GUI.Components.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace GUI.Components.ParameterEditor
{
    public partial class ParameterDefinitionControl : UserControl
    {
        private ExplorerElement _explorerElement;
        private ParameterDefinition _definition;
        private ParameterDefinitionViewModel viewModel;
        private War3Type previousSelected;
        public event Action OnChanged;

        public ParameterDefinitionControl(ExplorerElement explorerElement, ParameterDefinition definition)
        {
            InitializeComponent();

            _explorerElement = explorerElement;
            _definition = definition;
            previousSelected = definition.ReturnType;
            viewModel = new ParameterDefinitionViewModel(definition);
            DataContext = viewModel;

            viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnChanged?.Invoke();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded || viewModel.SuppressChangedEvents)
            {
                return;
            }

            // Gets a list of its own uses
            List<ExplorerElement> refs = Project.CurrentProject.References.GetReferrers(_definition);

            // Gets a list of all other explorer elements with a reference to this explorer element.
            IReferable referable = null;
            switch (_explorerElement.ElementType)
            {
                case ExplorerElementEnum.ActionDefinition:
                    referable = _explorerElement.actionDefinition;
                    break;
                case ExplorerElementEnum.ConditionDefinition:
                    referable = _explorerElement.conditionDefinition;
                    break;
                case ExplorerElementEnum.FunctionDefinition:
                    referable = _explorerElement.functionDefinition;
                    break;
                default:
                    break;
            }

            refs.AddRange(Project.CurrentProject.References.GetReferrers(referable));
            if (refs.Count > 0)
            {
                DialogBoxReferences dialog = new DialogBoxReferences(refs, ExplorerAction.Reset);
                dialog.ShowDialog();
                if (!dialog.OK)
                {
                    comboBox.SelectionChanged -= ComboBox_SelectionChanged; // hack, but prevents dialog box from showing twice :)
                    comboBox.SelectedItem = previousSelected;
                    comboBox.SelectionChanged += ComboBox_SelectionChanged;
                    return;
                }
            }

            var selected = comboBox.SelectedItem as War3Type;
            previousSelected = selected;
            CommandParameterDefinitionModifyType command = new(_explorerElement, _definition, selected);
            command.Execute();
        }
    }
}
