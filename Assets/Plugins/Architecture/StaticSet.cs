using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StaticSet<T> : ScriptableObject
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
}
