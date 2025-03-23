using BetterTriggers;
using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.Utility;
using GUI.Components;
using GUI.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GUI.Components.VariableList
{
    public partial class VariableListWindow : Window
    {
        private VariableControl control;
        private List<VariableControl> controls = new List<VariableControl>(); // for GC

        public VariableListWindow()
        {
            InitializeComponent();

            EditorSettings settings = EditorSettings.Load();
            this.Left = settings.variableListWindowX;
            this.Top = settings.variableListWindowY;
            this.Width = settings.variableListWindowWidth;
            this.Height = settings.variableListWindowHeight;
            this.ResetPositionWhenOutOfScreenBounds();

            List<ListViewItem> list = new List<ListViewItem>();
            List<Searchable> objects = new List<Searchable>();
            
            var variables = Project.CurrentProject.Variables.GetGlobals();
            variables.ForEach(v =>
            {
                var item = new ListViewItem { Content = v.variable.Name + v.variable.SuffixText, Tag = v };
                objects.Add(new Searchable()
                {
                    Object = item,
                    Words = new List<string>()
                    {
                        ((string)item.Content).ToLower()
                    },
                });
            });
            var searchables = new Searchables(objects);
            listControl.SetSearchableList(searchables);
            listControl.listView.SelectionChanged += ListView_SelectionChanged;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = listControl.listView.SelectedItem as ListViewItem;
            if (selected == null)
                return;

            ExplorerElement ex = (ExplorerElement)selected.Tag;
            VariableControl control = new VariableControl(ex, ex.variable);
            if(this.control != null)
                grid.Children.Remove(this.control);
            this.control = control;
            grid.Children.Add(control);
            Grid.SetRow(control, 0);
            Grid.SetColumn(control, 1);
            Grid.SetRowSpan(control, 2);

            //GC
            controls.Add(control);
            if (controls.Count > 200)
            {
                controls.Clear();
                GC.Collect();
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            EditorSettings settings = EditorSettings.Load();
            settings.variableListWindowX = (int)this.Left;
            settings.variableListWindowY = (int)this.Top;
            settings.variableListWindowWidth = (int)this.Width;
            settings.variableListWindowHeight = (int)this.Height;
        }
    }
}
