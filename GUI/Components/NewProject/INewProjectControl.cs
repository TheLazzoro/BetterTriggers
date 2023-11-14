using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.Components.NewProject
{
    public interface INewProjectControl
    {
        public string ProjectLocation { get; set; }
        public event Action OnOpenProject;
    }
}
