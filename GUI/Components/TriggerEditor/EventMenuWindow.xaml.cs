using Newtonsoft.Json;
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
using System.Windows.Shapes;
using Model;
using Model.Data;
using Model.Natives;

namespace GUI
{
    /// <summary>
    /// Interaction logic for EventMenuWindow.xaml
    /// </summary>
    public partial class EventMenuWindow : Window
    {
        public Model.Natives.Function selectedEvent;

        public EventMenuWindow()
        {
            InitializeComponent();

            List<Model.Natives.Function> events = Model.LoadData.LoadAllEvents(@"C:\Users\Lasse Dam\Desktop\JSON\events.json");

            for(int i = 0; i < events.Count; i++)
            {
                ListViewItem item = new ListViewItem();
                item.Content = events[i].name;
                item.Tag = events[i];
                listViewEvents.Items.Add(item);
                AddToCategory(events[i].category);
            }
        }

        private void AddToCategory(EnumCategory categoryNumber)
        {
            int i = 0;
            bool isCategoryFound = false;
            while (!isCategoryFound && i < listViewCategory.Items.Count)
            {
                ListViewItem categoryItem = (ListViewItem) listViewCategory.Items[i];
                EnumCategory categoryInList = (EnumCategory) categoryItem.Tag;

                if (categoryInList == categoryNumber)
                    isCategoryFound = true;

                i++;
            }

            if(!isCategoryFound)
            {
                ListViewItem categoryItem = new ListViewItem();
                categoryItem.Foreground = new SolidColorBrush(Colors.White);
                categoryItem.Tag = categoryNumber;
                categoryItem.Content = categoryNumber.ToString();

                listViewCategory.Items.Add(categoryItem);
            }
        }

        private void listViewCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = listViewCategory.SelectedItem as ListViewItem;
            var selectedCategory = (EnumCategory) selectedItem.Tag;

            for (int i = 0; i < listViewEvents.Items.Count; i++)
            {
                var eventItem = (ListViewItem) listViewEvents.Items[i];
                var _event = (Function)eventItem.Tag;

                if (_event.category == selectedCategory)
                    eventItem.Visibility = Visibility.Visible;
                else
                    eventItem.Visibility = Visibility.Hidden;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void listViewEvents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnOK.IsEnabled = true;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            var item = (ListViewItem)listViewEvents.SelectedItem;
            selectedEvent = (Function)item.Tag;
            this.Close();
        }
    }
}
