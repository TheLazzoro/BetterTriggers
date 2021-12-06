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

namespace GUI
{
    /// <summary>
    /// Interaction logic for EventMenuWindow.xaml
    /// </summary>
    public partial class ActionMenuWindow : Window
    {
        public Model.Natives.Function selectedAction;

        public ActionMenuWindow()
        {
            InitializeComponent();

            listViewCategory.Items.Add("- All");
            listViewCategory.Items.Add("- General");
            listViewCategory.Items.Add("Ability");

            List<Model.Natives.Function> actions = Model.LoadData.LoadAllFunctions(@"C:\Users\Lasse Dam\Desktop\JSON\actions.json");

            for(int i = 0; i < actions.Count; i++)
            {
                ListViewItem item = new ListViewItem();
                item.Content = actions[i].name;
                item.Tag = actions[i];
                listViewEvents.Items.Add(item);
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            var item = (ListViewItem) listViewEvents.SelectedItem;
            selectedAction = (Model.Natives.Function) item.Tag;
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
