using BetterTriggers.Controllers;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.Models.War3Data;
using BetterTriggers.Utility;
using BetterTriggers.WorldEdit;
using GUI.Components.Shared;
using GUI.Utility;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace GUI.Components.TriggerEditor.ParameterControls
{
    public partial class ValueControlIcons : UserControl, IValueControl
    {
        public event EventHandler SelectionChanged;
        public event EventHandler OK;


        public ValueControlIcons()
        {
            InitializeComponent();

            var icons = Icon.GetAll();
            List<Searchable> objects = new List<Searchable>();
            for (int i = 0; i < icons.Count; i++)
            {
                ListViewItem listItem = new ListViewItem();
                listItem.Content = $"{icons[i].displayName}";
                listItem.Tag = icons[i];
                objects.Add(new Searchable()
                {
                    Object = listItem,
                    Category = icons[i].category,
                    Words = new List<string>()
                    {
                        icons[i].displayName.ToLower(),
                        icons[i].path.ToLower()
                    },
                });
            }

            var searchables = new Searchables(objects);
            listControl.SetSearchableList(searchables);

            var categoryControl = new GenericCategoryControl(searchables);
            grid.Children.Add(categoryControl);
            Grid.SetRow(categoryControl, 0);
            Grid.SetRowSpan(categoryControl, 2);

            listControl.listView.SelectionChanged += ListView_SelectionChanged;
            listControl.listView.MouseDoubleClick += ListView_MouseDoubleClick;
        }


        public void SetDefaultSelection(Parameter parameter)
        {
            textBoxAsset.Text = parameter.value;
        }

        public int GetElementCount()
        {
            return listControl.listView.Items.Count;
        }

        public Parameter GetSelected()
        {
            return new Value()
            {
                value = textBoxAsset.Text,
            };
        }

        public void SetVisibility(Visibility visibility)
        {
            this.Visibility = visibility;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = listControl.listView.SelectedItem as ListViewItem;
            if (selectedItem == null)
                return;

            var icon = (Icon)selectedItem.Tag;
            textBoxAsset.Text = icon.path;
        }

        private void ListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            EventHandler handler = OK;
            handler?.Invoke(this, e);
        }

        private void textBoxAsset_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var bitmap = Icon.Get(textBoxAsset.Text);
                imgPreview.Source = BitmapConverter.ToBitmapImage(bitmap);
                EventHandler handler = SelectionChanged;
                handler?.Invoke(this, e);
            }
            catch (Exception ex)
            {
                MessageBox messageBox = new MessageBox("Error", ex.Message);
                messageBox.ShowDialog();
            }
        }
    }
}
