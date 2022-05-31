using BetterTriggers.Controllers;
using BetterTriggers.Utility;
using Model.SaveableData;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace GUI.Components.TriggerEditor.ParameterControls
{
    public partial class ValueControlGeneric : UserControl, IValueControl
    {
        private ListViewItem selectedItem;

        public ValueControlGeneric(List<Value> values)
        {
            InitializeComponent();

            ControllerTrigger controller = new ControllerTrigger();

            List<Searchable> objects = new List<Searchable>();

            for (int i = 0; i < values.Count; i++)
            {
                ListViewItem listItem = new ListViewItem();
                listItem.Content = $"{controller.GetFourCCDisplay(values[i].identifier, values[i].returnType)}{controller.GetValueName(values[i].identifier, values[i].returnType)}";
                listItem.Tag = values[i];
                objects.Add(new Searchable()
                {
                    Object = listItem,
                    Words = new List<string>()
                    {
                        "PLACEHOLDER",
                        values[i].identifier.ToLower()
                    },
                });
            }

            var searchables = new Searchables(objects);
            listControl.SetSearchableList(searchables);

            listControl.listView.SelectionChanged += ListView_SelectionChanged;
        }

        

        public int GetElementCount()
        {
            return listControl.listView.Items.Count;
        }

        public Parameter GetSelected()
        {
            return (Value)this.selectedItem.Tag;
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
