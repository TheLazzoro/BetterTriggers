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
using Model.Templates;
using Model.SaveableData;
using Facades.Containers;

namespace GUI
{
    /// <summary>
    /// Interaction logic for EventMenuWindow.xaml
    /// </summary>
    public partial class ActionMenuWindow : Window
    {
        public Function selectedAction;

        public ActionMenuWindow()
        {
            InitializeComponent();

            listViewCategory.Items.Add("- All");
            listViewCategory.Items.Add("- General");
            listViewCategory.Items.Add("Ability");

            for (int i = 0; i < ContainerTriggerData.ActionTemplates.Count; i++)
            {
                var action = ContainerTriggerData.ActionTemplates[i];
                ListViewItem item = new ListViewItem();
                item.Content = action.name;
                item.Tag = action.Clone();
                listViewEvents.Items.Add(item);
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            var item = (ListViewItem)listViewEvents.SelectedItem;
            var template = (FunctionTemplate)item.Tag;
            selectedAction = new Function()
            {
                identifier = template.identifier,
                parameters = template.parameters,
                returnType = template.returnType,
            };
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
