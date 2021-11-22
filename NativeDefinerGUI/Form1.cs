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
        }
    }
}
