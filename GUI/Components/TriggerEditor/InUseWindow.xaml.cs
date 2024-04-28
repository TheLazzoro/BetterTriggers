using BetterTriggers.Models.EditorData;
using System;
using System.Collections.Generic;
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

namespace GUI.Components.TriggerEditor
{
    public partial class InUseWindow : Window
    {
        public bool OK;

        public InUseWindow(List<LocalVariable> localVariables)
        {
            this.Owner = MainWindow.GetMainWindow();
            InitializeComponent();

            localVariables.ForEach(v => listView.Items.Add(v.variable.Name));
        }

        public InUseWindow(List<ParameterDefinition> paramDefs)
        {
            this.Owner = MainWindow.GetMainWindow();
            InitializeComponent();

            paramDefs.ForEach(v => listView.Items.Add(v.Name));
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            OK = true;
            this.Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
        }
    }
}
