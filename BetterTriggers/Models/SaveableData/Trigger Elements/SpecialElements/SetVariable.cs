namespace BetterTriggers.Models.SaveableData
{
    public class SetVariable : ECA
    {
        public readonly int ElementType = 9; // DO NOT CHANGE

        public SetVariable()
        {
            function.value = "SetVariable";
        }

        public override SetVariable Clone()
        {
            SetVariable setVariable = new SetVariable();
            setVariable.function = this.function.Clone();

            return setVariable;
        }
    }
}
