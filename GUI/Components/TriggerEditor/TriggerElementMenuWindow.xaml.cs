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
using BetterTriggers.Utility;

namespace GUI.Components.TriggerEditor
{
    /// <summary>
    /// Interaction logic for EventMenuWindow.xaml
    /// </summary>
    public partial class TriggerElementMenuWindow : Window
    {
        public TriggerElement createdTriggerElement;
        private SearchObjects searchObjects;

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

            List<SearchObject> objects = new List<SearchObject>();
            for (int i = 0; i < templates.Count; i++)
            {
                ListViewItem listItem = new ListViewItem();
                listItem.Content = templates[i].name;
                listItem.Tag = templates[i].ToTriggerElement();
                objects.Add(new SearchObject()
                {
                    Object = listItem,
                    Words = new List<string>()
                    {
                        templates[i].name.ToLower(),
                        templates[i].identifier.ToLower(),
                        string.Empty
                    },
                });
            }
            searchObjects = new SearchObjects(objects);
            PopulateList("");
        }

        private void PopulateList(string searchWord)
        {
            listView.Items.Clear();
            var list = searchObjects.Search(searchWord);
            for (int i = 0; i < list.Count; i++)
            {
                listView.Items.Add(list[i].Object);
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            var item = (ListViewItem)listView.SelectedItem;
            createdTriggerElement = (TriggerElement)item.Tag;
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

        private void textBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            PopulateList(textBoxSearch.Text);
        }
    }
}
