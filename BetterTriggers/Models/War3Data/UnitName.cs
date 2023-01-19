using System;
using System.Collections.Generic;
using System.Text;

namespace BetterTriggers.Models.War3Data
{
    public class UnitName
    {
        public string Name {
            get { return _name; }
            set { if (value != null) _name = value; else _name = string.Empty; }
        }

        public string Propernames
        {
            get { return _propernames; }
            set { if (value != null) _propernames = value; else _propernames = string.Empty; }
        }
        public string EditorSuffix
        {
            get { return _editorSuffix; }
            set { if (value != null) _editorSuffix = value; else _editorSuffix = string.Empty; }
        }

        private string _name;
        private string _propernames;
        private string _editorSuffix;

        internal UnitName Clone()
        {
            return new UnitName()
            {
                Name = new string(Name),
                Propernames = new string(Propernames),
                EditorSuffix = new string(EditorSuffix),
            };
        }
    }
}
