namespace GutenTag
{
    internal class ListTagProperty : TagProperty
    {
        private readonly SpaceSeparatedSet value = new SpaceSeparatedSet();

        public override string Set(string value)
        {
            Remove();
            return Add(value);
        }

        public override string Get()
        {
            return value.ToString();
        }

        public override string CoreAdd(string value)
        {
            this.value.Add(value);
            return Get();
        }

        public override string Add(string value)
        {
            var current = Get();
            value = Modifier.ModifyReceived(value, current);
            value = Modifier.ModifyBeforeAdd(value, current);
            return CoreAdd(value);
        }

        public override string Remove()
        {
            value.Clear();
            return Get();
        }

        public override string Remove(string value)
        {
            this.value.Remove(value);
            return Get();
        }
    }
}