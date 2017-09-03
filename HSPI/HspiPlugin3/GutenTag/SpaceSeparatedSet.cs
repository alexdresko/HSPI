using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GutenTag
{
    internal class SpaceSeparatedSet : ISet<string>
    {
        private readonly HashSet<string> _hash;

        public SpaceSeparatedSet()
        {
            _hash = new HashSet<string>();
        }

        // ReSharper disable once UnusedMember.Global
        public SpaceSeparatedSet(string classes)
            : this()
        {
            Add(classes);
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public SpaceSeparatedSet(IEnumerable<string> classes)
            : this()
        {
            foreach (var c in classes)
            {
                Add(c);
            }
        }

        void ICollection<string>.Add(string item)
        {
            Add(item);
        }

        public void Clear()
        {
            _hash.Clear();
        }

        public bool Contains(string cssClass)
        {
            return _hash.Contains(cssClass);
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            _hash.CopyTo(array, arrayIndex);
        }

        /// <returns>true if the set changed</returns>
        public bool Remove(string names)
        {
            return ParsedRemove(ParseNames(names));
        }

        public int Count => _hash.Count;

        public bool IsReadOnly => false;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _hash.GetEnumerator();
        }

        public IEnumerator<string> GetEnumerator()
        {
            return _hash.GetEnumerator();
        }

        /// <returns>true if the set changed</returns>
        public bool Add(string names)
        {
            return ParsedAdd(ParseNames(names));
        }

        public void UnionWith(IEnumerable<string> other)
        {
            _hash.UnionWith(new SpaceSeparatedSet(other));
        }

        public void IntersectWith(IEnumerable<string> other)
        {
            _hash.IntersectWith(new SpaceSeparatedSet(other));
        }

        public void ExceptWith(IEnumerable<string> other)
        {
            _hash.ExceptWith(new SpaceSeparatedSet(other));
        }

        public void SymmetricExceptWith(IEnumerable<string> other)
        {
            _hash.SymmetricExceptWith(new SpaceSeparatedSet(other));
        }

        public bool IsSubsetOf(IEnumerable<string> other)
        {
            return _hash.IsSubsetOf(new SpaceSeparatedSet(other));
        }

        public bool IsSupersetOf(IEnumerable<string> other)
        {
            return _hash.IsSupersetOf(new SpaceSeparatedSet(other));
        }

        public bool IsProperSupersetOf(IEnumerable<string> other)
        {
            return _hash.IsProperSupersetOf(new SpaceSeparatedSet(other));
        }

        public bool IsProperSubsetOf(IEnumerable<string> other)
        {
            return _hash.IsProperSubsetOf(new SpaceSeparatedSet(other));
        }

        public bool Overlaps(IEnumerable<string> other)
        {
            return _hash.Overlaps(new SpaceSeparatedSet(other));
        }

        public bool SetEquals(IEnumerable<string> other)
        {
            return _hash.SetEquals(new SpaceSeparatedSet(other));
        }

        private IEnumerable<string> ParseNames(IEnumerable<string> names)
        {
            return names.SelectMany(s => s.Split(' '));
        }

        private IEnumerable<string> ParseNames(string names)
        {
            return ParseNames(new[] {names});
        }

        private bool ParsedRemove(IEnumerable<string> names)
        {
            var enumerable = names as string[] ?? names.ToArray();
            var result = enumerable.Any();
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var name in enumerable)
            {
                result |= _hash.Remove(name);
            }
            return result;
        }

        /// <returns>true if the set changed</returns>
        // ReSharper disable once UnusedMember.Global
        public bool Remove(IEnumerable<string> names)
        {
            return ParsedRemove(ParseNames(names));
        }

        private bool ParsedAdd(IEnumerable<string> names)
        {
            var enumerable = names as string[] ?? names.ToArray();
            var result = enumerable.Any();
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var name in enumerable)
            {
                result |= _hash.Add(name);
            }
            return result;
        }

        /// <returns>true if the set changed</returns>
        // ReSharper disable once UnusedMember.Global
        public bool Add(IEnumerable<string> names)
        {
            return ParsedAdd(ParseNames(names));
        }

        public override string ToString()
        {
            switch (Count)
            {
                case 0: return string.Empty;
                case 1: return _hash.First();
                default: return _hash.Aggregate((a, b) => a + " " + b);
            }
        }
    }
}