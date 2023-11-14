using BetterTriggers.Containers;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.Utility;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace GUI.Components.TriggerEditor.ParameterControls
{
    public partial class ParameterTriggerControl : UserControl, IParameterControl
    {
        private ListViewItem selectedItem;

        public ParameterTriggerControl()
        {
            InitializeComponent();

            var project = Project.CurrentProject;
            List<TriggerRef> triggers = project.Triggers.GetTriggerRefs();
            List<Searchable> objects = new List<Searchable>();
            for (int i = 0; i < triggers.Count; i++)
            {
                string triggerName = project.Triggers.GetName(triggers[i].TriggerId);
                ListViewItem listItem = new ListViewItem();
                listItem.Content = triggerName;
                listItem.Tag = triggers[i];

                objects.Add(new Searchable()
                {
                    Object = listItem,
                    Words = new List<string>()
                    {
                        triggerName.ToLower()
                    },
                });
            }

            var searchables = new Searchables(objects);
            listControl.SetSearchableList(searchables);

            listControl.listView.SelectionChanged += ListView_SelectionChanged;
        }

        public void SetDefaultSelection(Parameter parameter)
        {
            int i = 0;
            bool found = false;
            while (!found && i < listControl.listView.Items.Count)
            {
                var item = listControl.listView.Items[i] as ListViewItem;
                var triggerRef = item.Tag as TriggerRef;
                if (triggerRef.value == parameter.value)
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

            var trigger = (TriggerRef)selectedItem.Tag;
            return trigger;
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
