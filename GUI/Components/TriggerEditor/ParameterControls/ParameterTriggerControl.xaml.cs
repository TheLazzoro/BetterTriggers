using BetterTriggers.Controllers;
using BetterTriggers.Utility;
using Model.EditorData;
using Model.SaveableData;
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

            ControllerTrigger controllerTrig = new ControllerTrigger();
            List<TriggerRef> triggers = controllerTrig.GetTriggerRefs();
            List<Searchable> objects = new List<Searchable>();

            for (int i = 0; i < triggers.Count; i++)
            {
                string triggerName = controllerTrig.GetTriggerName(triggers[i].TriggerId);
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
                var searchables = new Searchables(objects);
                listControl.SetSearchableList(searchables);

                listControl.listView.SelectionChanged += ListView_SelectionChanged;
            }
        }

        public void SetDefaultSelection(string identifier)
        {
            int i = 0;
            bool found = false;
            while (!found && i < listControl.listView.Items.Count)
            {
                var item = listControl.listView.Items[i] as ListViewItem;
                var triggerRef = item.Tag as TriggerRef;
                if(triggerRef.identifier == identifier)
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
