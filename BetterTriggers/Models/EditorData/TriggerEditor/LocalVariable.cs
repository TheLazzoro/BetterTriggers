using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.Models.EditorData
{
    public class LocalVariable : TriggerElement
    {
        public Variable variable = new Variable();

        public LocalVariable()
        {
            variable._isLocal = true;
            variable.IsArray = false; // forces locals to be non-arrays
        }

        public LocalVariable(Trigger trig)
        {
            variable._isLocal = true;
            variable.IsArray = false; // forces locals to be non-arrays
        }

        public override LocalVariable Clone()
        {
            LocalVariable clone = new LocalVariable();
            clone.variable = variable.Clone();
            clone.variable._isLocal = true;

            return clone;
        }
    }
}
