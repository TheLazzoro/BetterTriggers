using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.Models.EditorData
{
    public abstract class TreeNodeBase
    {
        private string _displayText;
        private string _category;
        private byte[] _icon;
        private bool _isRenaming;

        public string DisplayText
        {
            get
            {
                return _displayText;
            }
            set
            {
                _displayText = value;
                RenameText = value;
            }
        }
        public string RenameText { get; set; }

        public byte[] IconImage
        {
            get
            {
                return _icon;
            }
        }

        public bool IsIconVisible { get; set; } = true;

        public Visibility RenameBoxVisibility { get; set; } = Visibility.Hidden;

        public bool IsRenaming
        {
            get { return _isRenaming; }
            set { _isRenaming = value; }
        }

        public Visibility CheckBoxVisibility { get; set; } = Visibility.Hidden;

        public bool IsChecked { get; set; } = false;
        public bool IsEnabled { get; set; } = true;


        public void SetCategory(string categoryStr)
        {
            this._category = categoryStr;
            var category = Category.Get(_category);
            _icon = category.Icon;
        }
    }
}
