using BetterTriggers.Controllers;
using BetterTriggers.Utility;
using Model.EditorData;
using Model.SaveableData;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace GUI.Components.TriggerEditor.ParameterControls
{
    public partial class ParameterVariableControl : UserControl, IParameterControl
    {
        private ListViewItem selectedItem;

        public ParameterVariableControl(string returnType)
        {
            InitializeComponent();

            ControllerVariable controllerVariable = new ControllerVariable();
            List<ExplorerElementVariable> elements = controllerVariable.GetExplorerElementAll();
            List<Variable> variables = new List<Variable>();
            List<Searchable> objects = new List<Searchable>();

            for (int i = 0; i < elements.Count; i++)
            {
                if (returnType != "AnyGlobal" && elements[i].variable.Type != returnType)
                    continue;

                var explorerElement = elements[i];
                string name = System.IO.Path.GetFileNameWithoutExtension(explorerElement.GetPath());
                Variable variable = explorerElement.variable;
                variable.Name = name;
                variables.Add(variable);
            }

            // Creates a list of saveable variables
            for (int i = 0; i < variables.Count; i++)
            {
                ListViewItem listItem = new ListViewItem();
                listItem.Content = variables[i].Name;
                VariableRef varRef = new VariableRef()
                {
                    identifier = controllerVariable.GetVariableNameById(variables[i].Id),
                    returnType = variables[i].Type,
                    VariableId = variables[i].Id,
                };
                varRef.arrayIndexValues.Add(new Value() { returnType = "integer", identifier = "0" });
                varRef.arrayIndexValues.Add(new Value() { returnType = "integer", identifier = "0" });
                listItem.Tag = varRef;

                objects.Add(new Searchable()
                {
                    Object = listItem,
                    Words = new List<string>()
                    {
                        variables[i].Name.ToLower()
                    },
                });
                var searchables = new Searchables(objects);
                listControl.SetSearchableList(searchables);

                listControl.listView.SelectionChanged += ListView_SelectionChanged;
            }
        }

        public int GetElementCount()
        {
            return listControl.listView.Items.Count;
        }

        public Parameter GetSelectedItem()
        {
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
