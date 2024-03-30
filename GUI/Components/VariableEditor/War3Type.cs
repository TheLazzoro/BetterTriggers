using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace GUI.Components.VariableEditor
{
    public class War3Type
    {
        public string DisplayName { get; }
        public string Type { get; }

        public War3Type(string type, string displayName)
        {
            this.Type = type;
            this.DisplayName = displayName;
        }
    }
}
