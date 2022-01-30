using Model.Containers;
using Model.Enums;
using Model.SavableTriggerData;
using Model.Templates;
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

            /*
            var types = ContainerTypes.GetAllTypes();
            for(int i = 0; i < ContainerTypes.Size(); i++)
            {
                var parameter = new Parameter()
                {
                    identifier = types[i].type,
                    returnType = types[i],
                    name = types[i].name
                };

                ListViewItem item = new ListViewItem();
                item.Text = types[i].name;
                item.Tag = parameter;

                listViewTypes.Items.Add(item);
            }
            */

            // Populate list of categories
            var enumCategoryValues = Enum.GetValues(typeof(Category));
            for(int i = 0; i < enumCategoryValues.Length; i++)
            {
                ListViewItem item = new ListViewItem();
                item.Text = Enum.GetName(typeof(Category), enumCategoryValues.GetValue(i));
                item.Tag = enumCategoryValues.GetValue(i);
                listViewCategory.Items.Add(item);
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
            List<Parameter> parameters = new List<Parameter>();
            for(int i = 0; i < listViewParameters.Items.Count; i++)
            {
                Parameter parameter = (Parameter) listViewParameters.Items[i].Tag;
                parameters.Add(parameter);
            }

            try
            {
                var selectedCategory = listViewCategory.SelectedItems[0];
                var category = (Category)selectedCategory.Tag;

                FunctionTemplate _event = new FunctionTemplate()
                {
                    identifier = textBoxIdentifier.Text,
                    parameters = parameters,
                    paramText = richTextEventText.Text,
                    name = textBoxName.Text,
                    description = richTextDescription.Text,
                    category = category
                };

                ContainerEvents.AddEvent(_event);

                Dispose();
            }
            catch (Exception)
            {
                MessageBox.Show("Please select a category.");
            }
        }
    }
}
