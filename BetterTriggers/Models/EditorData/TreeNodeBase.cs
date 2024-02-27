using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BetterTriggers.Models.EditorData
{
    public abstract class TreeNodeBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private string _displayText;
        private string _category;
        private byte[] _icon;
        private bool _isRenaming;
        private Visibility _renameBoxVisibility = Visibility.Hidden;
        private Visibility _checkBoxVisibility = Visibility.Hidden;

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
            set => _icon = value;
            get => _icon;
        }

        public bool IsIconVisible { get; set; } = true;

        public Visibility RenameBoxVisibility
        {
            get => _renameBoxVisibility;
            set
            {
                _renameBoxVisibility = value;
                OnPropertyChanged();
            }
        }

        public bool IsRenaming
        {
            get { return _isRenaming; }
            set { _isRenaming = value; }
        }

        public Visibility CheckBoxVisibility
        {
            get => _checkBoxVisibility;
            set
            {
                _checkBoxVisibility = value;
                OnPropertyChanged();
            }
        }

        public bool IsChecked { get; set; } = false;
        public bool IsEnabled { get; set; } = true;


        public void SetCategory(string categoryStr)
        {
            this._category = categoryStr;
            var category = Category.Get(_category);
            _icon = category.Icon;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
