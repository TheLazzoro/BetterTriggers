namespace BetterTriggers.Models.EditorData
{
    public class ReturnStatement : ECA
    {
        public ReturnStatement()
        {
            function.value = "ReturnStatement";
        }

        public override ReturnStatement Clone()
        {
            var clone = new ReturnStatement();
            clone.DisplayText = new string(DisplayText);
            clone.function = this.function.Clone();
            clone.ElementType = ElementType;
            clone.IconImage = new byte[IconImage.Length];
            IconImage.CopyTo(clone.IconImage, 0);

            return clone;
        }
    }
}
