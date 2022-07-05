using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BetterTriggers.Controllers;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.WorldEdit;
using GUI.Controllers;

namespace GUI.Components.TriggerEditor.ParameterControls
{
    public partial class ValueControlAbilities : UserControl, IValueControl
    {
        private ListViewItem selectedItem;

        public event EventHandler SelectionChanged;

        public ValueControlAbilities()
        {
            InitializeComponent();

            var controller = new ControllerMapData();
            var abilities = controller.GetAbilitiesAll();

            for (int i = 0; i < abilities.Count; i++)
            {
                var ability = abilities[i];
                Value value = new Value() { identifier = ability.AbilCode };
                ListViewItem item = new ListViewItem();
                item.Content = $"[{ability.AbilCode}] {ability.DisplayName}";
                item.Tag = value;

                listViewAbilities.Items.Add(item);
                this.selectedItem = listViewAbilities.Items.GetItemAt(0) as ListViewItem;
            }

            listViewAbilities.SelectionChanged += ListViewAbilities_SelectionChanged;
        }


        public void SetDefaultSelection(string identifier)
        {
            int i = 0;
            bool found = false;
            while (!found && i < listViewAbilities.Items.Count)
            {
                var item = listViewAbilities.Items[i] as ListViewItem;
                var value = item.Tag as Value;
                if(value.identifier == identifier)
                    found = true;
                else
                    i++;
            }
            if (found == false)
                return;

            var defaultSelected = listViewAbilities.Items[i] as ListViewItem;
            defaultSelected.IsSelected = true;
            listViewAbilities.ScrollIntoView(defaultSelected);
        }

        private void ListViewAbilities_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EventHandler handler = SelectionChanged;
            handler?.Invoke(this, e);
        }

        public int GetElementCount()
        {
            return listViewAbilities.Items.Count;
        }

        public Parameter GetSelected()
        {
            return (Value)this.selectedItem.Tag;
        }

        public void SetVisibility(Visibility visibility)
        {
            this.Visibility = visibility;
        }

        private void listViewAbilities_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedItem = listViewAbilities.SelectedItem as ListViewItem;
        }
    }
}
