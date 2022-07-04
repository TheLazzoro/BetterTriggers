using BetterTriggers.Models.SaveableData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.Utility
{
    public static class TriggerElementFactory
    {
        public static TriggerElement Create(string identifier)
        {
            switch (identifier)
            {
                case "IfThenElseMultiple":
                    return new IfThenElse();
                case "AndMultiple":
                    return new AndMultiple();
                case "OrMultiple":
                    return new OrMultiple();
                case "ForGroupMultiple":
                    return new ForGroupMultiple();
                case "ForForceMultiple":
                    return new ForForceMultiple();
                case "ForLoopAMultiple":
                    return new ForLoopAMultiple();
                case "ForLoopBMultiple":
                    return new ForLoopBMultiple();
                case "ForLoopVarMultiple":
                    return new ForLoopVarMultiple();
                case "SetVariable":
                    return new SetVariable();
                case "EnumDestructablesInRectAllMultiple":
                    return new EnumDestructablesInRectAllMultiple();
                case "EnumDestructiblesInCircleBJMultiple":
                    return new EnumDestructiblesInCircleBJMultiple();
                case "EnumItemsInRectBJMultiple":
                    return new EnumItemsInRectBJ();
                default:
                    return new TriggerElement(identifier);
            }
        }
    }
}
