using BetterTriggers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.Models.Templates;
using BetterTriggers.Utility;
using BetterTriggers.WorldEdit;
using GUI.Components.Shared;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GUI.Components.TriggerEditor.ParameterControls
{
    public partial class ParameterFunctionControl : UserControl, IParameterControl
    {
        private ListViewItem selectedItem;

        public ParameterFunctionControl(string returnType)
        {
            InitializeComponent();

            if (returnType == "StringExt")
                returnType = "string";

            List<FunctionTemplate> functions = TriggerData.LoadAllCalls(returnType);
            List<Searchable> objects = new List<Searchable>();

            for (int i = 0; i < functions.Count; i++)
            {
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
                        functions[i].value.ToLower()
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


        public void SetDefaultSelection(Parameter parameter)
        {
            int i = 0;
            bool found = false;
            while (!found && i < listControl.listView.Items.Count)
            {
                var item = listControl.listView.Items[i] as ListViewItem;
                var function = item.Tag as FunctionTemplate;
                if(function.value == parameter.value)
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
                value = template.value,
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
            textBoxDescription.Inlines.Clear();
            var brush = (SolidColorBrush)Application.Current.FindResource("TextBrush");
            string colorCode = brush.Color.ToString().Replace("#", "");
            textBoxDescription.Inlines.AddRange(Utility.TextFormatter.War3ColoredText(Locale.Translate(function.value), colorCode));
        }

    }
}
