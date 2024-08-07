﻿using BetterTriggers.Commands;
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

namespace GUI.Components.Return
{
    public partial class ReturnTypeControl : UserControl
    {
        private ExplorerElement explorerElement;
        private FunctionDefinition functionDefinition;
        private War3Type previousSelected;

        public ReturnTypeControl(ExplorerElement explorerElement, FunctionDefinition functionDefinition, ReturnType returnType)
        {
            InitializeComponent();

            this.explorerElement = explorerElement;
            this.functionDefinition = functionDefinition;
            previousSelected = returnType.War3Type;
            DataContext = new ReturnTypeViewModel(returnType);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(!IsLoaded)
            {
                return;
            }

            List<ExplorerElement> refs = Project.CurrentProject.References.GetReferrers(functionDefinition);
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
            CommandFunctionDefinitionModifyType command = new(explorerElement, functionDefinition, selected);
            command.Execute();
        }
    }
}
