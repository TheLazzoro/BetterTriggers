using BetterTriggers;
using BetterTriggers.Controllers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.Models.Templates;
using BetterTriggers.Utility;
using GUI.Components.Shared;
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
            List<FunctionTemplate> functions = controller.LoadAllCalls(returnType);
            List<Searchable> objects = new List<Searchable>();

            for (int i = 0; i < functions.Count; i++)
            {
                if (functions[i].returnType != returnType)
                    continue;

                Category category = Category.Get(functions[i].category);
                string categoryStr = Locale.Translate(category.Name) ;
                if (categoryStr == "Nothing")
                    categoryStr = "";
                else
                    categoryStr += " - ";

                ListViewItem listItem = new ListViewItem();
                listItem.Content = categoryStr + functions[i].name;
                listItem.Tag = functions[i];
                objects.Add(new Searchable()
                {
                    Object = listItem,
                    Category = Locale.Translate(category.Name),
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

        public void SetDefaultSelection(string identifier)
        {
            int i = 0;
            bool found = false;
            while (!found && i < listControl.listView.Items.Count)
            {
                var item = listControl.listView.Items[i] as ListViewItem;
                var function = item.Tag as FunctionTemplate;
                if(function.identifier == identifier)
                    found = true;
                else
                    i++;
            }
            if (found == false)
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

            var template = (FunctionTemplate)selectedItem.Tag;
            var parameter = new Function()
            {
                identifier = template.identifier,
                parameters = template.ConvertParameters(),
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
            if (selectedItem == null)
                return;

            var function = selectedItem.Tag as FunctionTemplate;
            textBoxDescription.Text = Locale.Translate(function.identifier);
        }
    }
}
