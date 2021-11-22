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
        public TriggerControl()
        {
            InitializeComponent();

            treeViewTriggers.Items.Add(new CategoryEvent());
            treeViewTriggers.Items.Add(new CategoryCondition());
            treeViewTriggers.Items.Add(new CategoryAction());
        }
    }
}
