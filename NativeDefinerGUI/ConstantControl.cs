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
    public partial class ConstantControl : UserControl
    {
        public ConstantControl()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;

            string filePlainText = File.ReadAllText(@"C:\Users\Lasse Dam\Desktop\JSON\constants.json");
            List<DataAccess.Natives.Constant> list = JsonConvert.DeserializeObject<List<DataAccess.Natives.Constant>>(filePlainText);
            ContainerConstants.SetList(list);

            UpdateListView();
        }


        private void btnCreateConstant_Click(object sender, EventArgs e)
        {
            CreateConstant();
        }

        private void textBoxIdentifier_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                CreateConstant();
                e.SuppressKeyPress = true;
            }
        }

        private void textBoxName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                CreateConstant();
                e.SuppressKeyPress = true;
            }
        }

        private void btnAddReturnType_Click(object sender, EventArgs e)
        {
            var selectedItem = listViewTypes.SelectedItems[0];
            lblReturnType.Text = selectedItem.Text;
            lblReturnType.Tag = selectedItem.Tag;

            btnCreateConstant.Enabled = true;
        }

        private void CreateConstant()
        {
            if (textBoxIdentifier.Text != "" && textBoxName.Text != "")
            {
                DataAccess.Natives.Constant constant = new DataAccess.Natives.Constant()
                {
                    identifier = textBoxIdentifier.Text,
                    returnType = (DataAccess.Natives.Type)lblReturnType.Tag,
                    name = textBoxName.Text,
                };

                ContainerConstants.AddConstant(constant);
                UpdateListView();

                // save json
                string jsonOutput = JsonConvert.SerializeObject(ContainerConstants.GetAllTypes());
                File.WriteAllText(@"C:\Users\Lasse Dam\Desktop\JSON\constants.json", jsonOutput);
            }
        }

        private void UpdateListView()
        {
            listViewConstants.Items.Clear();
            var list = ContainerConstants.GetAllTypes();
            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = list[i].name;
                    item.Tag = list[i];
                    listViewConstants.Items.Add(item);
                }
            }

            listViewTypes.Items.Clear();
            var listTypes = ContainerTypes.GetAllTypes();
            if (listTypes != null)
            {
                for (int i = 0; i < listTypes.Count; i++)
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = listTypes[i].name;
                    item.Tag = listTypes[i];
                    listViewTypes.Items.Add(item);
                }
            }

            textBoxIdentifier.Text = "";
            textBoxName.Text = "";
            lblReturnType.Text = "";
        }

    }
}
