using BetterTriggers;
using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.Templates;
using BetterTriggers.Utility;
using BetterTriggers.WorldEdit;
using GUI.Components.Shared;
using GUI.Extensions;
using GUI.Utility;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace GUI.Components.TriggerEditor
{
    public partial class TriggerElementMenuWindow : Window
    {
        public ECA createdTriggerElement { get; private set; }
        public ECA previous { get; private set; }
        private ECA selected;

        private ExplorerElement explorerElement;
        private ListItemFunctionTemplate defaultSelected;

        private Project _project;

        public TriggerElementMenuWindow(Project project, ExplorerElement explorerElement, TriggerElementType triggerElementType, ECA previous = null)
        {
            InitializeComponent();

            _project = project;
            this.explorerElement = explorerElement;
            this.previous = previous;

            this.Owner = MainWindow.GetMainWindow();

            EditorSettings settings = EditorSettings.Load();
            this.Width = settings.triggerWindowWidth;
            this.Height = settings.triggerWindowHeight;
            this.Left = settings.triggerWindowX;
            this.Top = settings.triggerWindowY;
            this.ResetPositionWhenOutOfScreenBounds();

            Closing += TriggerElementMenuWindow_Closing;

            listControl.ListViewChanged += delegate
            {
                btnOK.IsEnabled = selected != null && listControl.GetItemsCount() > 0;
            };
            listControl.listView.SelectionChanged += delegate
            {
                var item = listControl.listView.SelectedItem as ListItemFunctionTemplate;
                if (item == null)
                    return;

                selected = item.eca;
                textBoxDescription.Text = Locale.Translate(selected.function.value);
            };
            listControl.listView.MouseDoubleClick += delegate
            {
                createdTriggerElement = selected;
                this.Close();
            };

            var templates = new List<FunctionTemplate>();
            if (triggerElementType == TriggerElementType.Event)
            {
                templates = TriggerData.LoadAllEvents();
            }
            else if (triggerElementType == TriggerElementType.Condition)
            {
                templates = TriggerData.LoadAllConditions(_project);
            }
            else if (triggerElementType == TriggerElementType.Action)
            {
                templates = TriggerData.LoadAllActions(_project, explorerElement.ElementType);
            }

            List<Searchable> objects = new List<Searchable>();
            for (int i = 0; i < templates.Count; i++)
            {
                var template = templates[i];
                Category category = Category.Get(template.category);
                ListItemFunctionTemplate listItem = new(_project, template, category);

                objects.Add(new Searchable()
                {
                    Object = listItem,
                    Category = Locale.Translate(category.Name),
                    Words = new List<string>()
                    {
                        listItem.DisplayText.ToLower(),
                        template.value.ToLower()
                    },
                });

                if (previous != null && previous.function.value == template.value)
                {
                    defaultSelected = listItem;
                    listItem.IsSelected = true;
                }
            }

            var searchables = new Searchables(objects);
            listControl.SetSearchableList(searchables);
            listControl.checkBoxShowIcons.Visibility = Visibility.Visible;

            var categoryControl = new GenericCategoryControl(searchables);
            categoryControl.Margin = new Thickness(0, 0, 4, 0);
            grid.Children.Add(categoryControl);
            Grid.SetRow(categoryControl, 1);
            Grid.SetRowSpan(categoryControl, 3);

            // default selection
            listControl.listView.Loaded += (s, e) => Dispatcher.Invoke(() =>
            {
                listControl.listView.ScrollIntoView(defaultSelected);
            });
        }

        private void TriggerElementMenuWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (createdTriggerElement != null)
            {
                ParamTextBuilder paramTextBuilder = new ParamTextBuilder(_project);
                ExplorerElement.CurrentToRender = explorerElement;
                createdTriggerElement.DisplayText = paramTextBuilder.GenerateTreeItemText(explorerElement, createdTriggerElement);
            }
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
            EditorSettings settings = EditorSettings.Load();
            settings.triggerWindowWidth = (int)this.Width;
            settings.triggerWindowHeight = (int)this.Height;
        }

        private void Window_LocationChanged(object sender, System.EventArgs e)
        {
            EditorSettings settings = EditorSettings.Load();
            settings.triggerWindowX = (int)this.Left;
            settings.triggerWindowY = (int)this.Top;
        }
    }
}
