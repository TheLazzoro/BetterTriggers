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
using BetterTriggers.WorldEdit;
using GUI.Controllers;
using Model;
using Model.SaveableData;
using Model.Templates;

namespace GUI.Components.TriggerEditor.ParameterControls
{
    public partial class ValueControlAbilities : UserControl, IValueControl
    {
        private ListViewItem selectedItem;

        public ValueControlAbilities()
        {
            InitializeComponent();

            var abilities = AbilityTypes.GetAbilitiesAll();

            for (int i = 0; i < abilities.Count; i++)
            {
                var ability = abilities[i];
                Value value = new Value()
                {
                    identifier = ability.AbilCode,
                    returnType = "abilcode",
                };
                ListViewItem item = new ListViewItem();
                item.Content = $"[{ability.AbilCode}] {ability.DisplayName}";
                item.Tag = value;

                listViewAbilities.Items.Add(item);
                this.selectedItem = listViewAbilities.Items.GetItemAt(0) as ListViewItem;
            }
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
