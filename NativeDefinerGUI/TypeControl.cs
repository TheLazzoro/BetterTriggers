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
    public partial class TypeControl : UserControl
    {
        public TypeControl()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;

            string filePlainText = File.ReadAllText(@"C:\Users\Lasse Dam\Desktop\JSON\types.txt");
            //List<DataAccess.Natives.Type> list = JsonConvert.DeserializeObject<List<DataAccess.Natives.Type>>(filePlainText);
            //ContainerTypes.SetList(list);

            UpdateListView();
        }

        private void btnCreateType_Click(object sender, EventArgs e)
        {
            CreateType();
        }

        private void textBoxIdentifier_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                CreateType();
                e.SuppressKeyPress = true;
            }
        }

        private void textBoxName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                CreateType();
                e.SuppressKeyPress = true;
            }
        }

        private void CreateType()
        {
            /*
            if (textBoxIdentifier.Text != "" && textBoxName.Text != "")
            {
                DataAccess.Natives.Type type = new DataAccess.Natives.Type(textBoxIdentifier.Text, textBoxName.Text);
                ContainerTypes.AddType(type);
                UpdateListView();

                // save json
                string jsonOutput = JsonConvert.SerializeObject(ContainerTypes.GetAllTypes());
                File.WriteAllText(@"C:\Users\Lasse Dam\Desktop\JSON\types.json", jsonOutput);
            }
            */
        }

        private void UpdateListView()
        {
            /*listViewTypes.Items.Clear();
            var list = ContainerTypes.GetAllTypes();
            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = list[i].name;
                    item.Tag = list[i];
                    listViewTypes.Items.Add(item);
                }
            }

            textBoxIdentifier.Text = "";
            textBoxName.Text = "";*/
        }
    }
}
