using BetterTriggers.Models.SaveableData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.Utility
{
    public static class FunctionFactory
    {
        private static readonly string returnType = "nothing";

        public static Function Create(string identifier)
        {
            switch (identifier)
            {
                case "IfThenElseMultiple":
                    return new IfThenElse() { identifier = identifier, returnType = returnType };
                case "AndMultiple":
                    return new AndMultiple() { identifier = identifier, returnType = returnType };
                case "OrMultiple":
                    return new OrMultiple() { identifier = identifier, returnType = returnType };
                case "ForGroupMultiple":
                    return new ForGroupMultiple() { identifier = identifier, returnType = returnType };
                case "ForForceMultiple":
                    return new ForForceMultiple() { identifier = identifier, returnType = returnType };
                case "ForLoopAMultiple":
                    return new ForLoopAMultiple() { identifier = identifier, returnType = returnType };
                case "ForLoopBMultiple":
                    return new ForLoopBMultiple() { identifier = identifier, returnType = returnType };
                case "ForLoopVarMultiple":
                    return new ForLoopVarMultiple() { identifier = identifier, returnType = returnType };
                case "SetVariable":
                    return new SetVariable() { identifier = identifier, returnType = returnType };
                case "EnumDestructablesInRectAllMultiple":
                    return new EnumDestructablesInRectAllMultiple() { identifier = identifier, returnType = returnType };
                case "EnumDestructiblesInCircleBJMultiple":
                    return new EnumDestructiblesInCircleBJMultiple() { identifier = identifier, returnType = returnType };
                case "EnumItemsInRectBJ":
                    return new EnumItemsInRectBJ() { identifier = identifier, returnType = returnType };
                default:
                    return new Function() { identifier = identifier, returnType = returnType };
            }
        }
    }
}
