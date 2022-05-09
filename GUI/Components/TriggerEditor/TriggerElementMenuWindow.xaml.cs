using BetterTriggers.Controllers;
using BetterTriggers.Utility;
using GUI.Components.Shared;
using Model.SaveableData;
using Model.Templates;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace GUI.Components.TriggerEditor
{
    public partial class TriggerElementMenuWindow : Window
    {
        public TriggerElement createdTriggerElement;

        public TriggerElementMenuWindow(TriggerElementType triggerElementType)
        {
            InitializeComponent();

            var controllerTriggerData = new ControllerTriggerData();
            var templates = new List<FunctionTemplate>();
            if (triggerElementType == TriggerElementType.Event)
            {
                templates = controllerTriggerData.LoadAllEvents();
            }
            else if (triggerElementType == TriggerElementType.Condition)
            {
                templates = controllerTriggerData.LoadAllConditions();
            }
            else if (triggerElementType == TriggerElementType.Action)
            {
                templates = controllerTriggerData.LoadAllActions();
            }

            List<Searchable> objects = new List<Searchable>();
            for (int i = 0; i < templates.Count; i++)
            {
                ListViewItem listItem = new ListViewItem();
                listItem.Content = templates[i].name;
                listItem.Tag = templates[i].ToTriggerElement();
                objects.Add(new Searchable()
                {
                    Object = listItem,
                    Category = templates[i].category.ToString(),
                    Words = new List<string>()
                    {
                        templates[i].name.ToLower(),
                        templates[i].identifier.ToLower()
                    },
                });
            }
            var searchables = new Searchables(objects);
            listControl.SetSearchableList(searchables);
            listControl.ListViewChanged += delegate
            {
                btnOK.IsEnabled = listControl.GetItemsCount() > 0;
            };

            var categoryControl = new GenericCategoryControl(searchables);
            categoryControl.Margin = new Thickness(0, 0, 4, 0);
            grid.Children.Add(categoryControl);
            Grid.SetRow(categoryControl, 1);
            Grid.SetRowSpan(categoryControl, 3);
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            var item = (ListViewItem)listControl.listView.SelectedItem;
            createdTriggerElement = (TriggerElement)item.Tag;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
