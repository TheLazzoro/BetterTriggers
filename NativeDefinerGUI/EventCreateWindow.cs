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
    public partial class EventCreateWindow : Form
    {
        public ListViewItem returns;
        
        public EventCreateWindow()
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

        private void btnCreateEvent_Click(object sender, EventArgs e)
        {
            List<DataAccess.Natives.Type> parameters = new List<DataAccess.Natives.Type>();
            for(int i = 0; i < listViewParameters.Items.Count; i++)
            {
                DataAccess.Natives.Type type = (DataAccess.Natives.Type) listViewParameters.Items[i].Tag;
                parameters.Add(type);
            }

            DataAccess.Natives.Event _event = new DataAccess.Natives.Event(textBoxIdentifier.Text, parameters, textBoxName.Text, richTextEventText.Text, richTextDescription.Text);

            Dispose();
        }
    }
}
