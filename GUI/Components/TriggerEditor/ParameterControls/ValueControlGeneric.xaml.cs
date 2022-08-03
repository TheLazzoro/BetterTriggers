using BetterTriggers.Controllers;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.Utility;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace GUI.Components.TriggerEditor.ParameterControls
{
    public partial class ValueControlGeneric : UserControl, IValueControl
    {
        private ListViewItem selectedItem;

        public event EventHandler SelectionChanged;
        public event EventHandler OK;

        public ValueControlGeneric(List<Value> values, string returnType)
        {
            InitializeComponent();

            ControllerTrigger controller = new ControllerTrigger();

            List<Searchable> objects = new List<Searchable>();

            for (int i = 0; i < values.Count; i++)
            {
                ListViewItem listItem = new ListViewItem();
                listItem.Content = $"{controller.GetFourCCDisplay(values[i].value, returnType)}{controller.GetValueName(values[i].value, returnType)}";
                listItem.Tag = values[i];
                objects.Add(new Searchable()
                {
                    Object = listItem,
                    Words = new List<string>()
                    {
                        controller.GetValueName(values[i].value, returnType).ToLower(),
                        values[i].value.ToLower()
                    },
                });
            }

            var searchables = new Searchables(objects);
            listControl.SetSearchableList(searchables);

            listControl.listView.SelectionChanged += ListView_SelectionChanged;
            listControl.listView.MouseDoubleClick += ListView_MouseDoubleClick;
        }


        public void SetDefaultSelection(string value)
        {
            int i = 0;
            bool found = false;
            while (!found && i < listControl.listView.Items.Count)
            {
                var item = listControl.listView.Items[i] as ListViewItem;
                var val = item.Tag as Value;
                if(val.value == value)
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

        public Parameter GetSelected()
        {
            if (selectedItem == null)
                return null;

            return (Value)this.selectedItem.Tag;
        }

        public void SetVisibility(Visibility visibility)
        {
            this.Visibility = visibility;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedItem = listControl.listView.SelectedItem as ListViewItem;
            EventHandler handler = SelectionChanged;
            handler?.Invoke(this, e);
        }

        private void ListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            EventHandler handler = OK;
            handler?.Invoke(this, e);
        }
    }
}
