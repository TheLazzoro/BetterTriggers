using GUI.Components.TriggerExplorer;
using GUI.Utility;
using Model.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace GUI.Controllers
{
    public class ControllerVariable
    {
        public void CreateVariable(Grid mainGrid, TriggerExplorer triggerExplorer)
        {
            var variableControl = new VariableControl();

            // Position editor
            mainGrid.Children.Add(variableControl);
            Grid.SetColumn(variableControl, 1);
            Grid.SetRow(variableControl, 3);
            Grid.SetRowSpan(variableControl, 2);

            string name = NameGenerator.GenerateVariableName();

            TreeViewItem item = new TreeViewItem();
            TreeViewManipulator.SetTreeViewItemAppearance(item, name, EnumCategory.SetVariable);
            Variable script = new Variable(variableControl);

            triggerExplorer.CreateTreeViewItem(item, name, Model.Data.EnumCategory.SetVariable);
        }
    }
}
