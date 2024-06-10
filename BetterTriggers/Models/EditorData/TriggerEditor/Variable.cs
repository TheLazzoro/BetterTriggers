using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.Models.EditorData
{
    public class Variable : IReferable, INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }
        public War3Type War3Type
        {
            get { return _war3Type; }
            set
            {
                _war3Type = value;
                OnPropertyChanged();
            }
        }
        public bool IsArray
        {
            get { return _isArray; }
            set
            {
                _isArray = value;
                OnPropertyChanged();
            }
        }
        public bool IsTwoDimensions
        {
            get { return _isTwoDimensions; }
            set
            {
                _isTwoDimensions = value;
                OnPropertyChanged();
            }
        }
        public int[] ArraySize = new int[2]; // TODO: how to detect set values in array indexes?
        public Parameter InitialValue
        {
            get { return _initialValue; }
            set
            {
                _initialValue = value;
                OnPropertyChanged();
            }
        }

        public bool SuppressChangedEvent = false;
        private string _name;
        private War3Type _war3Type;
        private bool _isArray;
        private bool _isTwoDimensions;
        private Parameter _initialValue;

        public event PropertyChangedEventHandler? PropertyChanged;

        public bool _isLocal { get; internal set; }

        public Variable Clone()
        {
            Variable cloned = new Variable();
            cloned.Id = Id;
            cloned.Name = new string(Name);
            cloned.War3Type = War3Type;
            cloned.IsArray = IsArray;
            cloned.IsTwoDimensions = IsTwoDimensions;
            cloned.ArraySize = new int[2] { ArraySize[0], ArraySize[1] };
            cloned.InitialValue = this.InitialValue.Clone();

            return cloned;
        }

        public string GetIdentifierName()
        {
            string prefix = _isLocal ? "udl_" : "udg_";
            string name = prefix + Name.Replace(" ", "_");
            if (name.EndsWith("_"))
                name = name + "v";

            return name;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (SuppressChangedEvent)
            {
                return;
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
