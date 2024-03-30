namespace BetterTriggers.Models.SaveableData
{
    public class SetVariable_Saveable : ECA_Saveable
    {
        public readonly int ElementType = 9; // DO NOT CHANGE

        public SetVariable_Saveable()
        {
            function.value = "SetVariable";
        }
    }
}
