using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Architecture
{
	public abstract class StaticSet<T> : ScriptableObject, IEnumerable<T>
	{
		[SerializeField]
		private List<T> Items = new List<T>();

		public bool Contains(T item)
		{
			return Items.Contains(item);
		}

		public int IndexOf(T item)
		{
			return Items.IndexOf(item);
		}

		public T this[int i]
		{
			get { return Items[i]; }
			private set { Items[i] = value; }
		}

		public int Count
		{
			get { return Items.Count; }
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