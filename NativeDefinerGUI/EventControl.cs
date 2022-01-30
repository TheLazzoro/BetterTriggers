using Model.Containers;
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
    public partial class EventControl : UserControl
    {
        public EventControl()
        {
            InitializeComponent();

            this.Dock = DockStyle.Fill;

            string filePlainText = File.ReadAllText(@"C:\Users\Lasse Dam\Desktop\JSON\events.json");
            List<Model.Templates.FunctionTemplate> list = JsonConvert.DeserializeObject<List<Model.Templates.FunctionTemplate>>(filePlainText);
            ContainerEvents.SetList(list);

            RefreshEventList();
        }

        private void btnCreateEvent_Click(object sender, EventArgs e)
        {
            var window = new EventCreateWindow();
            window.ShowDialog();

            // save json
            string jsonOutput = JsonConvert.SerializeObject(ContainerEvents.GetAllTypes());
            File.WriteAllText(@"C:\Users\Lasse Dam\Desktop\JSON\events.json", jsonOutput);

            RefreshEventList();
        }

        private void RefreshEventList()
        {
            listViewEvents.Items.Clear();
            var list = ContainerEvents.GetAllTypes();
            if (list != null)
            {
                for( int i = 0; i < list.Count; i++)
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = list[i].name;
                    item.Tag = list[i];
                    listViewEvents.Items.Add(item);
                }
            }
        }
    }
}
