using System.Text;

namespace GutenTag
{
    public abstract class TagWriter
    {
        private readonly TagWriter next;

        public TagWriter(TagWriter next = null)
        {
            this.next = next;
            Output = next == null ? new StringBuilder() : next.Output;
        }

        protected StringBuilder Output { get; }

        public virtual string GetOutput()
        {
            return Output.ToString();
        }

        public virtual void OpenStartTag(string name)
        {
            if (next != null)
            {
                next.OpenStartTag(name);
            }
        }

        public virtual void CloseStartTag(string name)
        {
            if (next != null)
            {
                next.CloseStartTag(name);
            }
        }

        public virtual void Attribute(string name, string value)
        {
            if (next != null)
            {
                next.Attribute(name, value);
            }
        }

        public virtual void OpenEndTag(string name)
        {
            if (next != null)
            {
                next.OpenEndTag(name);
            }
        }

        public virtual void CloseEndTag(string name)
        {
            if (next != null)
            {
                next.CloseEndTag(name);
            }
        }

        public virtual void Contents(string contents)
        {
            if (next != null)
            {
                next.Contents(contents);
            }
        }

        public virtual void Text(string text)
        {
            if (next != null)
            {
                next.Text(text);
            }
        }
    }
}