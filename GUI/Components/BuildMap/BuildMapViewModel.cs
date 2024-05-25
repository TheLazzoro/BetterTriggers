using BetterTriggers;
using GUI.Components.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.Components.BuildMap
{
    public class BuildMapViewModel : ViewModelBase
    {
        private bool _compressAdvanced;
        private bool _compress;

        public bool Compress
        {
            get => _compress;
            set
            {
                _compress = value;
                if(!value)
                    Compress_Advanced = value;

                OnPropertyChanged();
            }
        }

        public bool Compress_Advanced {
            get => _compressAdvanced;
            set
            {
                _compressAdvanced = value;
                OnPropertyChanged();
            }
        }

        public BuildMapViewModel()
        {
            EditorSettings settings = EditorSettings.Load();
            Compress = settings.Export_Compress;
            Compress_Advanced = settings.Export_Compress_Advanced;
        }
    }
}
