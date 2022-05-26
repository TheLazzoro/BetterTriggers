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
            List<VariableRef> variables = controllerVariable.GetVariableRefs(returnType);
            List<Searchable> objects = new List<Searchable>();

            for (int i = 0; i < variables.Count; i++)
            {
                ListViewItem listItem = new ListViewItem();
                listItem.Content = variables[i].identifier;
                listItem.Tag = variables[i];

                objects.Add(new Searchable()
                {
                    Object = listItem,
                    Words = new List<string>()
                    {
                        variables[i].identifier.ToLower()
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
