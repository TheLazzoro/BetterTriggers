using Model.Data;
using GUI.Components.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace GUI.Components.TriggerEditor
{
    public class TriggerAction : TriggerElement
    {
        private Model.Natives.Function action;
        
        public TriggerAction(Model.Natives.Function action)
        {
            this.action = action;
            this.parameters = action.parameters;
            this.paramText = action.paramText;
            this.descriptionTextBlock.Text = action.description;
            this.category = action.category;

            TreeViewManipulator.SetTreeViewItemAppearance(this, "placeholder", action.category);
            this.FormatParameterText(paramTextBlock, this.parameters);
        }

        public void SetCategory(EnumCategory category)
        {
            this.category = action.category;
        }
    }
}
