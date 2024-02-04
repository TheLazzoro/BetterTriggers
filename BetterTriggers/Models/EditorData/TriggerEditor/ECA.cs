using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.Models.EditorData
{
    public class ECA : TriggerElement
    {
        public bool isEnabled { get; set; } = true;
        public Function function = new Function();

        public ECA() { }

        public ECA(string value)
        {
            function.value = value;
        }

        public override ECA Clone()
        {
            ECA clone = new ECA();
            clone.isEnabled = isEnabled;
            clone.function = function.Clone();

            return clone;
        }
    }
}
