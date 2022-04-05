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
    public partial class ValueControlUpgrades : UserControl, IValueControl
    {
        private ListViewItem selectedItem;

        public ValueControlUpgrades()
        {
            InitializeComponent();

            var upgrades = UpgradeData.GetUpgradesAll();

            for (int i = 0; i < upgrades.Count; i++)
            {
                var upgrade = upgrades[i];
                Value value = new Value()
                {
                    identifier = upgrade.UpgradeCode,
                    returnType = "buffcode",
                };
                ListViewItem item = new ListViewItem();
                item.Content = $"[{upgrade.UpgradeCode}] {upgrade.DisplayName}";
                item.Tag = value;

                listViewBuffs.Items.Add(item);
                this.selectedItem = listViewBuffs.Items.GetItemAt(0) as ListViewItem;
            }
        }

        public int GetElementCount()
        {
            return listViewBuffs.Items.Count;
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
            selectedItem = listViewBuffs.SelectedItem as ListViewItem;
        }
    }
}
