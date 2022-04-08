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
    public partial class ValueControlMapDestructibles : UserControl, IValueControl
    {
        private ListViewItem selectedItem;

        public ValueControlMapDestructibles()
        {
            InitializeComponent();

            var destructibles = Destructibles.Load();

            for (int i = 0; i < destructibles.Count; i++)
            {
                var dest = destructibles[i];
                Value value = new Value()
                {
                    identifier = $"[{dest.ToString().Substring(0, 4)}]_{dest.CreationNumber}",
                    returnType = "destructable",
                };
                ListViewItem item = new ListViewItem();
                item.Content = $"{value.identifier}";
                item.Tag = value;

                listView.Items.Add(item);
                this.selectedItem = listView.Items.GetItemAt(0) as ListViewItem;
            }
        }

        public int GetElementCount()
        {
            return listView.Items.Count;
        }

        public Parameter GetSelected()
        {
            return (Value)this.selectedItem.Tag;
        }

        private void listViewAbilities_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedItem = listView.SelectedItem as ListViewItem;
        }
    }
}
