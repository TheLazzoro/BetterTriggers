using BetterTriggers.Controllers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.Utility;
using GUI.Components;
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

namespace GUI
{
    public class ListItemVariable : ListViewItem
    {
        public Variable variable { get; set; }
    }


    public partial class VariableListWindow : Window
    {
        private VariableControl control;
        private List<VariableControl> controls = new List<VariableControl>(); // for GC

        public VariableListWindow()
        {
            InitializeComponent();

            List<ListItemVariable> list = new List<ListItemVariable>();
            List<Searchable> objects = new List<Searchable>();
            
            var variables = ControllerVariable.GetVariablesAll();
            variables.ForEach(v =>
            {
                var item = new ListItemVariable { Content = v.Name, variable = v };
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
            var selected = listControl.listView.SelectedItem as ListItemVariable;
            if (selected == null)
                return;

            VariableControl control = new VariableControl(selected.variable);
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
