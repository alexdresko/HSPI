namespace GutenTag
{
    internal class BooleanTagProperty : TagProperty
    {
        private string EmptyOrNull(string value)
        {
            return value == null ? null : string.Empty;
        }

        public override string Set(string value)
        {
            return base.Set(EmptyOrNull(value));
        }

        public override string Add(string value)
        {
            return Set(value ?? Get());
        }
    }
}