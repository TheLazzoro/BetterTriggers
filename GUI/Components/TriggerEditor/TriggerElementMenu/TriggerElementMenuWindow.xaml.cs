using BetterTriggers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.Models.Templates;
using BetterTriggers.Utility;
using BetterTriggers.WorldEdit;
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

        private TriggerElementMenuViewModel _viewModel;

        public TriggerElementMenuWindow(TriggerElementType triggerElementType, ECA previous = null)
        {
            InitializeComponent();

            _viewModel = new TriggerElementMenuViewModel(triggerElementType, previous);
            DataContext = _viewModel;

            this.Owner = MainWindow.GetMainWindow();

            EditorSettings settings = EditorSettings.Load();
            this.Width = settings.triggerWindowWidth;
            this.Height = settings.triggerWindowHeight;
            this.Left = settings.triggerWindowX;
            this.Top = settings.triggerWindowY;

            this.triggerElementType = triggerElementType;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var selected = _viewModel.Selected;

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
            listControl.ShowIconsChanged += ListControl_ShowIconsChanged;


            Init();
        }

        private void Init()
        {
            var searchables = _viewModel.Searchables;
            listControl.SetSearchableList(searchables);

            var selectedListItem = listControl.listView.ItemContainerGenerator.ContainerFromItem(_viewModel.Selected) as ListViewItem;
            listControl.listView.ScrollIntoView(selectedListItem);
            selectedListItem.Focus();
            listControl.checkBoxShowIcons.Visibility = Visibility.Visible;

            var categoryControl = new GenericCategoryControl(searchables);
            categoryControl.Margin = new Thickness(0, 0, 4, 0);
            grid.Children.Add(categoryControl);
            Grid.SetRow(categoryControl, 1);
            Grid.SetRowSpan(categoryControl, 3);
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            createdTriggerElement = _viewModel.Selected;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void ListControl_ShowIconsChanged()
        {
            Init();
        }


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            var selected = _viewModel.Selected;
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
