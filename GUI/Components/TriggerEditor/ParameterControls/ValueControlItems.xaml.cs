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
    public partial class ValueControlItems : UserControl, IValueControl
    {
        private ListViewItem selectedItem;

        public ValueControlItems()
        {
            InitializeComponent();

            var items = ItemData.GetItemsAll();

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                Value value = new Value()
                {
                    identifier = item.ItemCode,
                    returnType = "itemcode",
                };
                ListViewItem listItem = new ListViewItem();
                listItem.Content = $"[{item.ItemCode}] {item.DisplayName}";
                listItem.Tag = value;

                listViewDestructibles.Items.Add(listItem);
                this.selectedItem = listViewDestructibles.Items.GetItemAt(0) as ListViewItem;
            }
        }

        public int GetElementCount()
        {
            return listViewDestructibles.Items.Count;
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
            selectedItem = listViewDestructibles.SelectedItem as ListViewItem;
        }
    }
}
