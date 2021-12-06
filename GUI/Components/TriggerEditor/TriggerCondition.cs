using Model.Data;
using Model;
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
    public class TriggerCondition : TriggerElement
    {
        private Model.Natives.Condition condition;
        
        public TriggerCondition(Model.Natives.Condition condition)
        {
            this.condition = condition;
            this.parameters = condition.parameters;
            this.paramText = condition.paramText;
            this.category = condition.category;

            TreeViewManipulator.SetTreeViewItemAppearance(this, "placeholder", condition.category);
            this.FormatParameterText(paramTextBlock, this.parameters);
        }

        public void SetCategory(EnumCategory category)
        {
            this.category = condition.category;
        }
    }
}
