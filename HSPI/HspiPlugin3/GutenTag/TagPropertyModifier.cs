using System.Collections;
using System.Collections.Generic;

namespace GutenTag
{
    public class TagPropertyModifier : IEnumerable<TagPropertyModifier>
    {
        private TagPropertyModifier next;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<TagPropertyModifier> GetEnumerator()
        {
            return Chain().GetEnumerator();
        }

        private IEnumerable<TagPropertyModifier> Chain()
        {
            yield return this;
            if (next != null)
            {
                foreach (var modifier in next.Chain())
                {
                    yield return modifier;
                }
            }
        }

        public string ModifyReceived(string value, string current)
        {
            value = OnReceive(value, current);
            if (next != null)
            {
                value = next.ModifyReceived(value, current);
            }
            return value;
        }

        public string ModifyBeforeAdd(string value, string current)
        {
            value = BeforeAdd(value, current);
            if (next != null)
            {
                value = next.ModifyBeforeAdd(value, current);
            }
            return value;
        }

        public string ModifyBeforeSet(string value, string current)
        {
            value = BeforeSet(value, current);
            if (next != null)
            {
                value = next.ModifyBeforeSet(value, current);
            }
            return value;
        }

        protected virtual string OnReceive(string value, string current)
        {
            return value;
        }

        protected virtual string BeforeSet(string value, string current)
        {
            return value;
        }

        protected virtual string BeforeAdd(string value, string current)
        {
            return value;
        }

        public void Add(TagPropertyModifier modifier)
        {
            if (modifier != null)
            {
                if (next == null)
                {
                    next = modifier;
                }
                else
                {
                    next.Add(modifier);
                }
            }
        }
    }
}