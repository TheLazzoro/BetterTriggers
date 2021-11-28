using GUI.Components.TriggerEditor;
using GUI.Components.Utility;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GUI
{
    /// <summary>
    /// Interaction logic for TriggerControl.xaml
    /// </summary>
    public partial class TriggerControl : UserControl
    {
        CategoryEvent categoryEvent;
        CategoryCondition categoryCondition;
        CategoryAction categoryAction;

        TextBlock currentParameterBlock;

        public TriggerControl()
        {
            InitializeComponent();

            categoryEvent = new CategoryEvent();
            categoryCondition = new CategoryCondition();
            categoryAction = new CategoryAction();

            treeViewTriggers.Items.Add(categoryEvent);
            treeViewTriggers.Items.Add(categoryCondition);
            treeViewTriggers.Items.Add(categoryAction);
        }

        public void CreateEvent()
        {
            var eventMenu = new EventMenuWindow();
            eventMenu.ShowDialog();
            DataAccess.Natives.Event _event = eventMenu.selectedEvent;

            if(_event != null)
            {
                TriggerEvent item = new TriggerEvent(_event);
                categoryEvent.Items.Add(item);

                categoryEvent.IsExpanded = true;
            }

        }

        private void treeViewTriggers_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var item = treeViewTriggers.SelectedItem as TriggerEvent;
            if (item != null) {
                var textBlockParameters = item.paramTextBlock;
                
                if (currentParameterBlock != null && currentParameterBlock.Parent != null)
                    grid.Children.Remove(currentParameterBlock); // remove current active parameter text block so the new one can be added.

                grid.Children.Add(textBlockParameters);
                Grid.SetRow(textBlockParameters, 4);
                textBlockParameters.Margin = new Thickness(0, 0, 5, 0);

                currentParameterBlock = textBlockParameters;
            }
        }
    }
}
