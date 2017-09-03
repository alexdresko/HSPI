using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GutenTag
{
    public class Tag : IEnumerable<Tag>, IEnumerable<KeyValuePair<string, string>>
    {
        protected readonly Dictionary<string, TagProperty> Attributes = new Dictionary<string, TagProperty>();

        protected readonly List<Tag> Children = new List<Tag>();

        private readonly TagPropertyFactory property;

        public Tag(string tagName)
        {
            Name = tagName;
            property = new TagPropertyFactory();
            RegisterProperties(property);
        }

        protected string Name { get; }

        public string this[string name]
        {
            get => Attributes[name].Get();
            set => Attributes?[name].Set(value);
        }

        public Tag this[Tag tag]
        {
            get => Children.Find(p => p == tag);
            set => Children[Children.IndexOf(tag)] = value;
        }

        public Tag this[int position]
        {
            get { return Children.ElementAtOrDefault(position); }
            set { Children[position] = value; }
        }

        IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
        {
            return Attributes
                .Select(attribute => new KeyValuePair<string, string>(attribute.Key, attribute.Value.Get()))
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<Tag> GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        internal virtual void RegisterProperties(TagPropertyFactory factory)
        {
            factory.Default<TagProperty>();
            factory.Register<ListTagProperty>("class");
        }

        public void Add(object obj)
        {
            Add(StringPairs.FromObject(obj));
        }

        public void Add(string text)
        {
            Children.Add(new Text(text));
        }

        public void Add(Tag tag)
        {
            Children.Add(tag);
        }

        public void Add(IEnumerable<KeyValuePair<string, string>> attributes)
        {
            foreach (var pair in attributes)
            {
                Add(pair.Key, pair.Value);
            }
        }

        public void Add(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            name = name.ToLower();
            if (Attributes.ContainsKey(name))
            {
                Attributes[name].Add(value);
            }
            else
            {
                var attribute = property.Create(name);
                if (attribute != null)
                {
                    attribute.Set(value);
                    Attributes.Add(name, attribute);
                }
            }
        }

        public override string ToString()
        {
            return ToString(new DefaultTagWriterFactory());
        }

        public virtual string ToString(TagWriterFactory factory)
        {
            var writer = factory.CreateWriter(this);

            writer.OpenStartTag(Name);
            foreach (var attribute in Attributes)
            {
                writer.Attribute(attribute.Key, attribute.Value.Get());
            }
            writer.CloseStartTag(Name);
            foreach (var child in Children)
            {
                writer.Contents(child.ToString(factory));
            }
            writer.OpenEndTag(Name);
            writer.CloseEndTag(Name);

            return writer.GetOutput();
        }

        private class Text : Tag
        {
            private readonly string text;

            public Text(string text)
                : base(string.Empty)
            {
                this.text = text;
            }

            public override string ToString()
            {
                return text;
            }

            public override string ToString(TagWriterFactory factory)
            {
                var writer = factory.CreateWriter(this);
                writer.Text(text);
                return writer.GetOutput();
            }
        }
    }
}