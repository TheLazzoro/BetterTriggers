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
using BetterTriggers.Containers;
using BetterTriggers.Controllers;

namespace GUI.Components.TriggerEditor
{
    /// <summary>
    /// Interaction logic for EventMenuWindow.xaml
    /// </summary>
    public partial class TriggerElementMenuWindow : Window
    {
        public Function selectedTriggerElement;

        public TriggerElementMenuWindow(TriggerElementType triggerElementType)
        {
            InitializeComponent();

            listViewCategory.Items.Add("- All");
            listViewCategory.Items.Add("- General");
            listViewCategory.Items.Add("Ability");

            var controllerTriggerData = new ControllerTriggerData();
            var templates = new List<FunctionTemplate>();
            if (triggerElementType == TriggerElementType.Event)
            {
                listViewHeader.Header = "Event";
                templates = controllerTriggerData.LoadAllEvents();
            }
            else if (triggerElementType == TriggerElementType.Condition)
            {
                listViewHeader.Header = "Condition";
                templates = controllerTriggerData.LoadAllConditions();
            }
            else if (triggerElementType == TriggerElementType.Action)
            {
                listViewHeader.Header = "Action";
                templates = controllerTriggerData.LoadAllActions();
            }


            for (int i = 0; i < templates.Count; i++)
            {
                var template = templates[i];
                ListViewItem item = new ListViewItem();
                item.Content = template.name;
                item.Tag = template.ToTriggerElement();
                listView.Items.Add(item);
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            var item = (ListViewItem)listView.SelectedItem;
            selectedTriggerElement = (Function)item.Tag;
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
