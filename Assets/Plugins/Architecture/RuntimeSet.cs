using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Architecture
{
    public abstract class RuntimeSet<T> : ScriptableObject, IEnumerable<T>
    {
        public List<T> Items = new List<T>();

        public void Add(T item)
        {
            if (!Items.Contains(item))
                Items.Add(item);
        }

        public bool Contains(T item)
        {
            return Items.Contains(item);
        }

        public int IndexOf(T item)
        {
            return Items.IndexOf(item);
        }

        public void Remove(T item)
        {
            Items.Remove(item);
        }

        public int Count
        {
            get { return Items.Count; }
        }

        public T this[int i]
        {
            get { return Items[i]; }
            set { Items[i] = value; }
        }
        
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return Items.GetEnumerator();
        }
    }
}