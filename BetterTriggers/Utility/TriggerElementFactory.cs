using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;

namespace BetterTriggers.Utility
{
    public static class TriggerElementFactory
    {
        public static ECA Create(Project project, string name)
        {
            switch (name)
            {
                case "IfThenElseMultiple":
                    return new IfThenElse(project);
                case "AndMultiple":
                    return new AndMultiple(project);
                case "OrMultiple":
                    return new OrMultiple(project);
                case "ForGroupMultiple":
                    return new ForGroupMultiple(project);
                case "ForForceMultiple":
                    return new ForForceMultiple(project);
                case "ForLoopAMultiple":
                    return new ForLoopAMultiple(project);
                case "ForLoopBMultiple":
                    return new ForLoopBMultiple(project);
                case "ForLoopVarMultiple":
                    return new ForLoopVarMultiple(project);
                case "SetVariable":
                    return new SetVariable(project);
                case "EnumDestructablesInRectAllMultiple":
                    return new EnumDestructablesInRectAllMultiple(project);
                case "EnumDestructablesInCircleBJMultiple":
                    return new EnumDestructiblesInCircleBJMultiple(project);
                case "EnumItemsInRectBJMultiple":
                    return new EnumItemsInRectBJ(project);
                case "ReturnStatement":
                    return new ReturnStatement(project);
                default:
                    return new ECA(project, name);
            }
        }
    }
}
