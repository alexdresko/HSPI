using System;
using System.Collections.Generic;

namespace GutenTag
{
    public class DefaultTagWriterFactory : TagWriterFactory
    {
        private static readonly Dictionary<Type, Func<TagWriter, TagWriter>> Writers =
            new Dictionary<Type, Func<TagWriter, TagWriter>>();

        static DefaultTagWriterFactory()
        {
            Register<VoidAttribute>(t => new VoidWriter(t));
            Register<CollapsibleAttribute>(t => new CollapsibleWriter(t));
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static void Register(Type type, Func<TagWriter, TagWriter> decorator)
        {
            if (decorator != null)
            {
                if (Writers.ContainsKey(type))
                {
                    Writers[type] = writer => decorator(Writers[type](writer));
                }
                else
                {
                    Writers[type] = decorator;
                }
            }
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static void Register<T>(Func<TagWriter, TagWriter> decorator)
        {
            Register(typeof(T), decorator);
        }

        // ReSharper disable once UnusedMember.Global
        public static void Unregister(Type type)
        {
            if (Writers.ContainsKey(type))
            {
                Writers.Remove(type);
            }
        }

        private TagWriter Decorate(TagWriter writer, Type type)
        {
            if (Writers.ContainsKey(type))
            {
                writer = Writers[type](writer);
            }
            return writer;
        }

        private TagWriter CreateWriter(Type type)
        {
            TagWriter writer = new DefaultWriter();

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var attribute in type.GetCustomAttributes(true))
            {
                writer = Decorate(writer, attribute.GetType());
            }

            return writer;
        }

        public override TagWriter CreateWriter(Tag tag)
        {
            return tag == null ? new NullWriter() : CreateWriter(tag.GetType());
        }

        private class DefaultWriter : TagWriter
        {
            public DefaultWriter(TagWriter next = null)
                : base(next)
            {
            }

            public override void OpenStartTag(string name)
            {
                Output.Append("<" + name);
            }

            public override void CloseStartTag(string name)
            {
                Output.Append(">");
            }

            public override void Attribute(string name, string value)
            {
                if (name == null)
                {
                    return;
                }
                if (value == null)
                {
                    Output.Append(" " + name);
                }
                Output.AppendFormat(" {0}=\"{1}\"", name, value);
            }

            public override void Contents(string contents)
            {
                Output.Append(contents);
            }

            public override void OpenEndTag(string name)
            {
                Output.Append("</" + name);
            }

            public override void CloseEndTag(string name)
            {
                Output.Append(">");
            }

            public override void Text(string text)
            {
                Output.Append(text);
            }
        }

        private class NullWriter : TagWriter
        {
        }

        private class VoidWriter : TagWriter
        {
            public VoidWriter(TagWriter next)
                : base(next)
            {
            }

            public override void Contents(string contents)
            {
            }

            public override void OpenEndTag(string name)
            {
            }

            public override void CloseEndTag(string name)
            {
            }
        }

        private class CollapsibleWriter : TagWriter
        {
            private bool _contentsWritten;

            public CollapsibleWriter(TagWriter next)
                : base(next)
            {
            }

            public override void Contents(string contents)
            {
                _contentsWritten = true;
                base.Contents(contents);
            }

            public override void OpenEndTag(string name)
            {
                if (_contentsWritten)
                {
                    base.OpenEndTag(name);
                }
                else
                {
                    Output.Insert(Output.Length - 1, " /");
                }
            }

            public override void CloseEndTag(string name)
            {
                if (_contentsWritten)
                {
                    base.CloseEndTag(name);
                }
            }
        }
    }
}