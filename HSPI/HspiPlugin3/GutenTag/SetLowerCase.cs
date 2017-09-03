namespace GutenTag
{
    internal class SetLowerCase : TagPropertyModifier
    {
        protected override string BeforeSet(string value, string current)
        {
            return value == null ? null : value.ToLower();
        }

        protected override string BeforeAdd(string value, string current)
        {
            return value == null ? null : value.ToLower();
        }
    }
}