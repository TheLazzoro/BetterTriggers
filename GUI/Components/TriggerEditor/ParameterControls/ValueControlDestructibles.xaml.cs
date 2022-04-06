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
    public partial class ValueControlDestructibles : UserControl, IValueControl
    {
        private ListViewItem selectedItem;

        public ValueControlDestructibles()
        {
            InitializeComponent();

            var destructibles = DestructibleData.GetDestructiblesAll();

            for (int i = 0; i < destructibles.Count; i++)
            {
                var destructible = destructibles[i];
                Value value = new Value()
                {
                    identifier = destructible.DestCode,
                    returnType = "buffcode",
                };
                ListViewItem item = new ListViewItem();
                item.Content = $"[{destructible.DestCode}] {destructible.DisplayName}";
                item.Tag = value;

                listViewDestructibles.Items.Add(item);
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

        private void listViewAbilities_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedItem = listViewDestructibles.SelectedItem as ListViewItem;
        }
    }
}
