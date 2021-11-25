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
    /// Interaction logic for ParameterFunctionControl.xaml
    /// </summary>
    public partial class ParameterConstantControl : UserControl
    {
        public ParameterConstantControl(string returnType)
        {
            InitializeComponent();

            List<DataAccess.Natives.Constant> constants = DataAccess.LoadData.LoadAllConstants(@"C:\Users\Lasse Dam\Desktop\JSON\constants.json");

            for (int i = 0; i < constants.Count; i++)
            {
                if(constants[i].returnType.type == returnType)
                {
                    ListViewItem item = new ListViewItem();
                    item.Content = constants[i].name;
                    item.Tag = constants[i];

                    listViewConstant.Items.Add(item);
                }
            }
        }
    }
}
