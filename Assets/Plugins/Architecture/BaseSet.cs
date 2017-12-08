using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Architecture
{
    public abstract class BaseSet : ScriptableObject, IEnumerable
    {
        public abstract int Count { get; }

        public abstract IEnumerator GetEnumerator();
        
        public abstract object GetValue();
        public abstract void SetValue(object obj);
    }
}