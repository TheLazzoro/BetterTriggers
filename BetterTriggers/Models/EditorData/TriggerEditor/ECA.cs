using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.Models.EditorData
{
    /// <summary>
    /// Event, Condition, Action.
    /// </summary>
    public class ECA : TriggerElement
    {
        public Function function = new Function();

        public ECA() { }

        public ECA(string value)
        {
            function.value = value;
        }

        public override ECA Clone()
        {
            ECA clone = new ECA();
            clone.DisplayText = new string(DisplayText);
            clone.IsEnabled = IsEnabled;
            clone.function = function.Clone();
            clone.ElementType = ElementType;
            clone.IconImage = new byte[IconImage.Length];
            IconImage.CopyTo(clone.IconImage, 0);

            return clone;
        }
    }
}
