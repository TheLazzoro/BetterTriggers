namespace BetterTriggers.Models.SaveableData
{
    public class SetVariable_Saveable : ECA_Saveable
    {
        public readonly int ElementType = 9; // DO NOT CHANGE

        public SetVariable_Saveable()
        {
            function.value = "SetVariable";
        }

        public override SetVariable_Saveable Clone()
        {
            SetVariable_Saveable setVariable = new SetVariable_Saveable();
            setVariable.function = this.function.Clone();

            return setVariable;
        }
    }
}
