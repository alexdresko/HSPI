namespace GutenTag
{
    public class TagProperty
    {
        private string value;

        public TagProperty()
        {
            Modifier = new TagPropertyModifier();
        }

        public TagProperty(string value) : this()
        {
            Set(value);
        }

        protected TagPropertyModifier Modifier { get; }

        protected virtual string CoreSet(string value)
        {
            return this.value = value;
        }

        public virtual string Set(string value)
        {
            var current = Get();
            value = Modifier.ModifyReceived(value, current);
            value = Modifier.ModifyBeforeSet(value, current);
            return CoreSet(value);
        }

        public virtual string Get()
        {
            return value;
        }

        public virtual string CoreAdd(string value)
        {
            return CoreSet(value);
        }

        public virtual string Add(string value)
        {
            var current = Get();
            value = Modifier.ModifyReceived(value, current);
            value = Modifier.ModifyBeforeAdd(value, current);
            return CoreAdd(value);
        }

        public virtual string Remove()
        {
            return value = null;
        }

        public virtual string Remove(string value)
        {
            return Remove();
        }

        public void AddModifier(TagPropertyModifier modifier)
        {
            Modifier.Add(modifier);
        }
    }
}