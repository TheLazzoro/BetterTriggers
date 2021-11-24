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
    public partial class FunctionControl : UserControl
    {
        public FunctionControl()
        {
            InitializeComponent();

            this.Dock = DockStyle.Fill;

            string filePlainText = File.ReadAllText(@"C:\Users\Lasse Dam\Desktop\JSON\params.json");
            List<DataAccess.Natives.Parameter> list = JsonConvert.DeserializeObject<List<DataAccess.Natives.Parameter>>(filePlainText);
            ContainerParameter.SetList(list);

            RefreshEventList();
        }

        private void btnCreateEvent_Click(object sender, EventArgs e)
        {
            var window = new FunctionCreateWindow();
            window.ShowDialog();

            // save json
            string jsonOutput = JsonConvert.SerializeObject(ContainerParameter.GetAllTypes());
            File.WriteAllText(@"C:\Users\Lasse Dam\Desktop\JSON\params.json", jsonOutput);

            RefreshEventList();
        }

        private void RefreshEventList()
        {
            listViewEvents.Items.Clear();
            var list = ContainerParameter.GetAllTypes();
            if (list != null)
            {
                for( int i = 0; i < list.Count; i++)
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = list[i].identifier;
                    item.Tag = list[i];
                    listViewEvents.Items.Add(item);
                }
            }
        }
    }
}
