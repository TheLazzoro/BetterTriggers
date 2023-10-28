using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public class Variable : IReferable
    {
        public int Id;

        public string Name { get { return _name; } set { _name = value; OnValuesChanged(); } }
        public string Type { get { return _type; } set { _type = value; OnValuesChanged(); } }
        public bool IsArray { get { return _isArray; } set { _isArray = value; OnValuesChanged(); } }
        public bool IsTwoDimensions { get { return _isTwoDimensions; } set { _isTwoDimensions = value; OnValuesChanged(); } }
        public int[] ArraySize = new int[2]; // TODO: how to detect set values in array indexes?
        public Parameter InitialValue { get { return _initialValue; } set { _initialValue = value; OnValuesChanged(); } }

        [JsonIgnore]
        public bool SuppressChangedEvent = false;
        [JsonIgnore]
        private string _name;
        [JsonIgnore]
        private string _type;
        [JsonIgnore]
        private bool _isArray;
        [JsonIgnore]
        private bool _isTwoDimensions;
        [JsonIgnore]
        private Parameter _initialValue;
        [JsonIgnore]
        public bool _isLocal { get; internal set; }

        public event EventHandler ValuesChanged;

        protected virtual void OnValuesChanged()
        {
            if (SuppressChangedEvent)
                return;
            if (ValuesChanged != null) ValuesChanged(this, EventArgs.Empty);
        }

        public Variable Clone()
        {
            Variable cloned = new Variable();
            cloned.Id = Id;
            cloned.Name = new string(Name);
            cloned.Type = new string(Type);
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
    }
}