using DataAccess.Containers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NativeDefinerGUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            var typeControl = new TypeControl();
            tabControl.TabPages[0].Controls.Add(typeControl);

            var eventControl = new EventControl();
            tabControl.TabPages[1].Controls.Add(eventControl);

            var parameterControl = new FunctionControl();
            tabControl.TabPages[2].Controls.Add(parameterControl);

            var constantControl = new ConstantControl();
            tabControl.TabPages[3].Controls.Add(constantControl);
        }
    }
}
