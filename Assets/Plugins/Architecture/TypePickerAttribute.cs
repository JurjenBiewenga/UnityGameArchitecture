using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Architecture
{
    public class TypePickerAttribute : PropertyAttribute
    {
        public Type baseType;
        public TypePickerAttribute()
        {}

        public TypePickerAttribute(Type baseType)
        {
            this.baseType = baseType;
        }
    }
}