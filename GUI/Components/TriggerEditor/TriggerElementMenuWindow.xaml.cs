using BetterTriggers;
using BetterTriggers.Controllers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.Models.Templates;
using BetterTriggers.Utility;
using GUI.Components.Shared;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GUI.Components.TriggerEditor
{
    public partial class TriggerElementMenuWindow : Window
    {
        public ECA createdTriggerElement;
        TriggerElementType triggerElementType;
        ECA selected;
        ECA previous;

        ListViewItem defaultSelected;

        public TriggerElementMenuWindow(TriggerElementType triggerElementType, ECA previous = null)
        {
            InitializeComponent();
            this.Owner = MainWindow.GetMainWindow();
            
            Settings settings = Settings.Load();
            this.Width = settings.triggerWindowWidth;
            this.Height = settings.triggerWindowHeight;
            this.Left = settings.triggerWindowX;
            this.Top = settings.triggerWindowY;

            this.triggerElementType = triggerElementType;
            this.previous = previous;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
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
                Category category = Category.Get(templates[i].category);
                string categoryStr = Locale.Translate(category.Name);
                if(categoryStr != "")
                    categoryStr += " - ";
                string name = templates[i].name != "" ? templates[i].name : templates[i].value;
                string content = categoryStr + name;
                ListViewItem listItem = new ListViewItem();
                listItem.Content = content;
                listItem.Tag = templates[i].ToTriggerElement();
                objects.Add(new Searchable()
                {
                    Object = listItem,
                    Category = Locale.Translate(category.Name),
                    Words = new List<string>()
                    {
                        content.ToLower(),
                        templates[i].value.ToLower()
                    },
                });

                // default selection
                if (previous != null && previous.function.value == templates[i].value)
                {
                    defaultSelected = listItem;
                    selected = (ECA)listItem.Tag;
                    listItem.IsSelected = true;
                }
            }


            var searchables = new Searchables(objects);
            listControl.SetSearchableList(searchables);
            listControl.ListViewChanged += delegate
            {
                btnOK.IsEnabled = selected != null && listControl.GetItemsCount() > 0;
            };
            listControl.listView.SelectionChanged += delegate
            {
                ListViewItem item = (ListViewItem)listControl.listView.SelectedItem;
                if (item == null)
                    return;

                selected = (ECA)item.Tag;
                textBoxDescription.Text = Locale.Translate(selected.function.value);
            };
            listControl.listView.MouseDoubleClick += delegate
            {
                createdTriggerElement = selected;
                this.Close();
            };

            if (selected == null)
            {
                defaultSelected = (ListViewItem)listControl.listView.Items[0];
                selected = (ECA)defaultSelected.Tag;
            }

            listControl.listView.ScrollIntoView(defaultSelected);
            defaultSelected.Focus();

            var categoryControl = new GenericCategoryControl(searchables);
            categoryControl.Margin = new Thickness(0, 0, 4, 0);
            grid.Children.Add(categoryControl);
            Grid.SetRow(categoryControl, 1);
            Grid.SetRowSpan(categoryControl, 3);
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            createdTriggerElement = selected;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && selected != null)
            {
                createdTriggerElement = selected;
                this.Close();
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Settings settings = Settings.Load();
            settings.triggerWindowWidth = (int)this.Width;
            settings.triggerWindowHeight = (int)this.Height;
        }

        private void Window_LocationChanged(object sender, System.EventArgs e)
        {
            Settings settings = Settings.Load();
            settings.triggerWindowX = (int)this.Left;
            settings.triggerWindowY = (int)this.Top;
        }
    }
}
