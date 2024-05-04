using BetterTriggers.WorldEdit;
using BetterTriggers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Controls;

namespace BetterTriggers.Models.EditorData
{
    public class War3Type
    {
        public string DisplayName { get; }
        public string Type { get; }


        private static Dictionary<string, War3Type> War3TypesDictionary = new();
        private static ObservableCollection<War3Type> _war3Types;
        public static ObservableCollection<War3Type> War3Types
        {
            get
            {
                Init();
                return _war3Types;
            }
        }

        public War3Type(string type, string displayName)
        {
            this.Type = type;
            this.DisplayName = displayName;
            War3TypesDictionary.Add(type, this);
        }

        public static War3Type Get(string type)
        {
            Init();
            War3TypesDictionary.TryGetValue(type, out War3Type result);
            if (result == null)
            {
                result  = new War3Type(type, type); // Blizzard bug. some previously selected variables are no longer selectable.
            }

            return result;
        }

        private static void Init()
        {
            if (_war3Types == null)
            {
                _war3Types = new();
                var types = Types.GetGlobalTypes();
                for (int i = 0; i < types.Count; i++)
                {
                    var type = types[i];
                    string displayName = Locale.Translate(types[i].DisplayName);
                    var war3Type = new War3Type(type.Key, displayName);
                    _war3Types.Add(war3Type);
                }
            }
        }
    }
}
