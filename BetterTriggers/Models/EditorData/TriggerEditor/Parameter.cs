using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.Models.EditorData
{
    public class Parameter
    {
        public string value { get; set; }

        public virtual Parameter Clone()
        {
            string value = null;
            if (this.value != null)
                value = new string(this.value);

            return new Parameter()
            {
                value = value,
            };
        }
    }
}
