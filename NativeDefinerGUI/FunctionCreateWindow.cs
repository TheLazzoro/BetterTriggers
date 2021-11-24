using DataAccess.Containers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NativeDefinerGUI
{
    public partial class FunctionCreateWindow : Form
    {
        public ListViewItem returns;
        
        public FunctionCreateWindow()
        {
            InitializeComponent();

            var types = ContainerTypes.GetAllTypes();
            for(int i = 0; i < ContainerTypes.Size(); i++)
            {
                ListViewItem item = new ListViewItem();
                item.Text = types[i].name;
                item.Tag = types[i];

                listViewTypes.Items.Add(item);
            }
        }

        private void btnAddParam_Click(object sender, EventArgs e)
        {
            if(listViewTypes.SelectedItems.Count > 0)
            {
                var selectedItem = listViewTypes.SelectedItems[0];
                var itemToAdd = new ListViewItem();
                itemToAdd.Text = selectedItem.Text;
                itemToAdd.Tag = selectedItem.Tag;
                listViewParameters.Items.Add(itemToAdd);
            }
        }

        private void btnRemoveParam_Click(object sender, EventArgs e)
        {
            if (listViewParameters.SelectedItems.Count > 0)
            {
                var item = listViewParameters.SelectedItems[0];
                listViewParameters.Items.Remove(item);
            }
        }

        private void btnAddReturnType_Click(object sender, EventArgs e)
        {
            var selectedItem = listViewTypes.SelectedItems[0];
            lblReturnType.Text = selectedItem.Text;
            lblReturnType.Tag = selectedItem.Tag;

            btnCreateParam.Enabled = true;
        }

        private void btnCreateParam_Click(object sender, EventArgs e)
        {
            List<DataAccess.Natives.Parameter> parameters = new List<DataAccess.Natives.Parameter>();
            for (int i = 0; i < listViewParameters.Items.Count; i++)
            {
                DataAccess.Natives.Parameter parameter = (DataAccess.Natives.Parameter)listViewParameters.Items[i].Tag;
                parameters.Add(parameter);
            }

            DataAccess.Natives.Function _function = new DataAccess.Natives.Function(textBoxIdentifier.Text, parameters, (DataAccess.Natives.Type) lblReturnType.Tag, textBoxName.Text, richTextParamText.Text, richTextDescription.Text);

            Dispose();
        }
    }
}
