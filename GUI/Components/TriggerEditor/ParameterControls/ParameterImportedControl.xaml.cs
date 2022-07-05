using BetterTriggers.Controllers;
using BetterTriggers.Utility;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System;
using BetterTriggers.Models.SaveableData;

namespace GUI.Components.TriggerEditor.ParameterControls
{
    public partial class ParameterImportedControl : UserControl, IParameterControl
    {
        private ListViewItem selectedItem;

        public ParameterImportedControl()
        {
            InitializeComponent();

            var imports = ControllerImports.GetImportsAll();
            List<Searchable> objects = new List<Searchable>();

            for (int i = 0; i < imports.Count; i++)
            {
                ListViewItem listItem = new ListViewItem();
                listItem.Content = imports[i].identifier;
                listItem.Tag = imports[i];

                objects.Add(new Searchable()
                {
                    Object = listItem,
                    Words = new List<string>()
                    {
                        imports[i].identifier.ToLower()
                    },
                });
            }
            var searchables = new Searchables(objects);
            listControl.SetSearchableList(searchables);

            listControl.listView.SelectionChanged += ListView_SelectionChanged;
        }

        public void SetDefaultSelection(string identifier)
        {
            int i = 0;
            bool found = false;
            while (!found && i < listControl.listView.Items.Count)
            {
                ListViewItem item = listControl.listView.Items[i] as ListViewItem;
                Value value = item.Tag as Value;
                if (identifier == value.identifier)
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

            var variables = (Value)selectedItem.Tag;
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
