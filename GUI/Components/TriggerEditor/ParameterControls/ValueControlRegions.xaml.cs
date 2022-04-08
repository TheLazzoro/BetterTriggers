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
    public partial class ValueControlRegions : UserControl, IValueControl
    {
        private ListViewItem selectedItem;

        public ValueControlRegions()
        {
            InitializeComponent();

            var regions = Regions.Load();

            for (int i = 0; i < regions.Count; i++)
            {
                var region = regions[i];
                Value value = new Value()
                {
                    identifier = region.Name,
                    returnType = "rect",
                };
                ListViewItem item = new ListViewItem();
                item.Content = $"{region.Name}";
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
