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
using Model.SavableTriggerData;
using GUI.Containers;
using Model.Templates;

namespace GUI
{
    /// <summary>
    /// Interaction logic for EventMenuWindow.xaml
    /// </summary>
    public partial class ConditionMenuWindow : Window
    {
        public Function selectedCondition;

        public ConditionMenuWindow()
        {
            InitializeComponent();

            listViewCategory.Items.Add("- All");
            listViewCategory.Items.Add("- General");
            listViewCategory.Items.Add("Ability");

            for(int i = 0; i < ContainerTriggerData.ConditionTemplates.Count; i++)
            {
                var condition = ContainerTriggerData.ConditionTemplates[i];


                ListViewItem item = new ListViewItem();
                item.Content = condition.name;
                item.Tag = condition;
                listViewEvents.Items.Add(item);
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            var item = (ListViewItem) listViewEvents.SelectedItem;
            var template = (FunctionTemplate) item.Tag;
            selectedCondition = new Function()
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
