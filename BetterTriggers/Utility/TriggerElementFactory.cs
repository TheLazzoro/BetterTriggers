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
        public static ECA_Saveable Create(string name)
        {
            switch (name)
            {
                case "IfThenElseMultiple":
                    return new IfThenElse_Saveable();
                case "AndMultiple":
                    return new AndMultiple_Saveable();
                case "OrMultiple":
                    return new OrMultiple_Saveable();
                case "ForGroupMultiple":
                    return new ForGroupMultiple_Saveable();
                case "ForForceMultiple":
                    return new ForForceMultiple_Saveable();
                case "ForLoopAMultiple":
                    return new ForLoopAMultiple_Saveable();
                case "ForLoopBMultiple":
                    return new ForLoopBMultiple_Saveable();
                case "ForLoopVarMultiple":
                    return new ForLoopVarMultiple_Saveable();
                case "SetVariable":
                    return new SetVariable_Saveable();
                case "EnumDestructablesInRectAllMultiple":
                    return new EnumDestructablesInRectAllMultiple_Saveable();
                case "EnumDestructablesInCircleBJMultiple":
                    return new EnumDestructiblesInCircleBJMultiple_Saveable();
                case "EnumItemsInRectBJMultiple":
                    return new EnumItemsInRectBJ_Saveable();
                default:
                    return new ECA_Saveable(name);
            }
        }
    }
}
