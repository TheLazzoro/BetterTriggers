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
        private bool _isEnabled = true;
        private bool _hasErrors = false;
        private bool _isSelected;
        private bool _isExpanded;
        private Visibility _iconVisibility = Visibility.Visible;
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
                OnPropertyChanged();
            }
        }
        public string RenameText { get; set; }

        public byte[] IconImage
        {
            get => _icon;
            set
            {
                _icon = value;
                OnPropertyChanged();
            }
        }

        public Visibility IsIconVisible
        {
            get => _iconVisibility;
            set
            {
                _iconVisibility = value;
                OnPropertyChanged();
            }
        }

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
            get => RenameBoxVisibility == Visibility.Visible;
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

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                OnPropertyChanged();
                OnToggleEnable?.Invoke();
            }
        }
        public bool HasErrors
        {
            get => _hasErrors;
            set
            {
                _hasErrors = value;
                OnPropertyChanged();
            }
        }
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                _isExpanded = value;
                OnPropertyChanged();
            }
        }

        public bool IsChecked { get; set; } = false;
        public event Action OnToggleEnable;


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
