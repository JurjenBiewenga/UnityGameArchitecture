using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Architecture
{
    public abstract class BaseVariable : ScriptableObject
    {
        public abstract object GetValue();
        public abstract void SetValue(object obj);
    }
}