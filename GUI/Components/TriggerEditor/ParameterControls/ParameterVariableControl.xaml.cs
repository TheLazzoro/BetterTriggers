using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Utility;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace GUI.Components.TriggerEditor.ParameterControls
{
    public partial class ParameterVariableControl : UserControl, IParameterControl
    {
        private ListViewItem selectedItem;

        /// <summary>
        /// </summary>
        /// <param name="returnType"></param>
        /// <param name="localVariables"></param>
        public ParameterVariableControl(string returnType, TriggerElementCollection? localVariables = null)
        {
            InitializeComponent();

            if (returnType == "VarAsString_Real")
                returnType = "real";
            else if (returnType == "StringExt")
                returnType = "string";

            var project = Project.CurrentProject;
            List<VariableRef> variables = project.Variables.GetVariableRefs(returnType, Variables.includeLocals, localVariables);
            List<Searchable> objects = new List<Searchable>();

            for (int i = 0; i < variables.Count; i++)
            {
                string varName = project.Variables.GetVariableNameById(variables[i].VariableId);
                ListViewItem listItem = new ListViewItem();
                listItem.Content = varName;
                listItem.Tag = variables[i];

                objects.Add(new Searchable()
                {
                    Object = listItem,
                    Words = new List<string>()
                    {
                        varName.ToLower()
                    },
                });
            }
            var searchables = new Searchables(objects);
            listControl.SetSearchableList(searchables);
            listControl.listView.SelectionChanged += ListView_SelectionChanged;
        }

        public void SetDefaultSelection(Parameter parameter)
        {
            var project = Project.CurrentProject;
            int i = 0;
            bool found = false;
            Variable selected = null;
            if (parameter is VariableRef)
                selected = project.Variables.GetByReference(parameter as VariableRef);

            if (selected == null)
                return;

            while (!found && i < listControl.listView.Items.Count)
            {
                var item = listControl.listView.Items[i] as ListViewItem;
                var variableRef = item.Tag as VariableRef;
                var variable = project.Variables.GetByReference(variableRef);
                if (variable == selected)
                    found = true;
                else
                    i++;
            }
            if (!found)
                return;

            var defaultSelected = listControl.listView.Items[i] as ListViewItem;
            defaultSelected.IsSelected = true;
            listControl.listView.ScrollIntoView(defaultSelected);
        }


        public int GetElementCount()
        {
            return listControl.listView.Items.Count;
        }

        public Parameter GetSelectedItem()
        {
            if (selectedItem == null)
                return null;

            var variables = (VariableRef)selectedItem.Tag;
            return variables;
        }

        public void SetVisibility(Visibility visibility)
        {
            this.Visibility = visibility;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedItem = listControl.listView.SelectedItem as ListViewItem;
        }
    }
}
