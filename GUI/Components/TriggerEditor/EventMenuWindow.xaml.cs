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
using DataAccess;

namespace GUI
{
    /// <summary>
    /// Interaction logic for EventMenuWindow.xaml
    /// </summary>
    public partial class EventMenuWindow : Window
    {
        public DataAccess.Natives.Function selectedEvent;

        public EventMenuWindow()
        {
            InitializeComponent();

            listViewCategory.Items.Add("- All");
            listViewCategory.Items.Add("- General");
            listViewCategory.Items.Add("Ability");

            List<DataAccess.Natives.Function> events = DataAccess.LoadData.LoadAllEvents(@"C:\Users\Lasse Dam\Desktop\JSON\events.json");

            for(int i = 0; i < events.Count; i++)
            {
                ListViewItem item = new ListViewItem();
                item.Content = events[i].name;
                item.Tag = events[i];
                listViewEvents.Items.Add(item);
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            var item = (ListViewItem) listViewEvents.SelectedItem;
            selectedEvent = (DataAccess.Natives.Function) item.Tag;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void listViewEvents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnOK.IsEnabled = true;
        }
    }
}
