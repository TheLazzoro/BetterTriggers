using BetterTriggers.Controllers;
using BetterTriggers.Utility;
using GUI.Components.Shared;
using Model.SaveableData;
using Model.Templates;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace GUI.Components.TriggerEditor.ParameterControls
{
    public partial class ParameterFunctionControl : UserControl, IParameterControl
    {
        private ListViewItem selectedItem;

        public ParameterFunctionControl(string returnType)
        {
            InitializeComponent();

            ControllerTriggerData controller = new ControllerTriggerData();
            List<FunctionTemplate> functions = controller.LoadAllCalls();
            List<Searchable> objects = new List<Searchable>();

            for (int i = 0; i < functions.Count; i++)
            {
                if (functions[i].returnType != returnType)
                    continue;

                ListViewItem listItem = new ListViewItem();
                listItem.Content = functions[i].name;
                listItem.Tag = functions[i];
                objects.Add(new Searchable()
                {
                    Object = listItem,
                    Category = (int) functions[i].category,
                    Words = new List<string>()
                    {
                        functions[i].name.ToLower(),
                        functions[i].identifier.ToLower()
                    },
                });
            }
            var searchables = new Searchables(objects);
            listControl.SetSearchableList(searchables);

            var categoryControl = new GenericCategoryControl(searchables);
            grid.Children.Add(categoryControl);
            Grid.SetRow(categoryControl, 1);
            Grid.SetRowSpan(categoryControl, 3);

            listControl.listView.SelectionChanged += ListView_SelectionChanged;
        }

        public int GetElementCount()
        {
            return listControl.listView.Items.Count;
        }

        public Parameter GetSelectedItem()
        {
            var template = (FunctionTemplate)selectedItem.Tag;
            var parameter = new Function()
            {
                identifier = template.identifier,
                parameters = template.parameters,
                returnType = template.returnType,
            };
            return parameter;
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
