using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Hspi.HspiPlugin3
{
    public class TreeNodeCollection<T> : IEnumerable<TreeNodeCollection<T>>
    {

        public T Data { get; set; }
        public TreeNodeCollection<T> Parent { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ICollection<TreeNodeCollection<T>> Children { get; set; }

        public bool IsRoot => Parent == null;

        public bool IsLeaf => Children.Count == 0;

        public int Level
        {
            get
            {
                if (IsRoot)
                {
                    return 0;
                }

                return Parent.Level + 1;
            }
        }


        public TreeNodeCollection(T data)
        {
            Data = data;
            Children = new LinkedList<TreeNodeCollection<T>>();

            // ReSharper disable once UseObjectOrCollectionInitializer
            ElementsIndex = new LinkedList<TreeNodeCollection<T>>();
            ElementsIndex.Add(this);
        }

        public TreeNodeCollection<T> AddChild(T child)
        {
            var childNode = new TreeNodeCollection<T>(child) { Parent = this };
            Children.Add(childNode);

            RegisterChildForSearch(childNode);

            return childNode;
        }

        public override string ToString()
        {
            return Data != null ? Data.ToString() : "[data null]";
        }


        #region searching

        private ICollection<TreeNodeCollection<T>> ElementsIndex { get; }

        private void RegisterChildForSearch(TreeNodeCollection<T> nodeCollection)
        {
            ElementsIndex.Add(nodeCollection);
            Parent?.RegisterChildForSearch(nodeCollection);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public TreeNodeCollection<T> FindTreeNode(Func<TreeNodeCollection<T>, bool> predicate)
        {
            return ElementsIndex.FirstOrDefault(predicate);
        }

        #endregion


        #region iterating

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<TreeNodeCollection<T>> GetEnumerator()
        {
            yield return this;
            foreach (var directChild in Children)
            {
                foreach (var anyChild in directChild)
                {
                    yield return anyChild;
                }
            }
        }

        #endregion


    }
}