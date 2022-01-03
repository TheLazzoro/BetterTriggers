using GUI.Components.TriggerExplorer;
using Model.Data;
using Model.War3Project;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GUI.Containers
{
    public class ContainerTriggerExplorer
    {
        public static TriggerExplorer triggerExplorer;
        public static event EventHandler ItemSelectionChanged;

        public static void CreateNewTriggerExplorer(Grid mainGrid, War3Project war3Project)
        {
            if (triggerExplorer != null)
            {
                var parent = (Grid)triggerExplorer.Parent;
                parent.Children.Remove(triggerExplorer);
            }
            triggerExplorer = new TriggerExplorer(war3Project.Root);
            triggerExplorer.Margin = new Thickness(0, 0, 4, 0);
            triggerExplorer.HorizontalAlignment = HorizontalAlignment.Stretch;
            triggerExplorer.Width = Double.NaN;
            //currentTriggerExplorer.BorderThickness = new Thickness(0, 0, 0, 0);
            triggerExplorer.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 32, 32, 32));
            mainGrid.Children.Add(triggerExplorer);
            Grid.SetRow(triggerExplorer, 3);
            Grid.SetRowSpan(triggerExplorer, 4);
            Grid.SetColumn(triggerExplorer, 0);
            triggerExplorer.treeViewTriggerExplorer.SelectedItemChanged += OnItemSelectionChanged;

            triggerExplorer.CreateRootItem(war3Project.Root, EnumCategory.Map);
        }

        protected static void OnItemSelectionChanged(object sender, EventArgs e)
        {
            EventHandler handler = ItemSelectionChanged;
            handler?.Invoke(sender, e);
        }
    }
}
