using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.Models.EditorData
{
    public class Preset : Parameter
    {
        public override Preset Clone()
        {
            string value = null;
            if (this.value != null)
                value = new string(this.value);

            return new Preset()
            {
                value = value,
            };
        }
    }
}
