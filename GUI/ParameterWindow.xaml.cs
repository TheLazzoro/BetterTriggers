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
using System.Windows.Shapes;

namespace GUI
{
    /// <summary>
    /// Interaction logic for ParameterWindow.xaml
    /// </summary>
    public partial class ParameterWindow : Window
    {
        public ParameterWindow(string returnType)
        {
            InitializeComponent();

            var functionControl = new ParameterFunctionControl(returnType);
            grid.Children.Add(functionControl);
            Grid.SetRow(functionControl, 1);
            functionControl.Visibility = Visibility.Hidden;

            var constantControl = new ParameterConstantControl(returnType);
            grid.Children.Add(constantControl);
            Grid.SetRow(constantControl, 1);
            constantControl.Visibility = Visibility.Visible;
        }
    }
}
