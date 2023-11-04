using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.WorldEdit;
using GUI.Controllers;

namespace GUI.Components.TriggerEditor.ParameterControls
{
    public partial class ValueControlFrameEvents : UserControl, IValueControl
    {
        public event EventHandler SelectionChanged;
        public event EventHandler OK;

        private CheckBox[] checkBoxes = new CheckBox[16];

        public ValueControlFrameEvents()
        {
            InitializeComponent();

            this.KeyDown += ValueControl_KeyDown;
            SelectionChanged?.Invoke(this, new EventArgs());

            checkBoxes[0] = checkBoxClick;
            checkBoxes[1] = checkBoxMouseEnter;
            checkBoxes[2] = checkBoxMouseLeave;
            checkBoxes[3] = checkBoxMouseUp;
            checkBoxes[4] = checkBoxMouseDown;
            checkBoxes[5] = checkBoxMouseWheel;
            checkBoxes[6] = checkBoxMouseDoubleClick;
            checkBoxes[7] = checkBoxCheckboxChecked;
            checkBoxes[8] = checkBoxCheckboxUnChecked;
            checkBoxes[9] = checkBoxEditboxTextChanged;
            checkBoxes[10] = checkBoxEditboxEnter;
            checkBoxes[11] = checkBoxPopupMenuItemChanged;
            checkBoxes[12] = checkBoxSpriteAnimUpdate;
            checkBoxes[13] = checkBoxSliderValueChanged;
            checkBoxes[14] = checkBoxDialogCancel;
            checkBoxes[15] = checkBoxDialogAccept;
        }

        private void ValueControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            EventHandler handler = OK;
            handler?.Invoke(this, e);
        }

        public int GetElementCount()
        {
            return 1;
        }

        public void SetDefaultSelection(Parameter parameter)
        {
            for (int i = 0; i < checkBoxes.Length; i++)
            {
                if (parameter.value.Length < i) // failsafe
                    break;

                checkBoxes[i].IsChecked = parameter.value.Substring(i, 1) == "1" ? true : false;
            }
        }

        public Parameter GetSelected()
        {
            string str = string.Empty;
            for (int i = 0; i < checkBoxes.Length; i++)
            {
                str += checkBoxes[i].IsChecked == true ? "1" : "0";
            }
            Value value = new Value()
            {
                value = str,
            };

            return value;
        }
    }
}
