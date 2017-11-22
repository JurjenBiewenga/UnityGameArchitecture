using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RuntimeSet<T> : ScriptableObject
{
    public List<T> Items = new List<T>();

    public void Add(T item)
    {
        if(!Items.Contains(item))
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

    public T this[int i]
    {
        get { return Items[i]; }
        set { Items[i] = value; }
    }
}
