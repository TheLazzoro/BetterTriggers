using BetterTriggers.JsonBaseConverter;
using BetterTriggers.Models.EditorData;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public class LocalVariable : TriggerElement
    {
        public readonly int LocalVar = 1; // DO NOT CHANGE
        public Variable variable = new Variable();

        public LocalVariable()
        {
            variable._isLocal = true;
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
